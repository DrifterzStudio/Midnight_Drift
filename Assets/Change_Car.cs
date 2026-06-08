using UnityEngine;
using System.Collections.Generic;

public class ChangeCar : MonoBehaviour {
    public List<GameObject> cars;
    public GameObject activeCar = null;
    public int whichCar = 0;

    private void Start() {
        if (activeCar == null && cars.Count >= 1) {
            activeCar = cars[0];
        }
        SwitchCar();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            if (whichCar == 0) {
                whichCar = cars.Count - 1;
            }
            else {
                whichCar--;
            }
            SwitchCar();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (whichCar == cars.Count - 1) {
                whichCar = 0;
            }
            else {
                whichCar++;
            }
            SwitchCar();
        }
    }

    void SwitchCar() {
        activeCar = cars[whichCar];
        activeCar.SetActive(true);
        for (int i = 0; i < cars.Count; i++) {
            if (cars[i] != activeCar) {
                cars[i].SetActive(false);
            }
        }
    }
}
