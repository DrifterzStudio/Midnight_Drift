using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;


public class DataPersistenceManager : MonoBehaviour
{
    public List<IGameData> objectsData;
    public List<IDataPersistence> dataPersistenceObjects;
    private DataFileHandler dataFileHandler;
    public static DataPersistenceManager instance { get; private set; }

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

    private void Update() {
    }

    public void NewGame()
    {
        this.objectsData = findAllGameData();
    }

    public void LoadGame()
    {
        if (this.objectsData == null)
        {
            Debug.Log("No save found");
            NewGame();
        }

        foreach(IDataPersistence dataPersistence in dataPersistenceObjects )
        {
            foreach(IGameData data in objectsData)
            {
                if(data.getDataFileName() == dataPersistence.getDataFileName())
                {
                    dataFileHandler.load(data);
                    dataPersistence.LoadGame(data);
                }
            }
        }
            
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
