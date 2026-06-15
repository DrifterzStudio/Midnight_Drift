using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UI;
using static RCCP_Gearbox;
using static RCCP_Gearbox.CurrentGearState;

public class Gearbox : RCCP_GenericComponent {
    private RCCP_CarController carController;

    [Header("Gearbox")]
    [Tooltip("Auto reverse button.")]
    public Button autoReverseButton;

    [Tooltip("Text showing the current state of auto reverse.")]
    public Text autoReverseText;

    [Tooltip("Gearbox type button.")]
    public Button gearboxButton;

    [Tooltip("Text showing the current Type of gearbox.")]
    public Text gearboxTypeText;

    [Tooltip("Other parameters.")]
    public GameObject otherParam;

    [Tooltip("Gear shifting threshold button.")]
    public Button GSTButton;

    [Tooltip("Text showing the current value of the gear shifting threshold.")]
    public Text GSTText;

    [Tooltip("Shifting delay button.")]
    public Button shiftingDelayButton;

    [Tooltip("Text showing the current value of the shifting delay.")]
    public Text shiftingDelayText;

    [Tooltip("Anti roll force button.")]
    public Button ARFButton;

    [Tooltip("Text showing the current value of the anti roll force.")]
    public Text ARFText;

    private bool isReverse = false;
    private int gearboxType = 0;
    private float GSTValue = .7f;
    private float shiftingDelay = .2f;
    private int ARFValue = 1000;

    private void Awake() {
        autoReverseButton.onClick.AddListener(OnAutoReverseButtonClicked);
        gearboxButton.onClick.AddListener(OnGearboxButtonClicked);
        GSTButton.onClick.AddListener(OnGSTButtonClicked);
        shiftingDelayButton.onClick.AddListener(OnShiftingDelayButtonClicked);
        ARFButton.onClick.AddListener(OnARFButtonClicked);
    }

    private void Update() {
        carController = RCCPSceneManager.activePlayerVehicle;

        if (isReverse) carController.Gearbox.currentGearState.gearState = GearState.InReverseGear;
        else carController.Gearbox.currentGearState.gearState = GearState.InForwardGear;

        if (gearboxType == 0) {
            carController.Gearbox.transmissionType = TransmissionType.Automatic;
            otherParam.SetActive(true);
            gearboxTypeText.text = "Automatic";
            GSTText.text = GSTValue.ToString();
            shiftingDelayText.text = shiftingDelay.ToString();
        }
        else if (gearboxType == 1) {
            carController.Gearbox.transmissionType = TransmissionType.Manual;
            otherParam.SetActive(false);
            gearboxTypeText.text = "Manual";
        }
        else {
            carController.Gearbox.transmissionType = TransmissionType.Automatic_DNRP;
            otherParam.SetActive(false);
            gearboxTypeText.text = "Automatic DNRP";
        }

        if (isReverse) autoReverseText.text = "On";
        else autoReverseText.text = "Off";

        ARFText.text = ARFValue.ToString();
    }

    private void OnAutoReverseButtonClicked() {
        isReverse = !isReverse;
    }

    private void OnGearboxButtonClicked() {
        if (gearboxType + 1 > 2) gearboxType = 0;
        else gearboxType += 1;
    }

    private void OnGSTButtonClicked() {
        if (otherParam.activeSelf) {
            if (GSTValue + .1f > .9f) GSTValue = .1f;
            else GSTValue += .1f;
            carController.Gearbox.shiftThreshold = GSTValue;
        }
    }

    private void OnShiftingDelayButtonClicked() {
        if (otherParam.activeSelf) {
            if (shiftingDelay + .1f > .5f) GSTValue = .2f;
            else GSTValue += .1f;
            carController.Gearbox.shiftingTime = shiftingDelay;
        }
    }
    private void OnARFButtonClicked() {
        if (ARFValue + 500 > 1500) ARFValue = 500;
        else ARFValue += 500;
        carController.FrontAxle.antirollForce = ARFValue;
        carController.RearAxle.antirollForce = ARFValue;
    }

    private void OnDestroy() {
        if (autoReverseButton != null) autoReverseButton.onClick.RemoveListener(OnAutoReverseButtonClicked);
        if (gearboxButton != null) gearboxButton.onClick.RemoveListener(OnGearboxButtonClicked);
        if (GSTButton != null) GSTButton.onClick.RemoveListener(OnGSTButtonClicked);
        if (shiftingDelayButton != null) shiftingDelayButton.onClick.RemoveListener(OnShiftingDelayButtonClicked);
        if (ARFButton != null) ARFButton.onClick.RemoveListener(OnARFButtonClicked);
    }
}
