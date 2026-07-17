using UnityEngine;
using UnityEngine.UI;
using static RCCP_Gearbox;
using static RCCP_Gearbox.CurrentGearState;

public class Gearbox : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

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

    // Anti-roll force used to live here too, fighting AntiRollBar (the Chassis upgrade) over
    // FrontAxle.antirollForce with no deterministic order. AntiRollBar owns it now.

    private bool isReverse = false;
    public GearState gearState = GearState.InForwardGear;
    private int gearboxType = 1;
    public TransmissionType transmissionType = TransmissionType.Automatic;
    public float GSTValue = .7f;
    public float shiftingDelay = .2f;
    public bool CTWS = true;
    public float clutchThreshold = .1f;

    public static Gearbox instance;


    public void LoadGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            gearState = tmp.isReverse;
            transmissionType = tmp.transmissionType;
            GSTValue = tmp.GSTValue;
            shiftingDelay = tmp.shiftingDelay;
            CTWS = tmp.CTWS;
            clutchThreshold = tmp.clutchThreshold;

            // isReverse and gearboxType drive the labels but aren't persisted themselves, so
            // rebuild them from the values that are. Update() used to hide this by rewriting
            // the labels every frame.
            isReverse = gearState == GearState.InReverseGear;
            gearboxType = TransmissionToIndex(transmissionType);
        }

        ApplyToController();
        RefreshUI();
    }

    public void SaveGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            tmp.isReverse = gearState;
            tmp.transmissionType = transmissionType;
            tmp.GSTValue = GSTValue;
            tmp.shiftingDelay = shiftingDelay;
            tmp.CTWS = CTWS;
            tmp.clutchThreshold = clutchThreshold;
        }
    }

    public string getDataFileName() {
        return dataFileName;
    }

    public void SetController(RCCP_CarController newController) {
        carController = newController;
        ApplyToController();
        RefreshUI();
    }


    private void Awake() {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);

        if (autoReverseButton != null) autoReverseButton.onClick.AddListener(OnAutoReverseButtonClicked);
        if (gearboxButton != null) gearboxButton.onClick.AddListener(OnGearboxButtonClicked);
        if (GSTButton != null) GSTButton.onClick.AddListener(OnGSTButtonClicked);
        if (shiftingDelayButton != null) shiftingDelayButton.onClick.AddListener(OnShiftingDelayButtonClicked);
        if (CTWSButton != null) CTWSButton.onClick.AddListener(OnCTWSButtonClicked);
        if (clutchThresholdButton != null) clutchThresholdButton.onClick.AddListener(OnClutchThresholdButtonClicked);
    }

    private void Start() {
        RefreshUI();
    }

    private void OnAutoReverseButtonClicked() {
        isReverse = !isReverse;
        gearState = isReverse ? GearState.InReverseGear : GearState.InForwardGear;

        if (carController != null && carController.Gearbox != null)
            carController.Gearbox.currentGearState.gearState = gearState;

        RefreshUI();
    }

    private void OnGearboxButtonClicked() {
        if (gearboxType + 1 > 2) gearboxType = 0;
        else gearboxType += 1;

        transmissionType = IndexToTransmission(gearboxType);

        if (carController != null && carController.Gearbox != null)
            carController.Gearbox.transmissionType = transmissionType;

        RefreshUI();
    }

    private void OnGSTButtonClicked() {
        // Threshold and shifting delay only apply to the automatic gearbox.
        if (gearboxType != 1)
            return;

        if (GSTValue + .1f > .9f) GSTValue = .1f;
        else GSTValue += .1f;

        if (carController != null && carController.Gearbox != null)
            carController.Gearbox.shiftThreshold = GSTValue;

        RefreshUI();
    }

    private void OnShiftingDelayButtonClicked() {
        if (gearboxType != 1)
            return;

        if (shiftingDelay + .1f > .5f) shiftingDelay = .2f;
        else shiftingDelay += .1f;

        if (carController != null && carController.Gearbox != null)
            carController.Gearbox.shiftingTime = shiftingDelay;

        RefreshUI();
    }

    private void OnCTWSButtonClicked() {
        CTWS = !CTWS;

        if (carController != null && carController.Inputs != null)
            carController.Inputs.cutThrottleWhenShifting = CTWS;

        RefreshUI();
    }

    private void OnClutchThresholdButtonClicked() {
        if (clutchThreshold + .2f > 1f) clutchThreshold = 0f;
        else clutchThreshold += .2f;

        if (carController != null && carController.Customizer != null && carController.Customizer.loadout != null
            && carController.Customizer.loadout.customizationData != null) {
            carController.Customizer.loadout.customizationData.clutchThreshold += clutchThreshold;
        }

        RefreshUI();
    }

    void ApplyToController() {
        if (carController == null)
            return;

        if (carController.Gearbox != null) {
            carController.Gearbox.transmissionType = transmissionType;
            carController.Gearbox.currentGearState.gearState = gearState;
            carController.Gearbox.shiftThreshold = GSTValue;
            carController.Gearbox.shiftingTime = shiftingDelay;
        }

        if (carController.Inputs != null)
            carController.Inputs.cutThrottleWhenShifting = CTWS;
    }

    // Called only when a value actually changes. Every label here used to be rewritten from
    // Update(), allocating a string per field per frame and dirtying the Canvas continuously.
    void RefreshUI() {
        if (autoReverseText != null)
            autoReverseText.text = isReverse ? "On" : "Off";

        if (otherParam != null)
            otherParam.SetActive(gearboxType == 1);

        if (gearboxTypeText != null) {
            if (gearboxType == 0) gearboxTypeText.text = "Manual";
            else if (gearboxType == 1) gearboxTypeText.text = "Automatic";
            else gearboxTypeText.text = "Automatic DNRP";
        }

        if (gearboxType == 1) {
            if (GSTText != null) GSTText.text = GSTValue.ToString();
            if (shiftingDelayText != null) shiftingDelayText.text = shiftingDelay.ToString();
        }

        if (CTWSText != null) CTWSText.text = CTWS ? "On" : "Off";
        if (clutchThresholdText != null) clutchThresholdText.text = clutchThreshold.ToString();
    }

    static int TransmissionToIndex(TransmissionType type) {
        if (type == TransmissionType.Manual) return 0;
        if (type == TransmissionType.Automatic) return 1;
        return 2;
    }

    static TransmissionType IndexToTransmission(int index) {
        if (index == 0) return TransmissionType.Manual;
        if (index == 1) return TransmissionType.Automatic;
        return TransmissionType.Automatic_DNRP;
    }

    private void OnDestroy() {
        if (autoReverseButton != null) autoReverseButton.onClick.RemoveListener(OnAutoReverseButtonClicked);
        if (gearboxButton != null) gearboxButton.onClick.RemoveListener(OnGearboxButtonClicked);
        if (GSTButton != null) GSTButton.onClick.RemoveListener(OnGSTButtonClicked);
        if (shiftingDelayButton != null) shiftingDelayButton.onClick.RemoveListener(OnShiftingDelayButtonClicked);
        if (CTWSButton != null) CTWSButton.onClick.RemoveListener(OnCTWSButtonClicked);
        if (clutchThresholdButton != null) clutchThresholdButton.onClick.RemoveListener(OnClutchThresholdButtonClicked);
    }
}
