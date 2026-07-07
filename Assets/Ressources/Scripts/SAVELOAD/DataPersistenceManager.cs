using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using Unity.VisualScripting;


public class DataPersistenceManager : MonoBehaviour
{
    public List<IGameData> objectsData;
    public List<IDataPersistence> dataPersistenceObjects;
    private DataFileHandler dataFileHandler;
    public static DataPersistenceManager instance { get; private set; }

    [Header("Save Customization")]
    public string customPath;
    public SaveCustom saveCustom;
    public ChangeWheels changeWheels;
    public ChangeSpoilers changeSpoilers;

    [Header("Save Upgrades")]
    public string upgradesPath;
    public SaveUpgrades saveUpgrades;
    public AddTurbo addTurbo;
    public AntiRollBar antiRollBar;
    public Brake brake;
    public CarbonFiberBody carbonFiberBody;
    //public Differential differential;
    public EnginePower enginePower;
    public GearboxRatio gearboxRatio;
    public Lightened lightened;
    public Reinforcement reinforcement;
    public Slick slick;
    public SuspensionUpgrade suspensionUpgrade;

    [Header("Save Settings")]
    public string settingsPath;
    public SaveSettings saveSettings;
    public Braking braking;
    public Camber camber;
    public DrivingAid drivingAid;
    public Gearbox Gearbox;
    public Grip grip;
    public Others others;
    public PropulsionType propulsionType;
    public Suspension suspension;
    public Wheels wheels;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
        }
        instance = this;
    }


    private void Start()
    {

        this.dataFileHandler = new DataFileHandler();
        this.dataPersistenceObjects = findAllDataPersistence();
        LoadGame();
        
    }

    public void NewGame()
    {
        this.objectsData = findAllGameData();
    }

    public void LoadGame()
    {
        if (this.objectsData == null)
        {
            NewGame();
        }

        // Customization
        dataFileHandler.load(saveCustom, customPath);
        changeWheels.LoadGame(saveCustom);
        changeSpoilers.LoadGame(saveCustom);

        // Upgrades
        dataFileHandler.load(saveUpgrades, upgradesPath);
        addTurbo.LoadGame(saveUpgrades);
        antiRollBar.LoadGame(saveUpgrades);
        brake.LoadGame(saveUpgrades);
        carbonFiberBody.LoadGame(saveUpgrades);
        //differential.LoadGame(saveUpgrades);
        enginePower.LoadGame(saveUpgrades);
        gearboxRatio.LoadGame(saveUpgrades);
        lightened.LoadGame(saveUpgrades);
        reinforcement.LoadGame(saveUpgrades);
        slick.LoadGame(saveUpgrades);
        suspensionUpgrade.LoadGame(saveUpgrades);


        // Settings
        dataFileHandler.load(saveSettings, settingsPath);
        braking.LoadGame(saveSettings);
        camber.LoadGame(saveSettings);
        drivingAid.LoadGame(saveSettings);
        Gearbox.LoadGame(saveSettings);
        grip.LoadGame(saveSettings);
        others.LoadGame(saveSettings);
        propulsionType.LoadGame(saveSettings);
        suspension.LoadGame(saveSettings);
        wheels.LoadGame(saveSettings);

    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistence in dataPersistenceObjects)
        {
            foreach (IGameData data in objectsData)
            {
                if (data.getDataFileName() == dataPersistence.getDataFileName())
                {
                    dataPersistence.SaveGame(data);
                    dataFileHandler.save(data);
                }
            }
        }
        
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }


    private List<IGameData> findAllGameData()
    {
        IEnumerable<IGameData> objectsData = FindObjectsByType<MonoBehaviour>().OfType<IGameData>();

        return new List<IGameData>(objectsData);
    }
    private List<IDataPersistence> findAllDataPersistence()
    {
        IEnumerable<IDataPersistence> dataPresistences = FindObjectsByType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPresistences);
    }
}
