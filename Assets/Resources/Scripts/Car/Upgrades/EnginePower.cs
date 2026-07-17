using UnityEngine;
using UnityEngine.UI;

public class EnginePower : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

    public RCCP_CarController controller;

    public Text minText;
    public Text maxText;

    public int minEnginePowerValue = 700;
    public int maxEnginePowerValue = 8000;

    private int enginePowerIdx = 0;

    public static EnginePower instance;

    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            tmp.minEnginePowerValue = minEnginePowerValue;
            tmp.maxEnginePowerValue = maxEnginePowerValue;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            minEnginePowerValue = tmp.minEnginePowerValue;
            maxEnginePowerValue = tmp.maxEnginePowerValue;
        }

        // LoadGame runs after SetController, so the controller still holds the pre-load values
        // until we push them again here.
        ApplyToController();
        RefreshUI();
    }

    public string getDataFileName() {
        return dataFileName;
    }

    public void SetController(RCCP_CarController newController) {
        controller = newController;
        ApplyToController();
        RefreshUI();
    }

    void Awake() {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);
    }

    void Start() {
        RefreshUI();
    }

    public void OnButtonClicked() {
        if (enginePowerIdx < 2) enginePowerIdx += 1;
        if (enginePowerIdx == 0) {
            minEnginePowerValue = 700;
            maxEnginePowerValue = 8000;
        }
        else if (enginePowerIdx == 1) {
            minEnginePowerValue = 1200;
            maxEnginePowerValue = 8500;
        }
        else if (enginePowerIdx == 2) {
            minEnginePowerValue = 1700;
            maxEnginePowerValue = 9000;
        }

        ApplyToController();
        RefreshUI();
    }

    void ApplyToController() {
        if (controller == null || controller.Engine == null)
            return;

        controller.Engine.minEngineRPM = minEnginePowerValue;
        controller.Engine.maxEngineRPM = maxEnginePowerValue;
    }

    // Called only when a value actually changes. Writing .text marks the Canvas dirty and
    // ToString() allocates, so doing this per frame in Update() rebuilt the UI continuously.
    void RefreshUI() {
        if (minText != null) minText.text = minEnginePowerValue.ToString();
        if (maxText != null) maxText.text = maxEnginePowerValue.ToString();
    }
}
