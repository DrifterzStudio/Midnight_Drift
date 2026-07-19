using UnityEngine;
using UnityEngine.UI;

public class EnginePower : MonoBehaviour, IDataPersistence, IVehicleDependent
{

    public string dataFileName;

    public RCCP_CarController controller;

    public Text minText;
    public Text maxText;

    public int minEnginePowerValue = 700;
    public int maxEnginePowerValue = 8000;

    private int enginePowerIdx = 0;

    public static EnginePower instance;

    public void SaveGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
        {
            tmp.minEnginePowerValue = minEnginePowerValue;
            tmp.maxEnginePowerValue = maxEnginePowerValue;
        }
    }

    public void LoadGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null && tmp.maxEnginePowerValue > 0)
        {
            minEnginePowerValue = tmp.minEnginePowerValue;
            maxEnginePowerValue = tmp.maxEnginePowerValue;
        }
        // restore the level from the loaded values so the next click keeps going right
        enginePowerIdx = DeriveEnginePowerIdx();

        ApplyToController();
        RefreshUI();
    }

    public string getDataFileName()
    {
        return dataFileName;
    }

    public void SetController(RCCP_CarController newController)
    {
        controller = newController;
        ApplyToController();
        RefreshUI();
    }

    void Awake()
    {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);
    }

    void Start()
    {
        RefreshUI();
    }

    public void OnButtonClicked()
    {
        // cycle stock -> 1 -> 2 -> stock
        enginePowerIdx = (enginePowerIdx + 1) % 3;

        if (enginePowerIdx == 1)
        {
            minEnginePowerValue = 1200;
            maxEnginePowerValue = 8500;
        }
        else if (enginePowerIdx == 2)
        {
            minEnginePowerValue = 1700;
            maxEnginePowerValue = 9000;
        }
        else
        {
            minEnginePowerValue = 700;
            maxEnginePowerValue = 8000;
        }

        ApplyToController();
        RefreshUI();
    }

    // figure out the level from the current engine values
    int DeriveEnginePowerIdx()
    {
        if (maxEnginePowerValue >= 9000) return 2;
        if (maxEnginePowerValue >= 8500) return 1;
        return 0;
    }

    void ApplyToController()
    {
        if (controller == null || controller.Engine == null)
            return;

        controller.Engine.minEngineRPM = minEnginePowerValue;
        controller.Engine.maxEngineRPM = maxEnginePowerValue;
    }

    // only called when a value changes. writing .text dirties the canvas and ToString() allocates,
    // so doing it every frame in Update() rebuilt the ui constantly
    void RefreshUI()
    {
        if (minText != null) minText.text = minEnginePowerValue.ToString();
        if (maxText != null) maxText.text = maxEnginePowerValue.ToString();
    }
}
