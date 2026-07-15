using UnityEngine;

public class SaveCustom : MonoBehaviour, IGameData {

    [Header("Save")]
    public string dataDirPath;
    public string dataFileName;
    public bool useEncryption;
    public string encryption;
    public bool usePrettyPrint;


    public int currentMat;

    public int currentSpoiler;

    public static SaveCustom saveInstance;

    private void Awake() {
        if (saveInstance == null) saveInstance = this;
        DataPersistenceManager.instance.objectsData.Add(saveInstance);
    }

    void IGameData.setData(IGameData data) {
        SaveCustom tmp = data as SaveCustom;
        currentMat = tmp.currentMat;

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
