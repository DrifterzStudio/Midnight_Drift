using UnityEngine;
using UnityEngine.UI;

public class Others : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

    public RCCP_CarController carController;

    [Header("Deadzone")]
    [Tooltip("The button that change the value of the steering deadzone.")]
    public Button steerButton;
    [Tooltip("Text showing the current value of the steering deadzone.")]
    public Text steerText;

    [Tooltip("The button that change the value of the throttle deadzone.")]
    public Button throttleButton;
    [Tooltip("Text showing the current value of the throttle deadzone.")]
    public Text throttleText;

    [Tooltip("The button that change the value of the brake deadzone.")]
    public Button brakeButton;
    [Tooltip("Text showing the current value of the brake deadzone.")]
    public Text brakeText;

    [Tooltip("The button that change the value of the handbrake deadzone.")]
    public Button handbrakeButton;
    [Tooltip("Text showing the current value of the handbrake deadzone.")]
    public Text handbrakeText;

    [Tooltip("The button that change the value of the clutch deadzone.")]
    public Button clutchButton;
    [Tooltip("Text showing the current value of the clutch deadzone.")]
    public Text clutchText;

    [Header("Telemetry")]
    [Tooltip("Button that switch the state of the telemetry.")]
    public Button telemetryButton;
    [Tooltip("Text showing the current state of the telemetry.")]
    public Text telemetryText;

    public float steerValue = .05f;
    public float throttleValue = .05f;
    public float brakeValue = .05f;
    public float handbrakeValue = .05f;
    public float clutchValue = .05f;

    public bool isTelemetry = false;

    public static Others instance;

    public void LoadGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            steerValue = tmp.steerValue;
            throttleValue = tmp.throttleValue;
            brakeValue = tmp.brakeValue;
            handbrakeValue = tmp.handbrakeValue;
            clutchValue = tmp.clutchValue;
            isTelemetry = tmp.isTelemetry;
        }

        ApplyToController();
        RefreshUI();
    }

    public void SaveGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            tmp.steerValue = steerValue;
            tmp.throttleValue = throttleValue;
            tmp.brakeValue = brakeValue;
            tmp.handbrakeValue = handbrakeValue;
            tmp.clutchValue = clutchValue;
            tmp.isTelemetry = isTelemetry;
        }
    }

    public string getDataFileName() {
        return dataFileName;
    }

    // Was missing IVehicleDependent, so 'carController' stayed null and every click threw.
    public void SetController(RCCP_CarController newController) {
        carController = newController;
        ApplyToController();
        RefreshUI();
    }

    private void Awake() {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);

        if (steerButton != null) steerButton.onClick.AddListener(OnSteerButtonClicked);
        if (throttleButton != null) throttleButton.onClick.AddListener(OnThrottleButtonClicked);
        if (brakeButton != null) brakeButton.onClick.AddListener(OnBrakeButtonClicked);
        if (handbrakeButton != null) handbrakeButton.onClick.AddListener(OnHandbrakeButtonClicked);
        if (clutchButton != null) clutchButton.onClick.AddListener(OnClutchButtonClicked);
        if (telemetryButton != null) telemetryButton.onClick.AddListener(OnTelemetryButtonClicked);
    }

    private void Start() {
        RefreshUI();
    }

    private void OnSteerButtonClicked() {
        if (steerValue + .02f > .2f) steerValue = 0f;
        else steerValue += .02f;

        ApplyToController();
        RefreshUI();
    }

    private void OnThrottleButtonClicked() {
        if (throttleValue + .02f > .2f) throttleValue = 0f;
        else throttleValue += .02f;

        ApplyToController();
        RefreshUI();
    }

    private void OnBrakeButtonClicked() {
        if (brakeValue + .02f > .2f) brakeValue = 0f;
        else brakeValue += .02f;

        ApplyToController();
        RefreshUI();
    }

    private void OnHandbrakeButtonClicked() {
        if (handbrakeValue + .02f > .2f) handbrakeValue = 0f;
        else handbrakeValue += .02f;

        ApplyToController();
        RefreshUI();
    }

    private void OnClutchButtonClicked() {
        if (clutchValue + .02f > .2f) clutchValue = 0f;
        else clutchValue += .02f;

        ApplyToController();
        RefreshUI();
    }

    private void OnTelemetryButtonClicked() {
        isTelemetry = !isTelemetry;
        RefreshUI();
    }

    void ApplyToController() {
        if (carController == null || carController.Inputs == null)
            return;

        carController.Inputs.steeringDeadzone = steerValue;
        carController.Inputs.throttleDeadzone = throttleValue;
        carController.Inputs.brakeDeadzone = brakeValue;
        carController.Inputs.handbrakeDeadzone = handbrakeValue;
        carController.Inputs.clutchDeadzone = clutchValue;
    }

    // Called only when a value changes, never per frame.
    void RefreshUI() {
        if (telemetryText != null) telemetryText.text = isTelemetry ? "On" : "Off";

        if (steerText != null) steerText.text = steerValue.ToString();
        if (throttleText != null) throttleText.text = throttleValue.ToString();
        if (brakeText != null) brakeText.text = brakeValue.ToString();
        if (handbrakeText != null) handbrakeText.text = handbrakeValue.ToString();
        if (clutchText != null) clutchText.text = clutchValue.ToString();
    }

    private void OnDestroy() {
        // RemoveAllListeners() also wiped anything wired in the Inspector on these buttons;
        // unsubscribe only what this script added.
        if (steerButton != null) steerButton.onClick.RemoveListener(OnSteerButtonClicked);
        if (throttleButton != null) throttleButton.onClick.RemoveListener(OnThrottleButtonClicked);
        if (brakeButton != null) brakeButton.onClick.RemoveListener(OnBrakeButtonClicked);
        if (handbrakeButton != null) handbrakeButton.onClick.RemoveListener(OnHandbrakeButtonClicked);
        if (clutchButton != null) clutchButton.onClick.RemoveListener(OnClutchButtonClicked);
        if (telemetryButton != null) telemetryButton.onClick.RemoveListener(OnTelemetryButtonClicked);
    }
}
