using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class Others : MonoBehaviour, IDataPersistence {

    public DataPersistenceManager dataPersistence;

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




    private void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);

        steerButton.onClick.AddListener(OnSteerButtonClicked);
        throttleButton.onClick.AddListener(OnThrottleButtonClicked);
        brakeButton.onClick.AddListener(OnBrakeButtonClicked);
        handbrakeButton.onClick.AddListener(OnHandbrakeButtonClicked);
        clutchButton.onClick.AddListener(OnClutchButtonClicked);
        telemetryButton.onClick.AddListener(OnTelemetryButtonClicked);
    }

    private void Update() {
        instance = this;

        if (isTelemetry) telemetryText.text = "On";
        else telemetryText.text = "Off";

        steerText.text = steerValue.ToString();
        throttleText.text = throttleValue.ToString();
        brakeText.text = brakeValue.ToString();
        handbrakeText.text = handbrakeValue.ToString();
        clutchText.text = clutchValue.ToString();
    }

    private void OnSteerButtonClicked() {
        if (steerValue + .02f > .2f) steerValue = 0f;
        else steerValue += .02f;
        carController.Inputs.steeringDeadzone = steerValue;
    }

    private void OnThrottleButtonClicked() {
        if (throttleValue + .02f > .2f) throttleValue = 0f;
        else throttleValue += .02f;
        carController.Inputs.throttleDeadzone = throttleValue;
    }

    private void OnBrakeButtonClicked() {
        if (brakeValue + .02f > .2f) brakeValue = 0f;
        else brakeValue += .02f;
        carController.Inputs.brakeDeadzone = brakeValue;
    }

    private void OnHandbrakeButtonClicked() {
        if (handbrakeValue + .02f > .2f) handbrakeValue = 0f;
        else handbrakeValue += .02f;
        carController.Inputs.handbrakeDeadzone = handbrakeValue;
    }

    private void OnClutchButtonClicked() {
        if (clutchValue + .02f > .2f) clutchValue = 0f;
        else clutchValue += .02f;
        carController.Inputs.clutchDeadzone = clutchValue;
    }

    private void OnTelemetryButtonClicked() {
        isTelemetry = !isTelemetry;
    }

    private void OnDestroy() {
        if (steerButton != null) steerButton.onClick.RemoveAllListeners();
        if (throttleButton != null) throttleButton.onClick.RemoveAllListeners();
        if (brakeButton != null) brakeButton.onClick.RemoveAllListeners();
        if (handbrakeButton != null) handbrakeButton.onClick.RemoveAllListeners();
        if (clutchButton != null) clutchButton.onClick.RemoveAllListeners();
        if (telemetryButton != null) telemetryButton.onClick.RemoveAllListeners();
    }
}
