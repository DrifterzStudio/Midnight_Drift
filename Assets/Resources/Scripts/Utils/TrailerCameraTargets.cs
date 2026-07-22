using Unity.Cinemachine;
using UnityEngine;

// points the trailer cameras at the car once it spawns (it spawns at runtime, so it can't be dragged
// into the cameras in the editor). leave their Tracking Target empty.
public class TrailerCameraTargets : MonoBehaviour
{
    [Tooltip("Every Cinemachine camera that should follow/look at the player car.")]
    public CinemachineCamera[] cameras;

    private bool _bound;

    void Update()
    {
        if (_bound)
            return;

        // wait until the car is spawned and registered as the RCCP player
        if (RCCP_SceneManager.Instance == null)
            return;

        RCCP_CarController car = RCCP_SceneManager.Instance.activePlayerVehicle;
        if (car == null)
            return;

        foreach (CinemachineCamera cam in cameras)
        {
            if (cam == null)
                continue;

            cam.Target.TrackingTarget = car.transform;
            cam.Target.LookAtTarget = car.transform;
        }

        _bound = true;
    }
}
