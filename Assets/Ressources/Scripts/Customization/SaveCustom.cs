using UnityEditor.TerrainTools;
using System.Collections.Generic;
using UnityEngine;

public class SaveCustom : MonoBehaviour, IGameData {

    public DataPersistenceManager dataPersistence;

    [Header("Save")]
    public string dataDirPath;
    public string dataFileName;
    public bool useEncryption;
    public string encryption;
    public bool usePrettyPrint;


    public List<Material> materials;
    public int currentMat;

    public List<GameObject> spoilers;
    public int currentSpoiler;

    public static SaveCustom saveInstance;

    private void Awake() {
        if (saveInstance == null) saveInstance = this;
        dataPersistence.objectsData.Add(saveInstance);
    }

    void IGameData.setData(IGameData data) {
        SaveCustom tmp = data as SaveCustom;
        materials = tmp.materials;
        currentMat = tmp.currentMat;

        spoilers = tmp.spoilers;
        currentSpoiler = tmp.currentSpoiler;
    }

    string IGameData.getDataDirPath() {
        return dataDirPath;
    }

    string IGameData.getDataFileName() {
        return dataFileName;
    }

    bool IGameData.useEncryption() {
        return useEncryption;
    }

    string IGameData.getEncryptionKey() {
        return encryption;
    }

    bool IGameData.usePrettyPrint() {
        return usePrettyPrint;
    }
}
