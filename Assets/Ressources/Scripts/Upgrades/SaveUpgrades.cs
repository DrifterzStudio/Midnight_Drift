using UnityEngine;

public class SaveUpgrades : MonoBehaviour, IGameData {

    public string dataDirPath;
    public string dataFileName;
    public bool useEncryption;
    public string encryption;
    public bool usePrettyPrint;

    // Engine
    public int minEnginePowerValue;
    public int maxEnginePowerValue;

    // Turbo
    public bool turbo1;
    public bool turbo2;

    // Chassis
    public int antiRollBarValue;

    // Tire
    public float slickness;

    // Transmision
    // Gearbox
    public float[] gearRatios;

    // Brake
    public float brakePower;
    //}

    void IGameData.setData(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;

        // Engine
        minEnginePowerValue = tmp.minEnginePowerValue;
        maxEnginePowerValue = tmp.maxEnginePowerValue;

        // Turbo
        turbo1 = tmp.turbo1;
        turbo2 = tmp.turbo2;

        // Chassis
        antiRollBarValue = tmp.antiRollBarValue;
        // Tire
        slickness = tmp.slickness;

        // Transmision {
        // Gearbox
        gearRatios = tmp.gearRatios;

        // Brake
        brakePower = tmp.brakePower;
        //}
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
