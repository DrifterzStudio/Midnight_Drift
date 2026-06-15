using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class Grip : RCCP_GenericComponent {
    
    private RCCP_Stability stability;

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

    [Tooltip("Back page.")]
    public GameObject back;

    private float forwardValue = 0.5f;
    private float rearSidewaysValue = 0.9f;
    private float frontSidewaysValue = 0.7f;

    private void Awake() {
        forwardButton.onClick.AddListener(OnForwardButtonClicked);
        rearSidewaysButton.onClick.AddListener(OnRearSidewaysButtonClicked);
        frontSidewaysButton.onClick.AddListener(OnFrontSidewaysButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    void Update() {
        forwardText.text = "" + forwardValue;
        rearSidewaysText.text = "" + rearSidewaysValue;
        frontSidewaysText.text = "" + frontSidewaysValue;
    }

    private void OnForwardButtonClicked() {
        if (forwardValue + 0.5f > 2) forwardValue = 0;
        else forwardValue += 0.5f;
        stability.driftRearForwardStiffnessMin = forwardValue;
    }

    private void OnRearSidewaysButtonClicked() {
        if (rearSidewaysValue + 0.5f > 2) rearSidewaysValue = 0;
        else rearSidewaysValue += 0.5f;
        stability.driftRearSidewaysStiffnessMin = rearSidewaysValue;
    }

    private void OnFrontSidewaysButtonClicked() {
        if (frontSidewaysValue + 0.5f > 2) frontSidewaysValue = 0;
        else frontSidewaysValue += 0.5f;
        stability.driftFrontSidewaysStiffnessMin = frontSidewaysValue;
    }

    private void OnBackButtonClicked() {
        gameObject.SetActive(false);
        back.SetActive(true);
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
        if (backButton != null) {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
    }
}
