using UnityEngine;

public class CarbonFiberBody : MonoBehaviour, IDataPersistence, IVehicleDependent
{

    public string dataFileName;

    public RCCP_CarController controller;

    [Tooltip("Mass delta applied to the vehicle's stock mass, in kg. Negative lightens the car.")]
    public int carbonMass = -200;

    private bool isCarbon = false;

    // read by Lightened so both can rebase the shared mass together
    public bool IsCarbon => isCarbon;

    public static CarbonFiberBody instance;

    public void SaveGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
        {
            // 0 = not bought, so an unowned upgrade isn't re-applied on load
            tmp.carbonMass = isCarbon ? carbonMass : 0;
        }
    }

    public void LoadGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
        {
            isCarbon = tmp.carbonMass != 0;

            if (isCarbon)
                carbonMass = tmp.carbonMass;
        }

        ApplyToController();
    }

    public string getDataFileName()
    {
        return dataFileName;
    }

    public void SetController(RCCP_CarController newController)
    {
        controller = newController;
        ApplyToController();
    }

    void Awake()
    {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);
    }

    public void OnButtonClicked()
    {
        // toggle on/off so you can cancel it
        isCarbon = !isCarbon;
        ApplyToController();
    }

    // carbonMass is a delta from the stock mass, not an absolute value
    void ApplyToController()
    {
        if (controller == null || controller.Rigid == null)
            return;

        float stockMass = StockMass;

        if (stockMass > 0f)
            controller.Rigid.mass = stockMass + CombinedMassDelta();
    }

    // carbon replaces lightened when both are on. recompute from stock so toggling either one works
    int CombinedMassDelta()
    {
        if (isCarbon)
            return carbonMass;

        if (Lightened.instance != null && Lightened.instance.IsLightened)
            return Lightened.instance.lightenedMass;

        return 0;
    }

    // read from the prefab, not the live car: Lightened writes the same field, so reading the
    // instance could stack both deltas. rebasing on the stock mass is what makes them replace each other
    float StockMass
    {
        get
        {
            if (GameSession.SelectedVehicle == null || GameSession.SelectedVehicle.prefab == null)
                return 0f;

            Rigidbody prefabRigid = GameSession.SelectedVehicle.prefab.GetComponent<Rigidbody>();

            return prefabRigid != null ? prefabRigid.mass : 0f;
        }
    }
}
