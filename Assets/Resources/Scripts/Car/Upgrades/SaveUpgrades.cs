using UnityEngine;

public class SaveUpgrades : MonoBehaviour, IGameData 
{
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
    public int antiRollBarValue;

    // Suspension
    public float newY;
    public float tilts;

    // Tire
    public float slickness;

    // Transmision
    public float[] gearRatios;
    public float brakePower;

    // Index into Differential's own list of modes, not RCCP's enum value. 1 = Limited, which is
    // RCCP's default, so a fresh save leaves the vehicle as authored.
    public int differentialType = 1;

    private void Awake()
    {
        DataPersistenceManager.instance.objectsData.Add(this);
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
        antiRollBarValue = tmp.antiRollBarValue;

        // Suspension
        newY = tmp.newY;
        tilts = tmp.tilts;

        // Tire
        slickness = tmp.slickness;

        // Transmision
        gearRatios = tmp.gearRatios;
        brakePower = tmp.brakePower;
        differentialType = tmp.differentialType;
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
