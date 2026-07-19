using UnityEngine;
using UnityEngine.UI;

public class PropulsionType : MonoBehaviour, IDataPersistence, IVehicleDependent
{

    public string dataFileName;

    public RCCP_CarController controller;

    [Header("Drive Wheels Type")]
    [Tooltip("DWT Button.")]
    public Button DWTButton;
    [Tooltip("DWT Text.")]
    public Text DWTText;

    private int driveType = 1;
    public bool fIsSteer = true;
    public bool fIsHandbrake = false;
    public bool rIsSteer = false;
    public bool rIsHandbrake = true;

    public static PropulsionType instance;

    public void LoadGame(IGameData data)
    {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null)
        {
            fIsSteer = tmp.frontAxleSteer;
            fIsHandbrake = tmp.frontAxleHandbrake;
            rIsSteer = tmp.rearAxleSteer;
            rIsHandbrake = tmp.rearAxleHandbrake;

            // driveType isn't saved, only the axle flags it makes, so rebuild it from them or the
            // label drifts out of sync with the loaded car.
            driveType = DeriveDriveType();
        }

        ApplyToController();
        RefreshUI();
    }

    public void SaveGame(IGameData data)
    {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null)
        {
            tmp.frontAxleSteer = fIsSteer;
            tmp.frontAxleHandbrake = fIsHandbrake;
            tmp.rearAxleSteer = rIsSteer;
            tmp.rearAxleHandbrake = rIsHandbrake;
        }
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

    private void Awake()
    {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);

        if (DWTButton != null)
            DWTButton.onClick.AddListener(OnDWTButtonClicked);
    }

    private void Start()
    {
        RefreshUI();
    }

    private void OnDWTButtonClicked()
    {
        if (driveType + 1 > 2) driveType = 0;
        else driveType += 1;

        SetFlagsFromDriveType();
        ApplyToController();
        RefreshUI();
    }

    void SetFlagsFromDriveType()
    {
        switch (driveType)
        {
            case 0:     // Front wheels drive
                fIsSteer = true; fIsHandbrake = true;
                rIsSteer = false; rIsHandbrake = false;
                break;
            case 1:     // Rear wheels drive
                fIsSteer = true; fIsHandbrake = false;
                rIsSteer = false; rIsHandbrake = true;
                break;
            default:    // All wheels drive
                fIsSteer = true; fIsHandbrake = true;
                rIsSteer = true; rIsHandbrake = true;
                break;
        }
    }

    int DeriveDriveType()
    {
        if (rIsSteer && rIsHandbrake) return 2;
        if (fIsHandbrake) return 0;
        return 1;
    }

    // Braking's handbrake toggle goes through here so this script stays the only owner of the axle
    // handbrake flags.
    public void SetRearHandbrake(bool enabled)
    {
        rIsHandbrake = enabled;
        ApplyToController();
        RefreshUI();
    }

    // only runs on a real change, so it doesn't fight other systems touching the axles
    void ApplyToController()
    {
        if (controller == null)
            return;

        if (controller.FrontAxle != null)
        {
            controller.FrontAxle.isSteer = fIsSteer;
            controller.FrontAxle.isHandbrake = fIsHandbrake;
        }

        if (controller.RearAxle != null)
        {
            controller.RearAxle.isSteer = rIsSteer;
            controller.RearAxle.isHandbrake = rIsHandbrake;
        }
    }

    void RefreshUI()
    {
        if (DWTText == null)
            return;

        switch (driveType)
        {
            case 0: DWTText.text = "Front wheels drive"; break;
            case 1: DWTText.text = "Rear wheels drive"; break;
            default: DWTText.text = "All wheels drive"; break;
        }
    }

    private void OnDestroy()
    {
        if (DWTButton != null)
            DWTButton.onClick.RemoveListener(OnDWTButtonClicked);
    }
}
