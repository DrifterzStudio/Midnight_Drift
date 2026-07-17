using UnityEngine;
using UnityEngine.UI;

public class Camber : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

    public RCCP_CarController controller;

    [Tooltip("Camber front button.")]
    public Button frontButton;

    [Tooltip("Camber rear button.")]
    public Button rearButton;

    [Tooltip("Text showing the current value of camber front angle.")]
    public Text frontAngleText;

    [Tooltip("Text showing the current value of camber rear angle.")]
    public Text rearAngleText;

    [Tooltip("Back button.")]
    public Button backButton;

    public float frontAngle = 0;
    public float rearAngle = 0;

    public static Camber instance;

    public void LoadGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            frontAngle = tmp.frontAngle;
            rearAngle = tmp.rearAngle;
        }

        ApplyToController();
        RefreshUI();
    }

    public void SaveGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            tmp.frontAngle = frontAngle;
            tmp.rearAngle = rearAngle;
        }
    }

    public string getDataFileName() {
        return dataFileName;
    }

    // Was missing IVehicleDependent, so 'controller' stayed null and every click threw.
    public void SetController(RCCP_CarController newController) {
        controller = newController;
        ApplyToController();
        RefreshUI();
    }

    private void Awake() {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);

        if (frontButton != null) frontButton.onClick.AddListener(OnFrontButtonClicked);
        if (rearButton != null) rearButton.onClick.AddListener(OnRearButtonClicked);
        if (backButton != null) backButton.onClick.AddListener(OnBackButtonClicked);
    }

    // backButton was declared but wired to nothing, so this panel had no way out.
    private void OnBackButtonClicked() {
        if (Wheels.instance != null)
            Wheels.instance.ShowMenu();
    }

    private void Start() {
        RefreshUI();
    }

    // -10..+10 in 5-degree steps. Went to +/-15 before, outside the -10..10 range RCCP uses on
    // its own camber sliders.
    private void OnFrontButtonClicked() {
        if (frontAngle + 5f > 10f) frontAngle = -10f;
        else frontAngle += 5f;

        ApplyToController();
        RefreshUI();
    }

    private void OnRearButtonClicked() {
        if (rearAngle + 5f > 10f) rearAngle = -10f;
        else rearAngle += 5f;

        ApplyToController();
        RefreshUI();
    }

    void ApplyToController() {
        RCCP_CustomizationData custom = CustomizationData;

        if (custom == null)
            return;

        custom.cambersFront = frontAngle;
        custom.cambersRear = rearAngle;
    }

    // Called only when a value changes, never per frame.
    void RefreshUI() {
        if (frontAngleText != null) frontAngleText.text = "" + (int)frontAngle;
        if (rearAngleText != null) rearAngleText.text = "" + (int)rearAngle;
    }

    RCCP_CustomizationData CustomizationData {
        get {
            if (controller == null || controller.Customizer == null || controller.Customizer.loadout == null)
                return null;

            return controller.Customizer.loadout.customizationData;
        }
    }

    private void OnDestroy() {
        if (frontButton != null) frontButton.onClick.RemoveListener(OnFrontButtonClicked);
        if (rearButton != null) rearButton.onClick.RemoveListener(OnRearButtonClicked);
        if (backButton != null) backButton.onClick.RemoveListener(OnBackButtonClicked);
    }
}
