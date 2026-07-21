using UnityEngine;

// spawns the car the player picked in the garage and hands it to RCCP as the player vehicle.
// drop this in Circuit_Solo instead of placing a car by hand, so the choice actually changes what
// you drive. the spawned prefab brings its own loaders (LoadUpgrades/LoadCarModification/LoadCustom),
// so its tuning follows automatically.
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

        // keep GameSession in sync with what we spawn - the mass upgrades read it for the prefab's
        // stock values, and it's null when launching this scene directly
        GameSession.SelectVehicle(vehicle);

        Transform where = spawnPoint != null ? spawnPoint : transform;
        GameObject car = Instantiate(vehicle.prefab, where.position, where.rotation);

        RCCP_CarController controller = car.GetComponentInChildren<RCCP_CarController>(true);

        if (controller == null)
        {
            Debug.LogError("VehicleSpawner: spawned prefab has no RCCP_CarController.", car);
            return;
        }

        // becomes activePlayerVehicle, which the RCCP camera, RaceManager and Score all follow
        RCCP.RegisterPlayerVehicle(controller, true, true);
    }
}
