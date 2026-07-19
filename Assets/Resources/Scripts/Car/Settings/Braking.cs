using UnityEngine;
using UnityEngine.UI;

public class Braking : MonoBehaviour, IDataPersistence, IVehicleDependent
{

    public string dataFileName;

    public RCCP_CarController controller;

    [Tooltip("Button that switch the state of the handbrake.")]
    public Button handbrakeButton;

    [Tooltip("Text showing the state of the handbrake.")]
    public Text handbrakeText;

    [Tooltip("Button that change the valiue of the handbrake multiplier.")]
    public Button handbrakeMultiplierButton;

    [Tooltip("Text showing the value of the handbrake multiplier.")]
    public Text handbrakeMultiplierText;

    [Tooltip("Button that change the valiue of the brake multiplier.")]
    public Button brakeMultiplierButton;

    [Tooltip("Text showing the value of the brake multiplier.")]
    public Text brakeMultiplierText;

    public bool isHandbrake = true;
    public float handbrakeMultiplier = .6f;
    public float brakeMultiplier = 1f;

    public static Braking instance;

    public void LoadGame(IGameData data)
    {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null)
        {
            isHandbrake = tmp.isHandbrake;
            handbrakeMultiplier = tmp.handbrakeMultiplier;
            brakeMultiplier = tmp.brakeMultiplier;
        }

        ApplyToController();
        RefreshUI();
    }

    public void SaveGame(IGameData data)
    {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null)
        {
            tmp.isHandbrake = isHandbrake;
            tmp.handbrakeMultiplier = handbrakeMultiplier;
            tmp.brakeMultiplier = brakeMultiplier;
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

        if (handbrakeButton != null) handbrakeButton.onClick.AddListener(OnHandbrakeButtonClicked);
        if (handbrakeMultiplierButton != null) handbrakeMultiplierButton.onClick.AddListener(OnHandbrakeMultiplierButtonClicked);
        if (brakeMultiplierButton != null) brakeMultiplierButton.onClick.AddListener(OnBrakeMultiplierButtonClicked);
    }

    private void Start()
    {
        RefreshUI();
    }

    private void OnHandbrakeButtonClicked()
    {
        isHandbrake = !isHandbrake;

        // goes to the axle through PropulsionType, the only owner of the handbrake flags
        if (PropulsionType.instance != null)
            PropulsionType.instance.SetRearHandbrake(isHandbrake);

        RefreshUI();
    }

    private void OnHandbrakeMultiplierButtonClicked()
    {
        if (handbrakeMultiplier + .1f > 1f) handbrakeMultiplier = 0f;
        else handbrakeMultiplier += .1f;

        ApplyToController();
        RefreshUI();
    }

    private void OnBrakeMultiplierButtonClicked()
    {
        if (brakeMultiplier + .1f > 1f) brakeMultiplier = 0f;
        else brakeMultiplier += .1f;

        ApplyToController();
        RefreshUI();
    }

    void ApplyToController()
    {
        if (controller == null)
            return;

        // isHandbrake isn't written here - PropulsionType owns the axle handbrake flags
        if (controller.RearAxle != null)
        {
            controller.RearAxle.handbrakeMultiplier = handbrakeMultiplier;
            controller.RearAxle.brakeMultiplier = brakeMultiplier;
        }

        // handbrakeMultiplier goes on both axles, matching LoadCarModification.
        if (controller.FrontAxle != null)
            controller.FrontAxle.handbrakeMultiplier = handbrakeMultiplier;
    }

    void RefreshUI()
    {
        if (handbrakeText != null) handbrakeText.text = isHandbrake ? "On" : "Off";
        if (handbrakeMultiplierText != null) handbrakeMultiplierText.text = handbrakeMultiplier.ToString();
        if (brakeMultiplierText != null) brakeMultiplierText.text = brakeMultiplier.ToString();
    }

    private void OnDestroy()
    {
        if (handbrakeButton != null) handbrakeButton.onClick.RemoveListener(OnHandbrakeButtonClicked);
        if (handbrakeMultiplierButton != null) handbrakeMultiplierButton.onClick.RemoveListener(OnHandbrakeMultiplierButtonClicked);
        if (brakeMultiplierButton != null) brakeMultiplierButton.onClick.RemoveListener(OnBrakeMultiplierButtonClicked);
    }
}
