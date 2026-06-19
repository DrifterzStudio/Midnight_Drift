using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class LoadCarModification : MonoBehaviour {

    public RCCP_CarController controller;

    void Update() {

        if (controller != SaveSettings.vehiculeSettings) {
            controller = SaveSettings.vehiculeSettings;
        }
    }

}
