using UnityEngine;
using UnityEngine.UI;

public class PropulsionType : MonoBehaviour {

    public RCCP_CarController controller;

    [Header("Drive Wheels Type")]
    [Tooltip("DWT Button.")]
    public Button DWTButton;
    [Tooltip("DWT Text.")]
    public Text DWTText;

    private int driveType = 1;

    private void Awake() {
        DWTButton.onClick.AddListener(OnDWTButtonClicked);
    }

    private void Update() {

        if (driveType == 0) {
            DWTText.text = "Front wheels drive";
            controller.FrontAxle.isSteer = true;
            controller.FrontAxle.isHandbrake = true;
            controller.RearAxle.isSteer = false;
            controller.RearAxle.isHandbrake = false;
        }
        if (driveType == 1) {
            DWTText.text = "Rear wheels drive"; 
            controller.FrontAxle.isSteer = true;
            controller.FrontAxle.isHandbrake = false;
            controller.RearAxle.isSteer = false;
            controller.RearAxle.isHandbrake = true;
        }
        if (driveType == 2) {
            DWTText.text = "All wheels drive";
            controller.FrontAxle.isSteer = true;
            controller.FrontAxle.isHandbrake = true;
            controller.RearAxle.isSteer = true;
            controller.RearAxle.isHandbrake = true;
        }
        SaveSettings.vehiculeSettings = controller;

    }

    private void OnDWTButtonClicked() {
        if (driveType + 1 > 2) driveType = 0;
        else driveType += 1;
        
    }

    private void OnDestroy() {
         DWTButton.onClick.RemoveAllListeners();
    }

}
