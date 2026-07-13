using UnityEngine;

public class SaveUpgrades : MonoBehaviour, IGameData {

    public DataPersistenceManager dataPersistence;

    [Header("Save")]
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
    public int lightenedMass;
    public int carbonMass;
    public float newYCenter;
    public int antiRollBarValue;

    // Suspension
    public float newY;
    public float tilts;

    // Tire
    public float slickness;

    // Transmision
    public float[] gearRatios;
    public float brakePower;


    public static SaveUpgrades saveInstance;

    private void Awake() {
        if (saveInstance == null) saveInstance = this;
        dataPersistence.objectsData.Add(saveInstance);
    }

    void IGameData.setData(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;

        // Engine
        minEnginePowerValue = tmp.minEnginePowerValue;
        maxEnginePowerValue = tmp.maxEnginePowerValue;

        // Turbo
        turbo1 = tmp.turbo1;
        turbo2 = tmp.turbo2;

        // Chassis
        lightenedMass = tmp.lightenedMass;
        carbonMass = tmp.carbonMass;
        newYCenter = tmp.newYCenter;
        antiRollBarValue = tmp.antiRollBarValue;

        // Suspension
        newY = tmp.newY;
        tilts = tmp.tilts;

        // Tire
        slickness = tmp.slickness;

        // Transmision 
        gearRatios = tmp.gearRatios;
        brakePower = tmp.brakePower;
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
