using System.ComponentModel.Design;
using UnityEngine;

public class SpeedRatio : RCCP_GenericComponent {
    private RCCP_CarController carController;

    private void Start() {
    }
    private void Update() {
        carController = RCCPSceneManager.activePlayerVehicle;

    }
}
