using UnityEngine;

public class CarbonFiberBody : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

    public RCCP_CarController controller;

    [Tooltip("Mass delta applied to the vehicle's stock mass, in kg. Negative lightens the car.")]
    public int carbonMass = -200;

    private bool isCarbon = false;

    public static CarbonFiberBody instance;

    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            // 0 means "not bought", so an unowned upgrade doesn't get re-applied on load.
            tmp.carbonMass = isCarbon ? carbonMass : 0;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            isCarbon = tmp.carbonMass != 0;

            if (isCarbon)
                carbonMass = tmp.carbonMass;
        }

        ApplyToController();
    }

    public string getDataFileName() {
        return dataFileName;
    }

    public void SetController(RCCP_CarController newController) {
        controller = newController;
        ApplyToController();
    }

    void Awake() {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);
    }

    public void OnButtonClicked() {
        if (isCarbon)
            return;

        isCarbon = true;
        ApplyToController();
    }

    // carbonMass is a delta against the stock mass, not an absolute value.
    void ApplyToController() {
        if (!isCarbon || controller == null || controller.Rigid == null)
            return;

        float stockMass = StockMass;

        if (stockMass > 0f)
            controller.Rigid.mass = stockMass + carbonMass;
    }

    /// <summary>
    /// Read from the prefab rather than the live car: Lightened writes the same field, so reading
    /// the instance would stack both deltas depending on which ran first. Rebasing each upgrade
    /// on the stock mass is what makes them replace each other.
    /// </summary>
    float StockMass {
        get {
            if (GameSession.SelectedVehicle == null || GameSession.SelectedVehicle.prefab == null)
                return 0f;

            Rigidbody prefabRigid = GameSession.SelectedVehicle.prefab.GetComponent<Rigidbody>();

            return prefabRigid != null ? prefabRigid.mass : 0f;
        }
    }
}
