using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Switch car with a button.
/// </summary>
public class SwitchVehiculeButton : MonoBehaviour {
    [Tooltip("The button that change the car.")]
    [Space()]
    public Button switchCarButton;

    [Tooltip("The list of the different cars that the player can play.")]
    [Space()]
    public List<GameObject> cars;

    private GameObject activeCar = null;
    private int whichCar = 0;

    private void Awake() {
        if (switchCarButton == null) {
            return;
        }
        switchCarButton.onClick.AddListener(OnButtonClicked);

        if (activeCar == null && cars.Count >= 1) {
            activeCar = cars[0];
        }
        SwitchCar();
    }

    private void OnButtonClicked() {
        if (whichCar == 0) {
            whichCar = cars.Count - 1;
        }
        else {
            whichCar--;
        }
        SwitchCar();
    }

    private void SwitchCar() {
        activeCar = cars[whichCar];
        activeCar.SetActive(true);
        for (int i = 0; i < cars.Count; i++) {
            if (cars[i] != activeCar) {
                cars[i].SetActive(false);
            }
        }
    }

    private void OnDestroy() {
        if (switchCarButton != null) {
            switchCarButton.onClick.RemoveListener(OnButtonClicked);
        }
    }
}
