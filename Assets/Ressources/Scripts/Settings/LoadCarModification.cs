using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class LoadCarModification : MonoBehaviour {

    public RCCP_CarController controller;

    void Start() {

        if (SaveSetttings.vehiculeSettings != controller) {
            controller = SaveSetttings.vehiculeSettings;
        } 

    }

}
