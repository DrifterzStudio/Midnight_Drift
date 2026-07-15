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

    [Header("Data Containers")]
    public SaveUpgrades saveUpgrades;
    public SaveCustom saveCustom;       // optional for now, can stay empty
    public SaveSettings saveSettings;   // optional for now, can stay empty

    // Full paths built at runtime, valid on any machine
    private string upgradesPath;
    private string customPath;
    private string settingsPath;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
            return;
        }
        instance = this;

        upgradesPath = Path.Combine(Application.persistentDataPath, upgradesFolder);
        customPath = Path.Combine(Application.persistentDataPath, customFolder);
        settingsPath = Path.Combine(Application.persistentDataPath, settingsFolder);

        Directory.CreateDirectory(upgradesPath);
        Directory.CreateDirectory(customPath);
        Directory.CreateDirectory(settingsPath);

        dataFileHandler = new DataFileHandler();
    }

    private void Start()
    {
        // Fallback: catch any IDataPersistence that did not register itself in Awake
        dataPersistenceObjects = findAllDataPersistence();
    }

    public void LoadGameFor(string vehicleId)
    {
        if (saveUpgrades != null)
            dataFileHandler.load(saveUpgrades, Path.Combine(upgradesPath, $"upgrades_{vehicleId}.json"));
        if (saveCustom != null)
            dataFileHandler.load(saveCustom, Path.Combine(customPath, $"custom_{vehicleId}.json"));
        if (saveSettings != null)
            dataFileHandler.load(saveSettings, Path.Combine(settingsPath, $"settings_{vehicleId}.json"));

        // Generic dispatch: every registered IDataPersistence receives
        // the data container matching its dataFileName
        DispatchAll((dp, data) => dp.LoadGame(data));
    }

    public void SaveGameFor(string vehicleId)
    {
        DispatchAll((dp, data) => dp.SaveGame(data));

        if (saveUpgrades != null)
            dataFileHandler.save(saveUpgrades, Path.Combine(upgradesPath, $"upgrades_{vehicleId}.json"));
        if (saveCustom != null)
            dataFileHandler.save(saveCustom, Path.Combine(customPath, $"custom_{vehicleId}.json"));
        if (saveSettings != null)
            dataFileHandler.save(saveSettings, Path.Combine(settingsPath, $"settings_{vehicleId}.json"));
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
        if (GameSession.SelectedVehicle != null)
            SaveGameFor(GameSession.SelectedVehicle.vehicleId);
    }

    private List<IDataPersistence> findAllDataPersistence()
    {
        IEnumerable<IDataPersistence> found =
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence>();

        return found.Distinct().ToList();
    }
}