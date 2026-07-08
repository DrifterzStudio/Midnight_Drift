using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Switch car with a key.
/// </summary>
public class ChangeCar : MonoBehaviour {

    [Tooltip("The list of the different cars that the player can play.")]
    [Space()]
    public List<GameObject> cars;

    private GameObject activeCar = null;
    private int whichCar = 0;

    private void Start() {
        if (activeCar == null && cars.Count >= 1) {
            activeCar = cars[0];
        }
        SwitchCar();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (whichCar == 0) {
                whichCar = cars.Count - 1;
            }
            else {
                whichCar--;
            }
            SwitchCar();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
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
