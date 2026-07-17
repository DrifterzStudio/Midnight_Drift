using UnityEngine;
using UnityEngine.UI;

public class Wheels : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

    public RCCP_CarController controller;

    [Header("Menu")]
    [Tooltip("Container holding this menu's own buttons (Slick, Camber, Grip). Must be a sibling " +
             "of the Camber and Grip panels, not their parent, or hiding it would hide them too.")]
    public GameObject wheelsMenu;

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

    // Slow / Normal / Fast, inside RCCP_Axle.steerSpeed's own [.01, 5] range, centred on its
    // default of 1. This stepped 0 / .5 / 1 before, and 0 is below the minimum: the steering
    // would have frozen solid.
    private static readonly float[] SensitivitySteps = { .5f, 1f, 2f };
    private static readonly string[] SensitivityLabels = { "Slow", "Normal", "Fast" };

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
        ShowSubPanel(camber);
    }

    private void OnGripButtonClicked() {
        ShowSubPanel(grip);
    }

    /// <summary>
    /// Camber and Grip call this from their back buttons. Wheels owns the navigation between its
    /// menu and its sub-panels, so no one else toggles those GameObjects.
    /// </summary>
    public void ShowMenu() {
        if (camber != null) camber.SetActive(false);
        if (grip != null) grip.SetActive(false);
        if (wheelsMenu != null) wheelsMenu.SetActive(true);
    }

    /// <summary>
    /// Hides the menu and reveals one sub-panel. This used to call gameObject.SetActive(false),
    /// which disabled whatever object the script sits on: with every settings script living on
    /// ComponentUpgrade, one click took the entire customization menu down with it.
    /// </summary>
    void ShowSubPanel(GameObject panel) {
        if (panel == null)
            return;

        if (wheelsMenu == null) {
            Debug.LogWarning("Wheels: 'Wheels Menu' is not assigned, refusing to open the sub-panel.", this);
            return;
        }

        wheelsMenu.SetActive(false);
        panel.SetActive(true);
    }

    private void OnSteerSensitivityButtonClicked() {
        sensitivityValue = SensitivitySteps[(NearestStepIndex(sensitivityValue) + 1) % SensitivitySteps.Length];

        ApplyToController();
        RefreshUI();
    }

    private void OnSteerCurveButtonClicked() { } // RCCP_Input.steeringCurve, an AnimationCurve

    // The saved value is the raw setting, not an index, so the cursor is rebuilt from it.
    static int NearestStepIndex(float value) {
        int nearest = 0;
        float smallestDelta = Mathf.Abs(value - SensitivitySteps[0]);

        for (int i = 1; i < SensitivitySteps.Length; i++) {
            float delta = Mathf.Abs(value - SensitivitySteps[i]);

            if (delta < smallestDelta) {
                smallestDelta = delta;
                nearest = i;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Drives RCCP_Axle.steerSpeed, how fast the wheels reach the requested angle. The original
    /// wrote GetVehicleBehaviorType().steeringSensitivity, wrong twice over: that object lives
    /// inside the shared RCCP_Settings ScriptableObject (so the write hit every vehicle and was
    /// saved to disk in the Editor), and RCCP reads steeringSensitivity nowhere, so it steered
    /// nothing regardless.
    /// </summary>
    void ApplyToController() {
        if (controller == null)
            return;

        // Applied to both axles: the rear one steers too once PropulsionType is set to AWD.
        if (controller.FrontAxle != null)
            controller.FrontAxle.steerSpeed = sensitivityValue;

        if (controller.RearAxle != null)
            controller.RearAxle.steerSpeed = sensitivityValue;
    }

    // Called only when the value changes, never per frame.
    void RefreshUI() {
        if (sterringSensitivityText != null)
            sterringSensitivityText.text = SensitivityLabels[NearestStepIndex(sensitivityValue)];
    }

    private void OnDestroy() {
        if (camberButton != null) camberButton.onClick.RemoveListener(OnCamberButtonClicked);
        if (gripButton != null) gripButton.onClick.RemoveListener(OnGripButtonClicked);
        if (steeringSensitivityButton != null) steeringSensitivityButton.onClick.RemoveListener(OnSteerSensitivityButtonClicked);
        if (steeringCurveButton != null) steeringCurveButton.onClick.RemoveListener(OnSteerCurveButtonClicked);
    }
}
