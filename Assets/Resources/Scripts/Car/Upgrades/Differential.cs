using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cycles the vehicle's differential between Open, Limited-slip and Fully Locked.
/// </summary>
public class Differential : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

    public RCCP_CarController controller;

    [Tooltip("Text showing the current differential mode.")]
    public Text typeText;

    // Direct is deliberately left out: it bypasses the differential rather than being a tier
    // above Locked.
    private static readonly RCCP_Differential.DifferentialType[] Steps = {
        RCCP_Differential.DifferentialType.Open,
        RCCP_Differential.DifferentialType.Limited,
        RCCP_Differential.DifferentialType.FullLocked
    };

    private static readonly string[] StepLabels = { "Open", "Limited Slip", "Locked" };

    // 1 = Limited, RCCP's own default.
    private int currentIdx = 1;

    public static Differential instance;

    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
            tmp.differentialType = currentIdx;
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
            currentIdx = Mathf.Clamp(tmp.differentialType, 0, Steps.Length - 1);

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

    private void Start() {
        RefreshUI();
    }

    public void OnButtonClicked() {
        currentIdx = (currentIdx + 1) % Steps.Length;

        ApplyToController();
        RefreshUI();
    }

    // Differentials is an array: an AWD car has one per axle plus a centre one.
    void ApplyToController() {
        if (controller == null || controller.Differentials == null)
            return;

        foreach (RCCP_Differential differential in controller.Differentials) {
            if (differential != null)
                differential.differentialType = Steps[currentIdx];
        }
    }

    void RefreshUI() {
        if (typeText != null)
            typeText.text = StepLabels[currentIdx];
    }
}
