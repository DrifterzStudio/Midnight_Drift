using UnityEngine;
using UnityEngine.UI;

public class DrivingAid : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

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

    // These used to live directly on GetVehicleBehaviorType(), so this script had no state of
    // its own and every read threw before a car was shown. They are the source of truth now, and
    // ApplyToController pushes them onto the behavior.
    public bool ABS = true;
    public bool TCS = true;
    public bool ESP = true;
    public bool SH = true;
    public bool TH = true;
    public float ASPValue = 1f;

    public static DrivingAid instance;

    public void LoadGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            ABS = tmp.ABS;
            TCS = tmp.TCS;
            ESP = tmp.ESP;
            SH = tmp.SH;
            TH = tmp.TH;
            ASPValue = tmp.ASPValue;
        }

        ApplyToController();
        RefreshUI();
    }

    public void SaveGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            tmp.ABS = ABS;
            tmp.TCS = TCS;
            tmp.ESP = ESP;
            tmp.SH = SH;
            tmp.TH = TH;
            tmp.ASPValue = ASPValue;
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

        if (ABSButton != null) ABSButton.onClick.AddListener(OnABSButtonClicked);
        if (TCSButton != null) TCSButton.onClick.AddListener(OnTCSButtonClicked);
        if (ESPButton != null) ESPButton.onClick.AddListener(OnESPButtonClicked);
        if (SHButton != null) SHButton.onClick.AddListener(OnSHButtonClicked);
        if (THButton != null) THButton.onClick.AddListener(OnTHButtonClicked);
        if (ASPButton != null) ASPButton.onClick.AddListener(OnASPButtonClicked);
    }

    private void Start() {
        RefreshUI();
    }

    private void OnABSButtonClicked() {
        ABS = !ABS;
        ApplyToController();
        RefreshUI();
    }

    private void OnTCSButtonClicked() {
        TCS = !TCS;
        ApplyToController();
        RefreshUI();
    }

    private void OnESPButtonClicked() {
        ESP = !ESP;
        ApplyToController();
        RefreshUI();
    }

    private void OnSHButtonClicked() {
        SH = !SH;
        ApplyToController();
        RefreshUI();
    }

    private void OnTHButtonClicked() {
        TH = !TH;
        ApplyToController();
        RefreshUI();
    }

    private void OnASPButtonClicked() {
        if (ASPValue + .2f > 1.1f) ASPValue = 0f;
        else ASPValue += .2f;

        ApplyToController();
        RefreshUI();
    }

    /// <summary>
    /// Writes to this vehicle's own RCCP_Stability. The original went through
    /// GetVehicleBehaviorType(), which returns an object inside the shared RCCP_Settings
    /// ScriptableObject: every car saw the change, and in the Editor it was written to disk and
    /// survived Play Mode. RCCP_CarController only ever copies that behavior into Stability
    /// (see its CheckBehavior), so Stability is the real per-vehicle home for these flags.
    /// CheckBehavior runs once on spawn and on OnBehaviorChanged, both before SetController
    /// reaches us, so these writes are not overwritten.
    /// </summary>
    void ApplyToController() {
        if (carController == null || carController.Stability == null)
            return;

        carController.Stability.ABS = ABS;
        carController.Stability.TCS = TCS;
        carController.Stability.ESP = ESP;
        carController.Stability.steeringHelper = SH;
        carController.Stability.tractionHelper = TH;
        carController.Stability.preserveSpeedFactor = ASPValue;
    }

    // Called only when a value changes, never per frame.
    void RefreshUI() {
        if (ABSText != null) ABSText.text = ABS ? "On" : "Off";
        if (TCSText != null) TCSText.text = TCS ? "On" : "Off";
        if (ESPText != null) ESPText.text = ESP ? "On" : "Off";
        if (SHText != null) SHText.text = SH ? "On" : "Off";
        if (THText != null) THText.text = TH ? "On" : "Off";
        if (ASPText != null) ASPText.text = ASPValue.ToString();
    }

    private void OnDestroy() {
        if (ABSButton != null) ABSButton.onClick.RemoveListener(OnABSButtonClicked);
        if (TCSButton != null) TCSButton.onClick.RemoveListener(OnTCSButtonClicked);
        if (ESPButton != null) ESPButton.onClick.RemoveListener(OnESPButtonClicked);
        if (SHButton != null) SHButton.onClick.RemoveListener(OnSHButtonClicked);
        if (THButton != null) THButton.onClick.RemoveListener(OnTHButtonClicked);
        if (ASPButton != null) ASPButton.onClick.RemoveListener(OnASPButtonClicked);
    }
}
