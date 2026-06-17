using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DrivingAid : MonoBehaviour {
    public RCCP_CarController carController;

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

    [Header("SH")]
    [Tooltip("The button that change sterring helper state.")]
    public Button SHButton;
    [Tooltip("Text showing the current state of the steering helper.")]
    public Text SHText;

    [Header("TH")]
    [Tooltip("The button that change traction helper state.")]
    public Button THButton;
    [Tooltip("Text showing the current state of the traction helper.")]
    public Text THText;

    [Header("ASP")]
    [Tooltip("The button that change arcade speed preservation state.")]
    public Button ASPButton;
    [Tooltip("Text showing the current state of the arcade speed preservation.")]
    public Text ASPText;

    private float ASPValue = 1f;

    private void Awake() {
        ABSButton.onClick.AddListener(OnABSButtonClicked);
        TCSButton.onClick.AddListener(OnTCSButtonClicked);
        ESPButton.onClick.AddListener(OnESPButtonClicked);
        SHButton.onClick.AddListener(OnSHButtonClicked);
        THButton.onClick.AddListener(OnTHButtonClicked);
        ASPButton.onClick.AddListener(OnASPButtonClicked);
    }

    private void Update() {

        if (carController.GetVehicleBehaviorType().ABS == true) ABSText.text = "On";
        else ABSText.text = "Off";

        if (carController.GetVehicleBehaviorType().TCS == true) TCSText.text = "On";
        else TCSText.text = "Off";

        if (carController.GetVehicleBehaviorType().ESP == true) ESPText.text = "On";
        else ESPText.text = "Off";

        if (carController.GetVehicleBehaviorType().steeringHelper == true) SHText.text = "On";
        else SHText.text = "Off";

        if (carController.GetVehicleBehaviorType().tractionHelper == true) THText.text = "On";
        else THText.text = "Off";

        ASPText.text = ASPValue.ToString();
    }
    private void OnABSButtonClicked() {
        carController.GetVehicleBehaviorType().ABS = !carController.GetVehicleBehaviorType().ABS;
        SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().ABS = !carController.GetVehicleBehaviorType().ABS;
    }
    private void OnTCSButtonClicked() {
        carController.GetVehicleBehaviorType().TCS = !carController.GetVehicleBehaviorType().TCS;
        SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().TCS = !carController.GetVehicleBehaviorType().TCS;
    }
    private void OnESPButtonClicked() {
        carController.GetVehicleBehaviorType().ESP = !carController.GetVehicleBehaviorType().ESP;
        SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().ESP = !carController.GetVehicleBehaviorType().ESP;
    }

    private void OnSHButtonClicked() {
        carController.GetVehicleBehaviorType().steeringHelper = !carController.GetVehicleBehaviorType().steeringHelper;
        SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().steeringHelper = !carController.GetVehicleBehaviorType().steeringHelper;
    }

    private void OnTHButtonClicked() {
        carController.GetVehicleBehaviorType().tractionHelper = !carController.GetVehicleBehaviorType().tractionHelper;
        SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().tractionHelper = !carController.GetVehicleBehaviorType().tractionHelper;
    }

    private void OnASPButtonClicked() {
        if (ASPValue + .2f > 1.1f) ASPValue = 0f;
        else ASPValue += .2f;
        carController.Stability.preserveSpeedFactor = ASPValue;
        SaveSetttings.vehiculeSettings.Stability.preserveSpeedFactor = ASPValue;
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
        if (SHButton != null) {
            SHButton.onClick.RemoveListener(OnSHButtonClicked);
        }
        if (THButton != null) {
            THButton.onClick.RemoveListener(OnTHButtonClicked);
        }
        if (ASPButton != null) {
            ASPButton.onClick.RemoveListener(OnASPButtonClicked);
        }
    }

}
