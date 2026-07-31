using UnityEngine;

[System.Serializable]
public class SettingsData : MonoBehaviour, IGameData
{

    [Header("Save")]
    public string dataDirPath;
    public string dataFileName;
    public bool useEncryption;
    public string encryption;
    public bool usePrettyPrint;

    public int resolution;
    public bool fullscreen;
    public bool Vsync;
    public int maxfps;
    public int colorBlindOption;
    public float masterVolume;
    public float SFXVolume;
    public float musicVolume;

    public static SettingsData saveInstance;

    private void Awake()
    {
        if (saveInstance == null) saveInstance = this;
        DataPersistenceManager.instance.objectsData.Add(saveInstance);
    }

    void IGameData.setData(IGameData data)
    {
        SettingsData tmp = data as SettingsData;

        resolution = tmp.resolution;
        fullscreen = tmp.fullscreen;
        Vsync = tmp.Vsync;
        maxfps = tmp.maxfps;
        colorBlindOption = tmp.colorBlindOption;
        masterVolume = tmp.masterVolume;
        SFXVolume = tmp.SFXVolume;
        musicVolume = tmp.musicVolume;
    }

    string IGameData.getDataDirPath()
    {
        return dataDirPath;
    }
    string IGameData.getDataFileName()
    {
        return dataFileName;
    }
    bool IGameData.useEncryption()
    {
        return useEncryption;
    }
    string IGameData.getEncryptionKey()
    {
        return encryption;
    }
    bool IGameData.usePrettyPrint()
    {
        return usePrettyPrint;
    }
}
