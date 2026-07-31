using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    public List<IGameData> objectsData = new List<IGameData>();
    public List<IDataPersistence> dataPersistenceObjects = new List<IDataPersistence>();
    private DataFileHandler dataFileHandler;
    public static DataPersistenceManager instance { get; private set; }

    [Header("Save Folders (relative to persistentDataPath)")]
    public string upgradesFolder = "Upgrades";
    public string customFolder = "Custom";
    public string settingsFolder = "Settings";
    public string appSettingsFolder = "AppSettings";

    [Header("Data Containers")]
    public SaveUpgrades saveUpgrades;
    public SaveCustom saveCustom;       // optional for now, can stay empty
    public SaveSettings saveSettings;   // optional for now, can stay empty
    public SettingsData saveAppSettings; // optional for now, can stay empty

    // Full paths built at runtime, valid on any machine
    private string upgradesPath;
    private string customPath;
    private string settingsPath;
    private string appSettingsPath;

    // factory-default snapshot of each container (json), to reset a vehicle that has no save
    private string upgradesDefaults;
    private string customDefaults;
    private string settingsDefaults;
    private string appSettingsDefaults;
    private bool defaultsCaptured;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            // dupe from a scene reload, the DontDestroy one already exists
            Destroy(gameObject);
            return;
        }
        instance = this;

        upgradesPath = Path.Combine(Application.persistentDataPath, upgradesFolder);
        customPath = Path.Combine(Application.persistentDataPath, customFolder);
        settingsPath = Path.Combine(Application.persistentDataPath, settingsFolder);
        appSettingsPath = Path.Combine(Application.persistentDataPath, appSettingsFolder);

        Directory.CreateDirectory(upgradesPath);
        Directory.CreateDirectory(customPath);
        Directory.CreateDirectory(settingsPath);
        Directory.CreateDirectory(appSettingsPath);

        dataFileHandler = new DataFileHandler();
    }

    private void Start()
    {
        // Fallback: catch any IDataPersistence that did not register itself in Awake
        dataPersistenceObjects = findAllDataPersistence();

        // grab the factory defaults now, while the containers still have their authored values
        CaptureDefaults();
    }

    public void LoadGameFor(string vehicleId)
    {
        RefreshRuntimeLists();
        CaptureDefaults();

        LoadOrReset(saveUpgrades, Path.Combine(upgradesPath, $"upgrades_{vehicleId}.json"), upgradesDefaults);
        LoadOrReset(saveCustom, Path.Combine(customPath, $"custom_{vehicleId}.json"), customDefaults);
        LoadOrReset(saveSettings, Path.Combine(settingsPath, $"settings_{vehicleId}.json"), settingsDefaults);
        LoadOrReset(saveAppSettings, Path.Combine(appSettingsPath, $"{vehicleId}.json"), appSettingsDefaults);

        // Generic dispatch: every registered IDataPersistence receives
        // the data container matching its dataFileName
        DispatchAll((dp, data) => dp.LoadGame(data));
    }

    public void SaveGameFor(string vehicleId)
    {
        RefreshRuntimeLists();

        DispatchAll((dp, data) => dp.SaveGame(data));

        if (saveUpgrades != null)
            dataFileHandler.save(saveUpgrades, Path.Combine(upgradesPath, $"upgrades_{vehicleId}.json"));
        if (saveCustom != null)
            dataFileHandler.save(saveCustom, Path.Combine(customPath, $"custom_{vehicleId}.json"));
        if (saveSettings != null)
            dataFileHandler.save(saveSettings, Path.Combine(settingsPath, $"settings_{vehicleId}.json"));
        if (saveAppSettings != null)
            dataFileHandler.save(saveAppSettings, Path.Combine(appSettingsPath, $"{vehicleId}.json"));
    }

    private void CaptureDefaults()
    {
        if (defaultsCaptured) return;

        if (saveUpgrades != null) upgradesDefaults = JsonUtility.ToJson(saveUpgrades);
        if (saveCustom != null) customDefaults = JsonUtility.ToJson(saveCustom);
        if (saveSettings != null) settingsDefaults = JsonUtility.ToJson(saveSettings);
        if (saveAppSettings != null) appSettingsDefaults = JsonUtility.ToJson(saveAppSettings, saveAppSettings.usePrettyPrint);

        defaultsCaptured = true;
    }

    // load the container from disk, or reset it to defaults if this vehicle has no save yet
    private void LoadOrReset(IGameData container, string path, string defaultsJson)
    {
        if (container == null) return;

        if (!dataFileHandler.load(container, path) && !string.IsNullOrEmpty(defaultsJson))
            JsonUtility.FromJsonOverwrite(defaultsJson, container);
    }

    // rebuild the lists from live objects so a scene reload doesn't leave stale/duplicate refs around
    private void RefreshRuntimeLists()
    {
        objectsData.Clear();
        if (saveUpgrades != null) objectsData.Add(saveUpgrades);
        if (saveCustom != null) objectsData.Add(saveCustom);
        if (saveSettings != null) objectsData.Add(saveSettings);
        if (saveAppSettings != null) objectsData.Add(saveAppSettings);

        dataPersistenceObjects = findAllDataPersistence();
    }

    // Single generic loop shared by load and save (no duplicated matching logic)
    private void DispatchAll(System.Action<IDataPersistence, IGameData> action)
    {
        foreach (IDataPersistence dp in dataPersistenceObjects)
        {
            foreach (IGameData data in objectsData)
            {
                if (data.getDataFileName() == dp.getDataFileName())
                    action(dp, data);
            }
        }
    }

    private void OnApplicationQuit()
    {
        // only the garage has tuning to save on quit, and its guarded save won't clobber it
        GarageDisplayManager garage = FindFirstObjectByType<GarageDisplayManager>();
        if (garage != null)
            garage.SaveIfCustomizing();
    }

    private List<IDataPersistence> findAllDataPersistence()
    {
        IEnumerable<IDataPersistence> found =
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence>();

        return found.Distinct().ToList();
    }
}