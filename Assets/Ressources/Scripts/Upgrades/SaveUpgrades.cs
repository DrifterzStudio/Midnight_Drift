using UnityEngine;

public class SaveUpgrades : MonoBehaviour, IGameData {

    public string dataDirPath;
    public string dataFileName;
    public bool useEncryption;
    public string encryption;
    public bool usePrettyPrint;

    // Turbo
    public bool turbo1;
    public bool turbo2;

    // Transmision
    // Gearbox
    public float[] gearRatios;

    void IGameData.setData(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;

        // Turbo
        turbo1 = tmp.turbo1;
        turbo2 = tmp.turbo2;

        // Transmision
        // Gearbox
        gearRatios = tmp.gearRatios;
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
