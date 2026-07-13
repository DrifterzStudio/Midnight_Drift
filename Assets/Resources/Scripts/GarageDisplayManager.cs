using UnityEngine;

public class GarageDisplayManager : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject customizationPanel;

    private GameObject currentInstance;

    public void ShowVehicle(VehicleDefinition vehicle)
    {
        // Save the vehicle we are leaving before switching to a new one
        if (GameSession.SelectedVehicle != null)
            DataPersistenceManager.instance.SaveGameFor(GameSession.SelectedVehicle.vehicleId);

        if (currentInstance != null)
            Destroy(currentInstance);

        currentInstance = Instantiate(vehicle.prefab, spawnPoint.position, spawnPoint.rotation);
        RCCP_CarController controller = currentInstance.GetComponent<RCCP_CarController>();

        CarInstance.instance = controller;
        AssignControllerToUpgradeComponents(controller);

        GameSession.SelectVehicle(vehicle);
        DataPersistenceManager.instance.LoadGameFor(vehicle.vehicleId);

        customizationPanel.SetActive(true);
    }

    public void CloseCustomization()
    {
        if (GameSession.SelectedVehicle != null)
            DataPersistenceManager.instance.SaveGameFor(GameSession.SelectedVehicle.vehicleId);

        customizationPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Single place that knows how to point every upgrade component at the active vehicle.
    void AssignControllerToUpgradeComponents(RCCP_CarController controller)
    {
        AntiRollBar.instance.controller = controller;
        Brake.instance.controller = controller;
        CarbonFiberBody.instance.controller = controller;
        Differential.instance.controller = controller;
        EnginePower.instance.controller = controller;
        AddTurbo.instance.controller = controller;
        Slick.instance.controller = controller;
        SuspensionUpgrade.instance.controller = controller;
        Reinforcement.instance.controller = controller;
        GearboxRatio.instance.controller = controller;
        PropulsionType.instance.controller = controller;

        DrivingAid.instance.carController = controller;
        Gearbox.instance.carController = controller;
        Others.instance.carController = controller;
    }
}