using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWheels : MonoBehaviour {

    public RCCP_CarController controller;

    public List<Transform> wheelsSet;

    public Button wheelsButton;

    public Text wheelsText;

    private int wheelsSetIndex = 0;

    private void Awake() {
        wheelsButton.onClick.AddListener(OnButtonClicked);
    }

    void Update() {
        controller.FrontAxle.leftWheelModel = wheelsSet[wheelsSetIndex];
        controller.FrontAxle.rightWheelModel = wheelsSet[wheelsSetIndex + 1];
        controller.RearAxle.leftWheelModel = wheelsSet[wheelsSetIndex + 2];
        controller.RearAxle.rightWheelModel = wheelsSet[wheelsSetIndex + 3];

        wheelsText.text = "" + wheelsSetIndex / 4;
    }

    private void OnButtonClicked() {
        if (wheelsSetIndex + 4 > wheelsSet.Count) wheelsSetIndex = 0;
        else wheelsSetIndex += 4;
    }
}
