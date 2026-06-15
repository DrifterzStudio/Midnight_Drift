using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using static RCCP_Differential;

public class Absorbers : RCCP_GenericComponent {
    private RCCP_CarController carController;

    [Tooltip("The button that change the differential.")]
    public Button diffButton;
    [Tooltip("Text showing the current state of differential.")]
    public Text diffText;

    private int currentDifferential = 1;

    private void Awake() {
        diffButton.onClick.AddListener(OnButtonClicked);
    }

    private void Update() {
        carController = RCCPSceneManager.activePlayerVehicle;

        if (currentDifferential == 0) {
            for (int i = 0; i < carController.Differentials.Length; i++) {
                carController.Differentials[i].differentialType = DifferentialType.Open;
            }
            diffText.text = "Open";
        }
        else if (currentDifferential == 1) {
            for (int i = 0; i < carController.Differentials.Length; i++) {
                carController.Differentials[i].differentialType = DifferentialType.Limited;
            }
            diffText.text = "Limited";
        }
        else if (currentDifferential == 2) {
            for (int i = 0; i < carController.Differentials.Length; i++) {
                carController.Differentials[i].differentialType = DifferentialType.FullLocked;
            }
            diffText.text = "Full locked";
        }
        else if (currentDifferential == 3) {
            for (int i = 0; i < carController.Differentials.Length; i++) {
                carController.Differentials[i].differentialType = DifferentialType.Direct;
            }
            diffText.text = "Direct";
        }

    }

    private void OnButtonClicked() {
        currentDifferential++;
        if (currentDifferential > 3) currentDifferential = 0;
    }

    private void OnDestroy() {
        diffButton.onClick.RemoveAllListeners();
    }
}
