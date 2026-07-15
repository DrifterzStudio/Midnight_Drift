using System.Linq;
using UnityEngine;

public class GarageDisplayManager : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject customizationPanel;
    public GameObject vehicleSelectionMenu;
    public PlayerMovement player;

    private GameObject currentInstance;

    public void ShowVehicle(VehicleDefinition vehicle)
    {
        // Save the vehicle we are leaving before switching to a new one
        if (GameSession.SelectedVehicle != null)
            DataPersistenceManager.instance.SaveGameFor(GameSession.SelectedVehicle.vehicleId);

        if (currentInstance != null)
            Destroy(currentInstance);

        currentInstance = Instantiate(vehicle.prefab, spawnPoint.position, spawnPoint.rotation);
        RCCP_CarController controller = currentInstance.GetComponentInChildren<RCCP_CarController>(true);

        // Lights (and their lens flares) would look wrong in the small garage room, but only for
        // this preview instance, the prefab (and every other scene using it, like Circuit_Solo)
        // is left untouched. Deactivating the GameObject (rather than just disabling the script)
        // guarantees the lens flare stops rendering too, since it isn't tied to enabled state.
        foreach (RCCP_Light rccpLight in currentInstance.GetComponentsInChildren<RCCP_Light>(true))
            rccpLight.gameObject.SetActive(false);

        // Score/drift tracking is baked onto the vehicle prefab but only wired up with real UI
        // references in Circuit_Solo; it has no business running for the garage preview instance.
        foreach (Score score in currentInstance.GetComponentsInChildren<Score>(true))
            score.enabled = false;

        CarInstance.instance = controller;
        AssignControllerToUpgradeComponents(controller);

        GameSession.SelectVehicle(vehicle);
        DataPersistenceManager.instance.LoadGameFor(vehicle.vehicleId);

        customizationPanel.SetActive(true);

        if (vehicleSelectionMenu != null)
            vehicleSelectionMenu.SetActive(false);

        if (player != null)
            player.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseCustomization()
    {
        if (GameSession.SelectedVehicle != null)
            DataPersistenceManager.instance.SaveGameFor(GameSession.SelectedVehicle.vehicleId);

        if (currentInstance != null)
            Destroy(currentInstance);

        currentInstance = null;
        CarInstance.instance = null;

        customizationPanel.SetActive(false);

        if (vehicleSelectionMenu != null)
            vehicleSelectionMenu.SetActive(false);

        if (player != null)
            player.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // finds every IVehicleDependent in the scene, no manual list to maintain
    void AssignControllerToUpgradeComponents(RCCP_CarController controller)
    {
        var dependents = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IVehicleDependent>();

        foreach (IVehicleDependent d in dependents)
            d.SetController(controller);
    }
}