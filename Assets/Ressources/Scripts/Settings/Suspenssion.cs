using UnityEngine;
using UnityEngine.UI;

public class Suspenssion : RCCP_GenericComponent {
    //dictance / force / target
    private RCCP_CarController carController;

    [Tooltip("The button that change the differential.")]
    public Button suspenssionButton;
    [Tooltip("Text showing the current state of differential.")]
    public Text suspenssionText;

    private void Update() {
        carController = RCCPSceneManager.activePlayerVehicle;

        
    }
}
