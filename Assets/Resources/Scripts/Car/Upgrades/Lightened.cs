using UnityEngine;

public class Lightened : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

    public RCCP_CarController controller;

    [Tooltip("Mass delta applied to the vehicle's stock mass, in kg. Negative lightens the car.")]
    public int lightenedMass = -50;

    private bool isLightened = false;

    public static Lightened instance;

    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            // 0 means "not bought". The previous version always wrote the constant, so a car
            // that never bought this upgrade still loaded as if it had.
            tmp.lightenedMass = isLightened ? lightenedMass : 0;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            isLightened = tmp.lightenedMass != 0;

            if (isLightened)
                lightenedMass = tmp.lightenedMass;
        }

        ApplyToController();
    }

    public string getDataFileName() {
        return dataFileName;
    }

    // Was missing IVehicleDependent, so 'controller' stayed null and OnButtonClicked threw.
    public void SetController(RCCP_CarController newController) {
        controller = newController;
        ApplyToController();
    }

    void Awake() {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);
    }

    public void OnButtonClicked() {
        if (isLightened)
            return;

        isLightened = true;
        ApplyToController();
    }

    /// <summary>
    /// The original assigned the delta straight to Rigid.mass, setting the car to -50 kg, which
    /// Unity rejects. It is a delta against the vehicle's stock mass.
    /// </summary>
    void ApplyToController() {
        if (!isLightened || controller == null || controller.Rigid == null)
            return;

        float stockMass = StockMass;

        if (stockMass > 0f)
            controller.Rigid.mass = stockMass + lightenedMass;
    }

    /// <summary>
    /// Read from the prefab rather than the live car: CarbonFiberBody writes the same field, so
    /// reading the instance would stack both deltas depending on which ran first. Rebasing each
    /// upgrade on the stock mass is what makes them replace each other.
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
