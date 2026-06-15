using UnityEngine;
using UnityEngine.UI;

public class Wheels : RCCP_GenericComponent {

    private RCCP_CarController controller;

    [Header("Camber")]
    [Tooltip("Camber button.")]
    public Button camberButton;

    [Tooltip("Camber.")]
    public GameObject camber;

    [Header("Grip")]
    [Tooltip("Grip button.")]
    public Button gripButton;

    [Tooltip("Grip.")]
    public GameObject grip;

    [Header("Steer")]
    [Tooltip("Steering sansibily button.")]
    public Button steeringSensitivityButton;

    [Tooltip("Steering curve button.")]
    public Button steeringCurveButton;

    [Tooltip("Text showing the current value of steering sensitivity.")]
    public Text sterringSensitivityText;

    private float sensitivityValue = 1f;



    private void Awake() {
        camberButton.onClick.AddListener(OnCamberButtonClicked);
        steeringSensitivityButton.onClick.AddListener(OnSteerSensitivityButtonClicked);
        steeringCurveButton.onClick.AddListener(OnSteerCurveButtonClicked);
        gripButton.onClick.AddListener(OnGripButtonClicked);
    }
    private void Update() {
        controller = RCCPSceneManager.activePlayerVehicle;

        sterringSensitivityText.text = "" + sensitivityValue;   

    }

    private void OnCamberButtonClicked() {
        gameObject.SetActive(false);
        camber.SetActive(true);
    }
    private void OnGripButtonClicked() {
        gameObject.SetActive(false);
        camber.SetActive(true);
    }

    private void OnSteerSensitivityButtonClicked() {
        if (sensitivityValue + 0.5f > 1f) sensitivityValue = 0f;
        else sensitivityValue += 0.5f;
        controller.GetVehicleBehaviorType().steeringSensitivity = sensitivityValue;
    } 

    private void OnSteerCurveButtonClicked() { } // controller.GetVehiculeType().sterringCurve

    private void OnDestroy() {
        if (camberButton != null) {
            camberButton.onClick.RemoveListener(OnCamberButtonClicked);
        }
        if (gripButton != null) {
            gripButton.onClick.RemoveListener(OnGripButtonClicked);
        }
        if (steeringSensitivityButton != null) {
            steeringSensitivityButton.onClick.RemoveListener(OnSteerSensitivityButtonClicked);
        }
        if (steeringCurveButton != null) {
            steeringCurveButton.onClick.RemoveListener(OnSteerCurveButtonClicked);
        }
    }
}
