using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class Camber : MonoBehaviour {

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

    [Tooltip("Back page.")]
    public GameObject back;

    private float frontAngle = 0;
    private float rearAngle = 0;

    private void Awake() { 
        frontButton.onClick.AddListener(OnFrontButtonClicked);
        rearButton.onClick.AddListener(OnRearButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void Update() {
        frontAngleText.text = "" + (int)frontAngle;
        rearAngleText.text = "" + (int)rearAngle;
    }

    private void OnFrontButtonClicked() {
        if (frontAngle + 5f > 15f) frontAngle = -15f;
        else frontAngle += 5f;
        controller.Customizer.loadout.customizationData.cambersFront = frontAngle;
        SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.cambersFront = frontAngle;
    }

    private void OnRearButtonClicked() {
        if (rearAngle + 5f > 15f) rearAngle = -15f;
        else rearAngle += 5f;
        controller.Customizer.loadout.customizationData.cambersRear = rearAngle;
        SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.cambersRear = rearAngle;
    }

    private void OnBackButtonClicked() {
        gameObject.SetActive(false);
        back.SetActive(true);
    }

    private void OnDestroy() {
        if (frontButton != null) {
            frontButton.onClick.RemoveListener(OnFrontButtonClicked);
        }
        if (rearButton != null) {
            rearButton.onClick.RemoveListener(OnRearButtonClicked);
        }
        if (backButton != null) {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
    }
}
