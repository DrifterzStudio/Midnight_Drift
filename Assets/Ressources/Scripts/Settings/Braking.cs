using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class Braking : RCCP_GenericComponent {

    private RCCP_CarController controller;

    [Tooltip("Button that switch the state of the handbreak.")]
    public Button handbreakButton;

    [Tooltip("Text showing the state of the handbreak.")]
    public Text handbreakText;

    [Tooltip("Button that change the valiue of the handbreak multiplier.")]
    public Button handbreakMultiplierButton;

    [Tooltip("Text showing the value of the handbreak multiplier.")]
    public Text handbreakMultiplierText;

    [Tooltip("Button that change the valiue of the break multiplier.")]
    public Button breakMultiplierButton;

    [Tooltip("Text showing the value of the break multiplier.")]
    public Text breakMultiplierText;

    private float handbreakMultiplier = .6f;
    private float breakMultiplier = 1f;


    private void Awake() {
        handbreakButton.onClick.AddListener(OnHandbreakButtonClicked);
        handbreakMultiplierButton.onClick.AddListener(OnHandbreakMultiplierButtonClicked);
        breakMultiplierButton.onClick.AddListener(OnBreakMultiplierButtonClicked);
    }

    private void Update() {
        controller = RCCPSceneManager.activePlayerVehicle;

        if (controller.RearAxle.isHandbrake) handbreakText.text = "On";
        else handbreakText.text = "Off";

        handbreakMultiplierText.text = handbreakMultiplier.ToString();
        breakMultiplierText.text = breakMultiplier.ToString();
    }

    private void OnHandbreakButtonClicked() {
        controller.RearAxle.isHandbrake = !controller.RearAxle.isHandbrake;
    }

    private void OnHandbreakMultiplierButtonClicked() {
        if (handbreakMultiplier + .1f > 1f) handbreakMultiplier = 0f;
        else handbreakMultiplier += .1f;
        controller.RearAxle.handbrakeMultiplier = handbreakMultiplier;
        controller.FrontAxle.brakeMultiplier = handbreakMultiplier;
    }

    private void OnBreakMultiplierButtonClicked() {
        if (breakMultiplier + .1f > 1f) breakMultiplier = 0f;
        else breakMultiplier += .1f;
        controller.RearAxle.brakeMultiplier = breakMultiplier;
    }

    private void OnDestroy() {
        if (handbreakButton != null) handbreakButton.onClick.RemoveListener(OnHandbreakButtonClicked);
        if (handbreakMultiplierButton != null) handbreakMultiplierButton.onClick.RemoveListener(OnHandbreakMultiplierButtonClicked);
        if (breakMultiplierButton != null) breakMultiplierButton.onClick.RemoveListener(OnBreakMultiplierButtonClicked);
    }

}
