using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DrivingAid : RCCP_GenericComponent {
    private RCCP_CarController carController;

    [Header("ABS")]
    [Tooltip("The button that change ABS state.")]
    public Button ABSButton;
    [Tooltip("Text showing the current state of the ABS.")]
    public Text ABSText;

    [Header("TCS")]
    [Tooltip("The button that change TCS state.")]
    public Button TCSButton;
    [Tooltip("Text showing the current state of the TCS.")]
    public Text TCSText;

    [Header("ESP")]
    [Tooltip("The button that change ESP state.")]
    public Button ESPButton;
    [Tooltip("Text showing the current state of the ESP.")]
    public Text ESPText;

    private void Awake() {
        ABSButton.onClick.AddListener(OnABSButtonClicked);
        TCSButton.onClick.AddListener(OnTCSButtonClicked);
        ESPButton.onClick.AddListener(OnESPButtonClicked);
    }

    private void Update() {
        carController = RCCPSceneManager.activePlayerVehicle;

        ABSText.text = "ABS: ";
        if (carController.GetVehicleBehaviorType().ABS == true) ABSText.text += "On";
        else ABSText.text += "Off";

        TCSText.text = "TCS: ";
        if (carController.GetVehicleBehaviorType().TCS == true) TCSText.text += "On";
        else TCSText.text += "Off";

        ESPText.text = "ESP: ";
        if (carController.GetVehicleBehaviorType().ESP == true) ESPText.text += "On";
        else ESPText.text += "Off";
    }
    private void OnABSButtonClicked() {
        carController.GetVehicleBehaviorType().ABS = !carController.GetVehicleBehaviorType().ABS;
    }
    private void OnTCSButtonClicked() {
        carController.GetVehicleBehaviorType().TCS = !carController.GetVehicleBehaviorType().TCS;
    }
    private void OnESPButtonClicked() {
        carController.GetVehicleBehaviorType().ESP = !carController.GetVehicleBehaviorType().ESP;
    }

    private void OnDestroy() {
        if (ABSButton != null) {
            ABSButton.onClick.RemoveListener(OnABSButtonClicked);
        }
        if (TCSButton != null) {
            TCSButton.onClick.RemoveListener(OnTCSButtonClicked);
        }
        if (ESPButton != null) {
            ESPButton.onClick.RemoveListener(OnESPButtonClicked);
        }
    }

}
