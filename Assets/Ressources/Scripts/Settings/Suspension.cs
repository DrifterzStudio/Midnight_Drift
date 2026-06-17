using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class Suspension : MonoBehaviour {

    public RCCP_CarController controller;

    [Tooltip("The button that change the suspensson distance.")]
    public Button distButton;
    [Tooltip("Text showing the current value of suspension distance.")]
    public Text distText;

    [Tooltip("The button that change the suspension froce.")]
    public Button forceButton;
    [Tooltip("Text showing the current value of suspension force.")]
    public Text forceText;

    [Tooltip("The button that change the suspension target.")]
    public Button targetButton;
    [Tooltip("Text showing the current value of suspension target.")]
    public Text targetText;

    [Tooltip("The button that change the suspension damper.")]
    public Button damperButton;
    [Tooltip("Text showing the current value of suspension damper.")]
    public Text damperText;

    private float distValue = 0f;
    private float forceValue = 0f;
    private float targetValue = 0f;
    private float damperValue = 0f;

    private void Awake() {
        distButton.onClick.AddListener(OnDistButtonClicked);
        forceButton.onClick.AddListener(OnForceButtonClicked);
        targetButton.onClick.AddListener(OnTargetButtonClicked);
        damperButton.onClick.AddListener(OnDamperButtonClicked);
    }

    private void Update() {
        distText.text = distValue.ToString();
        forceText.text = forceValue.ToString();
        targetText.text = targetValue.ToString();
        damperText.text = damperValue.ToString();
    }

    private void OnDistButtonClicked() {
        if (distValue > 1f) distValue = 0f;
        else distValue += .5f;
        controller.Customizer.loadout.customizationData.suspensionDistanceFront = distValue;
        controller.Customizer.loadout.customizationData.suspensionDistanceRear = distValue;
        SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionDistanceFront = distValue;
        SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionDistanceRear = distValue;
    }

    private void OnForceButtonClicked() {
        if (forceValue > 1f) forceValue = 0f;
        else forceValue += .5f;
        controller.Customizer.loadout.customizationData.suspensionSpringForceFront = forceValue;
        controller.Customizer.loadout.customizationData.suspensionSpringForceRear = forceValue;
        SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionSpringForceFront = forceValue;
        SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionSpringForceRear = forceValue;
    }

    private void OnTargetButtonClicked() {
        if (targetValue + .2f > 1) targetValue = 0f;
        else targetValue += .5f;
        controller.Customizer.loadout.customizationData.suspensionTargetFront = targetValue;
        controller.Customizer.loadout.customizationData.suspensionTargetRear = targetValue;
        SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionTargetFront = targetValue;
        SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionTargetRear = targetValue;
    }

    private void OnDamperButtonClicked() {
        if (damperValue > 1f) damperValue = 0f;
        else damperValue += 0.5f;
        controller.Customizer.loadout.customizationData.suspensionDamperFront = damperValue;
        controller.Customizer.loadout.customizationData.suspensionDamperRear = damperValue;
        SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionDamperFront = damperValue;
        SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionDamperRear = damperValue;
    }

    private void OnDestroy() {
        if (distButton != null) distButton.onClick.RemoveListener(OnDistButtonClicked);
        if (forceButton != null) distButton.onClick.RemoveListener(OnForceButtonClicked);
        if (targetButton != null) distButton.onClick.RemoveListener(OnTargetButtonClicked);
        if (damperButton != null) distButton.onClick.RemoveListener(OnDamperButtonClicked);
    }
}
