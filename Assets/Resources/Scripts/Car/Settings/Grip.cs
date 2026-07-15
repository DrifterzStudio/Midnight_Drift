using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class Grip : MonoBehaviour, IDataPersistence {

    public string dataFileName;

    public RCCP_CarController controller;

    [Tooltip("Grip forward button.")]
    public Button forwardButton;

    [Tooltip("Grip rear sideways button.")]
    public Button rearSidewaysButton;

    [Tooltip("Grip front sideways button.")]
    public Button frontSidewaysButton;

    [Tooltip("Text showing the current value of forward grip.")]
    public Text forwardText;

    [Tooltip("Text showing the current value of rear sideways grip.")]
    public Text rearSidewaysText;

    [Tooltip("Text showing the current value of front sideways grip.")]
    public Text frontSidewaysText;

    [Tooltip("Back button.")]
    public Button backButton;

    public float forwardValue = 0.5f;
    public float rearSidewaysValue = 0.9f;
    public float frontSidewaysValue = 0.7f;

    public static Grip instance;


    public void LoadGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            forwardValue = tmp.forwardValue;
            rearSidewaysValue = tmp.rearSidewaysValue;
            frontSidewaysValue = tmp.frontSidewaysValue;
        }
    }

    public void SaveGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            tmp.forwardValue = forwardValue;
            tmp.rearSidewaysValue = rearSidewaysValue;
            tmp.frontSidewaysValue = frontSidewaysValue;
        }
    }

    public string getDataFileName() {
        return dataFileName;
    }


    private void Awake() {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);

        forwardButton.onClick.AddListener(OnForwardButtonClicked);
        rearSidewaysButton.onClick.AddListener(OnRearSidewaysButtonClicked);
        frontSidewaysButton.onClick.AddListener(OnFrontSidewaysButtonClicked);
    }

    void Update() {
        instance = this;

        forwardText.text = "" + forwardValue;
        rearSidewaysText.text = "" + rearSidewaysValue;
        frontSidewaysText.text = "" + frontSidewaysValue;
    }

    private void OnForwardButtonClicked() {
        if (forwardValue + 0.1f > 1) forwardValue = 0;
        else forwardValue += 0.1f;
        controller.Stability.driftRearForwardStiffnessMin = forwardValue;
    }

    private void OnRearSidewaysButtonClicked() {
        if (rearSidewaysValue + 0.1f > 1) rearSidewaysValue = 0;
        else rearSidewaysValue += 0.1f;
        controller.Stability.driftRearSidewaysStiffnessMin = rearSidewaysValue;
    }

    private void OnFrontSidewaysButtonClicked() {
        if (frontSidewaysValue + 0.1f > 1) frontSidewaysValue = 0;
        else frontSidewaysValue += 0.1f;
        controller.Stability.driftFrontSidewaysStiffnessMin = frontSidewaysValue;
    }

    private void OnDestroy() {
        if (forwardButton != null) {
            forwardButton.onClick.RemoveListener(OnForwardButtonClicked);
        }
        if (rearSidewaysButton != null) {
            rearSidewaysButton.onClick.RemoveListener(OnRearSidewaysButtonClicked);
        }
        if (frontSidewaysButton != null) {
            frontSidewaysButton.onClick.RemoveListener(OnFrontSidewaysButtonClicked);
        }
    }
}
