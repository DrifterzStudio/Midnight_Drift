using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using static RCCP_Differential;

public class Others : RCCP_GenericComponent {
    private RCCP_CarController carController;

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
    [Tooltip("The telemetry.")]
    public GameObject telemetry;

    private float steerValue = .05f;
    private float throttleValue = .05f;
    private float brakeValue = .05f;
    private float handbrakeValue = .05f;
    private float clutchValue = .05f;

    private bool isTelemetry = false;

    private void Awake() {
        steerButton.onClick.AddListener(OnSteerButtonClicked);
        throttleButton.onClick.AddListener(OnThrottleButtonClicked);
        brakeButton.onClick.AddListener(OnBrakeButtonClicked);
        handbrakeButton.onClick.AddListener(OnHandbrakeButtonClicked);
        clutchButton.onClick.AddListener(OnClutchButtonClicked);
        telemetryButton.onClick.AddListener(OnTelemetryButtonClicked);
    }

    private void Update() {
        carController = RCCPSceneManager.activePlayerVehicle;

        if (isTelemetry) {
            telemetry.SetActive(true);
            telemetryText.text = "On";
        }
        else {
            telemetry.SetActive(false);
            telemetryText.text = "Off";
        }

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
