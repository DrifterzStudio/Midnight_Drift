using UnityEngine;
using UnityEngine.UI;

public class Slick : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

    public RCCP_CarController controller;

    public float slickness = 10f;

    public Text slick;

    public static Slick instance;

    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            tmp.slickness = slickness;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            slickness = tmp.slickness;
        }

        ApplyToController();
        RefreshUI();
    }

    public string getDataFileName() {
        return dataFileName;
    }

    // spawned at runtime and can't be dragged in from the Inspector.
    public void SetController(RCCP_CarController newController) {
        controller = newController;
        ApplyToController();
        RefreshUI();
    }

    private void Awake() {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);
    }

    private void Start() {
        RefreshUI();
    }

    public void OnButtonClicked() {
        if (slickness == 10) slickness = 5f;
        else if (slickness == 5f) slickness = 1f;

        ApplyToController();
        RefreshUI();
    }

    void ApplyToController() {
        if (controller == null || controller.AeroDynamics == null)
            return;

        controller.AeroDynamics.wheelResistance = slickness;
    }

    void RefreshUI() {
        if (slick == null)
            return;

        if (slickness == 10) slick.text = "Normal";
        else if (slickness == 5) slick.text = "Semi-Slick";
        else if (slickness == 1) slick.text = "Slick";
    }
}
