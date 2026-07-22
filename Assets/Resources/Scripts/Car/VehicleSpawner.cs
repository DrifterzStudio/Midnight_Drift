using UnityEngine;

// spawns the car picked in the garage and gives it to RCCP. put it in Circuit_Solo instead of a
// hand-placed car.
public class VehicleSpawner : MonoBehaviour
{
    [Tooltip("Where the car spawns. Empty = this object's own transform.")]
    public Transform spawnPoint;

    [Tooltip("Car used when Circuit_Solo is launched directly, without going through the garage.")]
    public VehicleDefinition defaultVehicle;

    void Awake()
    {
        VehicleDefinition vehicle = GameSession.SelectedVehicle != null ? GameSession.SelectedVehicle : defaultVehicle;

        if (vehicle == null || vehicle.prefab == null)
        {
            Debug.LogError("VehicleSpawner: nothing to spawn (no selection and no default vehicle set).", this);
            return;
        }

        // the mass upgrades read GameSession for the prefab's stock values
        GameSession.SelectVehicle(vehicle);

        Transform where = spawnPoint != null ? spawnPoint : transform;
        GameObject car = Instantiate(vehicle.prefab, where.position, where.rotation);

        RCCP_CarController controller = car.GetComponentInChildren<RCCP_CarController>(true);

        if (controller == null)
        {
            Debug.LogError("VehicleSpawner: spawned prefab has no RCCP_CarController.", car);
            return;
        }

        // becomes activePlayerVehicle (camera, RaceManager and Score follow it)
        RCCP.RegisterPlayerVehicle(controller, true, true);
    }
}
