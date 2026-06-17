using Unity.VisualScripting;
using UnityEngine;

public class Starting : RCCP_GenericComponent {

    RCCP_CarController carController;

    private float timer;
    private bool[] countdown = { false,  false, false }; 

    void Start() {
        timer = 0;
        carController = RCCPSceneManager.activePlayerVehicle;
    }

    void Update() {
        timer += Time.deltaTime;
        if (timer < 3) {
            carController.canControl = false;
        }
        if (timer >= 3 && !countdown[2]) {
            Debug.Log("1");
            countdown[2] = true;
            carController.canControl = true;
        }
        else if (timer >= 2 && !countdown[1]) {
            Debug.Log("2");
            countdown[1] = true;
        }
        else if (timer >= 1 && !countdown[0]) {
            Debug.Log("3");
            countdown[0] = true;
        }
    }
}
