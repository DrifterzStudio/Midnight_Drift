using UnityEditor.TerrainTools;
using System.Collections.Generic;
using UnityEngine;

public class SaveCustom : MonoBehaviour, IGameData {

    public string dataDirPath;
    public string dataFileName;
    public bool useEncryption;
    public string encryption;
    public bool usePrettyPrint;


    public List<Material> materials;
    public int currentMat;

    public List<GameObject> spoilers;
    public int currentSpoiler;

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
