using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using static RCCP_Gearbox;
using static RCCP_Gearbox.CurrentGearState;

public class Gearbox : MonoBehaviour, IDataPersistence {

    public RCCP_CarController carController;

    [Header("Gearbox")]
    [Tooltip("Auto reverse button.")]
    public Button autoReverseButton;

    [Tooltip("Text showing the current state of auto reverse.")]
    public Text autoReverseText;

    [Tooltip("Gearbox type button.")]
    public Button gearboxButton;

    [Tooltip("Text showing the current Type of gearbox.")]
    public Text gearboxTypeText;

    [Header("Parameters for Automatic gearbox")]
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

    [Header("Cut throttle when shifting")]
    [Tooltip("CTWS force button.")]
    public Button CTWSButton;

    [Tooltip("Text showing the current state of the CTWS.")]
    public Text CTWSText;

    [Header("Clutch Threshold")]
    [Tooltip("Clutch threshold force button.")]
    public Button clutchThresholdButton;

    [Tooltip("Text showing the current state of the clutch threshold.")]
    public Text clutchThresholdText;

    [Header("Anti roll force")]
    [Tooltip("Anti roll force button.")]
    public Button ARFButton;

    [Tooltip("Text showing the current value of the anti roll force.")]
    public Text ARFText;

    
    private bool isReverse = false;
    public GearState gearState = GearState.InForwardGear;
    private int gearboxType = 1;
    public TransmissionType transmissionType = TransmissionType.Automatic;
    public float GSTValue = .7f;
    public float shiftingDelay = .2f;
    public bool CTWS = true;
    public float clutchThreshold = .1f;
    public int ARFValue = 1000;

    public static Gearbox instance;


    public void LoadGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            gearState = tmp.isReverse;
            transmissionType = tmp.transmissionType;
            GSTValue = tmp.GSTValue;
            shiftingDelay = tmp.shiftingDelay;
            clutchThreshold = tmp.clutchThreshold;
            ARFValue = tmp.ARFValue;
        }
    }

    public void SaveGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            tmp.isReverse = carController.Gearbox.currentGearState.gearState;
            tmp.transmissionType = carController.Gearbox.transmissionType;
            tmp.GSTValue = GSTValue;
            tmp.shiftingDelay = shiftingDelay;
            tmp.clutchThreshold = clutchThreshold;
            tmp.ARFValue = ARFValue;
        }
    }

    public string getDataFileName() {
        return "";
    }




    private void Awake() {
        if (instance == null) instance = this;

        autoReverseButton.onClick.AddListener(OnAutoReverseButtonClicked);
        gearboxButton.onClick.AddListener(OnGearboxButtonClicked);
        GSTButton.onClick.AddListener(OnGSTButtonClicked);
        shiftingDelayButton.onClick.AddListener(OnShiftingDelayButtonClicked);
        CTWSButton.onClick.AddListener(OnCTWSButtonClicked);
        clutchThresholdButton.onClick.AddListener(OnClutchThresholdButtonClicked);
        ARFButton.onClick.AddListener(OnARFButtonClicked);
    }

    private void Update() {
        instance = this;

        if (isReverse) autoReverseText.text = "On";
        else autoReverseText.text = "Off";

        if (gearboxType == 0) {
            otherParam.SetActive(false);
            gearboxTypeText.text = "Manual";
        }
        else if (gearboxType == 1) {
            otherParam.SetActive(true);
            gearboxTypeText.text = "Automatic";
            GSTText.text = GSTValue.ToString();
            shiftingDelayText.text = shiftingDelay.ToString();
        }
        else {
            otherParam.SetActive(false);
            gearboxTypeText.text = "Automatic DNRP";
        }

        if (carController.Inputs.cutThrottleWhenShifting) CTWSText.text = "On";
        else CTWSText.text = "Off";

        clutchThresholdText.text = clutchThreshold.ToString();

        ARFText.text = ARFValue.ToString();

    }

    private void OnAutoReverseButtonClicked() {
        isReverse = !isReverse;
        if (isReverse) {
            carController.Gearbox.currentGearState.gearState = GearState.InReverseGear;
            gearState = GearState.InReverseGear;
        }
        else {
            carController.Gearbox.currentGearState.gearState = GearState.InForwardGear;
            gearState = GearState.InForwardGear;
        }
    }

    private void OnGearboxButtonClicked() {
        if (gearboxType + 1 > 2) gearboxType = 0;
        else gearboxType += 1;

        if (gearboxType == 0) {
            carController.Gearbox.transmissionType = TransmissionType.Manual;
            transmissionType = TransmissionType.Manual;
        }
        else if (gearboxType == 1) {
            carController.Gearbox.transmissionType = TransmissionType.Automatic;
            transmissionType = TransmissionType.Automatic;
        }
        else {
            carController.Gearbox.transmissionType = TransmissionType.Automatic_DNRP;
            transmissionType = TransmissionType.Automatic_DNRP;
        }

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
            carController.Gearbox.shiftingTime = shiftingDelay;;
        }
    }

    private void OnCTWSButtonClicked() {
        carController.Inputs.cutThrottleWhenShifting = !carController.Inputs.cutThrottleWhenShifting;
        CTWS = carController.Inputs.cutThrottleWhenShifting; ;
    }

    private void OnClutchThresholdButtonClicked() {
        if (clutchThreshold + .2f > 1f) clutchThreshold = 0f;
        else clutchThreshold += .2f;
        carController.Customizer.loadout.customizationData.clutchThreshold += clutchThreshold;
    }

    private void OnARFButtonClicked() {
        if (ARFValue + 500 > 1500) ARFValue = 500;
        else ARFValue += 500;
        carController.FrontAxle.antirollForce = ARFValue;
    }

    private void OnDestroy() {
        if (autoReverseButton != null) autoReverseButton.onClick.RemoveListener(OnAutoReverseButtonClicked);
        if (gearboxButton != null) gearboxButton.onClick.RemoveListener(OnGearboxButtonClicked);
        if (GSTButton != null) GSTButton.onClick.RemoveListener(OnGSTButtonClicked);
        if (shiftingDelayButton != null) shiftingDelayButton.onClick.RemoveListener(OnShiftingDelayButtonClicked);
        if (CTWSButton != null) CTWSButton.onClick.RemoveListener(OnCTWSButtonClicked);
        if (clutchThresholdButton != null) clutchThresholdButton.onClick.RemoveListener(OnClutchThresholdButtonClicked);
        if (ARFButton != null) ARFButton.onClick.RemoveListener(OnARFButtonClicked);
    }
}
