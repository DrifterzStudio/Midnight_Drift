using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWheels : MonoBehaviour {

    public RCCP_CarController controller;

    public  List<Material> wheelsSets;

    public Button wheelsButton;

    public Text wheelsText;

    private int wheelsSetIndex = -1;

    void Update() {
    
    }
}
