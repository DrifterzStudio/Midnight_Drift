using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class Braking : MonoBehaviour {

    public RCCP_CarController controller;

    [Tooltip("Button that switch the state of the handbrake.")]
    public Button handbrakeButton;

    [Tooltip("Text showing the state of the handbrake.")]
    public Text handbrakeText;

    [Tooltip("Button that change the valiue of the handbrake multiplier.")]
    public Button handbrakeMultiplierButton;

    [Tooltip("Text showing the value of the handbrake multiplier.")]
    public Text handbrakeMultiplierText;

    [Tooltip("Button that change the valiue of the brake multiplier.")]
    public Button brakeMultiplierButton;

    [Tooltip("Text showing the value of the brake multiplier.")]
    public Text brakeMultiplierText;

    private float handbrakeMultiplier = .6f;
    private float brakeMultiplier = 1f;


    private void Awake() {
        handbrakeButton.onClick.AddListener(OnHandbrakeButtonClicked);
        handbrakeMultiplierButton.onClick.AddListener(OnHandbrakeMultiplierButtonClicked);
        brakeMultiplierButton.onClick.AddListener(OnBrakeMultiplierButtonClicked);
    }

    private void Update() {

        if (controller.RearAxle.isHandbrake) handbrakeText.text = "On";
        else handbrakeText.text = "Off";

        handbrakeMultiplierText.text = handbrakeMultiplier.ToString();
        brakeMultiplierText.text = brakeMultiplier.ToString();
        SaveSettings.vehiculeSettings = controller;
    }

    private void OnHandbrakeButtonClicked() {
        controller.RearAxle.isHandbrake = !controller.RearAxle.isHandbrake;
    }

    private void OnHandbrakeMultiplierButtonClicked() {
        if (handbrakeMultiplier + .1f > 1f) handbrakeMultiplier = 0f;
        else handbrakeMultiplier += .1f;
        controller.RearAxle.handbrakeMultiplier = handbrakeMultiplier;
        controller.FrontAxle.brakeMultiplier = handbrakeMultiplier;
    }

    private void OnBrakeMultiplierButtonClicked() {
        if (brakeMultiplier + .1f > 1f) brakeMultiplier = 0f;
        else brakeMultiplier += .1f;
        controller.RearAxle.brakeMultiplier = brakeMultiplier;
    }

    private void OnDestroy() {
        if (handbrakeButton != null) handbrakeButton.onClick.RemoveListener(OnHandbrakeButtonClicked);
        if (handbrakeMultiplierButton != null) handbrakeMultiplierButton.onClick.RemoveListener(OnHandbrakeMultiplierButtonClicked);
        if (brakeMultiplierButton != null) brakeMultiplierButton.onClick.RemoveListener(OnBrakeMultiplierButtonClicked);
    }

}
