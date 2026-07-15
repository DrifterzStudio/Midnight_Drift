using UnityEngine;
using UnityEngine.UI;

public class Wheels : MonoBehaviour, IDataPersistence {

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



    private void Awake() {
        Debug.Log("Load instance");
        if (instance == null) {
            instance = this;
            Debug.Log("Instance loaded");
        }
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);

        camberButton.onClick.AddListener(OnCamberButtonClicked);
        steeringSensitivityButton.onClick.AddListener(OnSteerSensitivityButtonClicked);
        steeringCurveButton.onClick.AddListener(OnSteerCurveButtonClicked);
        gripButton.onClick.AddListener(OnGripButtonClicked);
    }
    private void Update() {
        instance = this;

        sterringSensitivityText.text = "" + sensitivityValue;
    }

    private void OnCamberButtonClicked() {
        gameObject.SetActive(false);
        camber.SetActive(true);
    }
    private void OnGripButtonClicked() {
        gameObject.SetActive(false);
        grip.SetActive(true);
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
