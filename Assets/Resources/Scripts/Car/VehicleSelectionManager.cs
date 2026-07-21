using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VehicleSelectionManager : MonoBehaviour
{
    [Tooltip("All the vehicles the player can pick.")]
    public List<VehicleDefinition> vehicles;

    [Tooltip("Prefab of one button in the list.")]
    public Button buttonPrefab;

    [Tooltip("Where the buttons get spawned.")]
    public Transform buttonContainer;

    public GarageDisplayManager garageDisplay;

    void OnEnable()
    {
        BuildButtons();
    }

    void BuildButtons()
    {
        // Clear old buttons first, in case the menu gets opened more than once
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        foreach (VehicleDefinition vehicle in vehicles)
        {
            Button b = Instantiate(buttonPrefab, buttonContainer);
            TMP_Text label = b.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = vehicle.displayName;

            // capture the variable properly for the lambda
            VehicleDefinition capturedVehicle = vehicle;
            b.onClick.AddListener(() => OnVehicleChosen(capturedVehicle));
        }
    }

    void OnVehicleChosen(VehicleDefinition vehicle)
    {
        GameSession.SelectVehicle(vehicle);
        garageDisplay.ShowVehicle(vehicle);
    }
}