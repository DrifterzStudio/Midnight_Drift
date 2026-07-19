using UnityEngine;
using UnityEngine.UI;

public class SuspensionUpgrade : MonoBehaviour, IDataPersistence, IVehicleDependent
{

    public string dataFileName;

    public RCCP_CarController controller;

    public Text typeText;

    public float newY = 0.16f;

    public float tilts = 5f;

    private int suspensionIdx = 0;

    public static SuspensionUpgrade instance;

    public void SaveGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
        {
            tmp.newY = newY;
            tmp.tilts = tilts;
        }
    }

    public void LoadGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null && tmp.newY > 0f)
        {
            newY = tmp.newY;
            tilts = tmp.tilts;
        }
        else
        {
            // no saved upgrade, stock ride height
            newY = 0.2f;
        }
        suspensionIdx = DeriveSuspensionIdx();

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

    private void Start()
    {
        RefreshUI();
    }

    public void OnButtonClicked()
    {
        // cycle Normal -> Low -> Drift -> Normal, past Drift goes back to stock
        suspensionIdx = (suspensionIdx + 1) % 3;

        if (suspensionIdx == 0) newY = 0.2f;
        else if (suspensionIdx == 1) newY = 0.15f;
        else newY = 0.1f;

        ApplyToController();
        RefreshUI();
    }

    void ApplyToController()
    {
        RCCP_CustomizationData custom = CustomizationData;

        if (custom == null)
            return;

        // camber only at Drift, clear it otherwise
        custom.cambersFront = (suspensionIdx == 2) ? tilts : 0f;

        // Suspension writes the ride height, not us - it layers its own trim on this base. ask it
        // to re-apply so the new base kicks in right away.
        if (Suspension.instance != null)
        {
            Suspension.instance.ReapplySuspension();
            return;
        }

        // no fine-tuning tab in this scene, so apply the base directly
        custom.suspensionDistanceFront = newY;
        custom.suspensionDistanceRear = newY;
    }

    // base ride height for this level, which Suspension trims around
    public float BaseRideHeight
    {
        get { return newY; }
    }

    int DeriveSuspensionIdx()
    {
        if (newY <= 0.1f) return 2;
        if (newY <= 0.15f) return 1;
        return 0;
    }

    void RefreshUI()
    {
        if (typeText == null)
            return;

        if (suspensionIdx == 0) typeText.text = "Normal";
        else if (suspensionIdx == 1) typeText.text = "Low";
        else if (suspensionIdx == 2) typeText.text = "Drift";
    }

    RCCP_CustomizationData CustomizationData
    {
        get
        {
            if (controller == null || controller.Customizer == null || controller.Customizer.loadout == null)
                return null;

            return controller.Customizer.loadout.customizationData;
        }
    }
}
