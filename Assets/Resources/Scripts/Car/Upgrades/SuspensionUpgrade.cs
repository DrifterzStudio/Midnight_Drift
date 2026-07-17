using UnityEngine;
using UnityEngine.UI;

public class SuspensionUpgrade : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

    public RCCP_CarController controller;

    public Text typeText;

    public float newY = 0.16f;

    public float tilts = 5f;

    private int suspensionIdx = 0;

    public static SuspensionUpgrade instance;

    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            tmp.newY = newY;
            tmp.tilts = tilts;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            newY = tmp.newY;
            tilts = tmp.tilts;
            suspensionIdx = DeriveSuspensionIdx();
        }

        ApplyToController();
        RefreshUI();
    }

    public string getDataFileName() {
        return dataFileName;
    }

    // Was missing IVehicleDependent, so 'controller' stayed null and OnButtonClicked threw.
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
        if (suspensionIdx < 2) suspensionIdx += 1;

        if (suspensionIdx == 0) newY = 0.2f;
        else if (suspensionIdx == 1) newY = 0.15f;
        else if (suspensionIdx == 2) newY = 0.1f;

        ApplyToController();
        RefreshUI();
    }

    void ApplyToController() {
        RCCP_CustomizationData custom = CustomizationData;

        if (custom == null)
            return;

        if (suspensionIdx == 2)
            custom.cambersFront = tilts;

        // Ride height is written by Suspension, not here: it layers its own trim on top of this
        // base and both scripts would otherwise fight over suspensionDistance in an undefined
        // order. Ask it to re-apply so the new base takes effect immediately.
        if (Suspension.instance != null) {
            Suspension.instance.ReapplySuspension();
            return;
        }

        // No fine-tuning tab in this scene, so apply the base directly. The original wrote
        // controller.transform.up.Set(..., newY, ...), a no-op: transform.up returns a Vector3
        // copy, so Set() mutated a temporary that was discarded.
        custom.suspensionDistanceFront = newY;
        custom.suspensionDistanceRear = newY;
    }

    /// <summary>
    /// Base ride height this upgrade level grants, which Suspension trims around.
    /// </summary>
    public float BaseRideHeight {
        get { return newY; }
    }

    int DeriveSuspensionIdx() {
        if (newY <= 0.1f) return 2;
        if (newY <= 0.15f) return 1;
        return 0;
    }

    // Called only when the value changes, never per frame.
    void RefreshUI() {
        if (typeText == null)
            return;

        if (suspensionIdx == 0) typeText.text = "Normal";
        else if (suspensionIdx == 1) typeText.text = "Low";
        else if (suspensionIdx == 2) typeText.text = "Drift";
    }

    RCCP_CustomizationData CustomizationData {
        get {
            if (controller == null || controller.Customizer == null || controller.Customizer.loadout == null)
                return null;

            return controller.Customizer.loadout.customizationData;
        }
    }
}
