using UnityEngine;
using UnityEngine.UI;

public class Wheels : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

    public RCCP_CarController controller;

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

    [Tooltip("Text showing the current value of steering sensitivity.")]
    public Text sterringSensitivityText;

    [Tooltip("Steering curve button.")]
    public Button steeringCurveButton;

    //public float steeringCurve;

    public float sensitivityValue = 1f;

    public static Wheels instance;

    public void LoadGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            //steeringCurve = tmp.steeringCurve;
            sensitivityValue = tmp.sensitivityValue;
        }

        ApplyToController();
        RefreshUI();
    }

    public void SaveGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            //tmp.steeringCurve = steeringCurve;
            tmp.sensitivityValue = sensitivityValue;
        }
    }

    public string getDataFileName() {
        return dataFileName;
    }

    // Was missing IVehicleDependent, so 'controller' stayed null and the sensitivity click threw.
    public void SetController(RCCP_CarController newController) {
        controller = newController;
        ApplyToController();
        RefreshUI();
    }

    private void Awake() {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);

        if (camberButton != null) camberButton.onClick.AddListener(OnCamberButtonClicked);
        if (gripButton != null) gripButton.onClick.AddListener(OnGripButtonClicked);
        if (steeringSensitivityButton != null) steeringSensitivityButton.onClick.AddListener(OnSteerSensitivityButtonClicked);
        if (steeringCurveButton != null) steeringCurveButton.onClick.AddListener(OnSteerCurveButtonClicked);
    }

    // This panel hides itself when opening Camber/Grip, so refresh on every re-enable rather
    // than once in Start.
    private void OnEnable() {
        RefreshUI();
    }

    private void OnCamberButtonClicked() {
        gameObject.SetActive(false);
        if (camber != null) camber.SetActive(true);
    }

    private void OnGripButtonClicked() {
        gameObject.SetActive(false);
        if (grip != null) grip.SetActive(true);
    }

    private void OnSteerSensitivityButtonClicked() {
        if (sensitivityValue + 0.5f > 1f) sensitivityValue = 0f;
        else sensitivityValue += 0.5f;

        ApplyToController();
        RefreshUI();
    }

    private void OnSteerCurveButtonClicked() { } // controller.GetVehiculeType().sterringCurve

    /// <summary>
    /// Does nothing on purpose, and the Steering Sensitivity button is inert until this is
    /// settled. It used to write GetVehicleBehaviorType().steeringSensitivity, which was wrong
    /// twice over: that object lives inside the shared RCCP_Settings ScriptableObject (so the
    /// write hit every vehicle and was saved to disk in the Editor), and RCCP never reads
    /// steeringSensitivity anywhere, so it steered nothing regardless. The value is still saved
    /// and shown; it needs to be pointed at a real RCCP steering parameter.
    /// </summary>
    void ApplyToController() {
    }

    // Called only when the value changes, never per frame.
    void RefreshUI() {
        if (sterringSensitivityText != null)
            sterringSensitivityText.text = "" + sensitivityValue;
    }

    private void OnDestroy() {
        if (camberButton != null) camberButton.onClick.RemoveListener(OnCamberButtonClicked);
        if (gripButton != null) gripButton.onClick.RemoveListener(OnGripButtonClicked);
        if (steeringSensitivityButton != null) steeringSensitivityButton.onClick.RemoveListener(OnSteerSensitivityButtonClicked);
        if (steeringCurveButton != null) steeringCurveButton.onClick.RemoveListener(OnSteerCurveButtonClicked);
    }
}
