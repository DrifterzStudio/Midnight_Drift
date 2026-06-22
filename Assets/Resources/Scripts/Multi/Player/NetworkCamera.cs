using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class NetworkCamera : MonoBehaviour
{
    private RCCP_Camera cam;

    private List<RCCP_CarController> vehicles = new();
    private int currentTarget = -1;


    private void Start()
    {
        cam = FindAnyObjectByType<RCCP_Camera>();

        if (cam == null)
        {
            Debug.LogError("Aucune cam");
            enabled = false;
            return;
        }

        RefreshVehicles();
    }

    private void Update()
    {
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            Debug.Log("changement de tutur");
            SwitchCam();
        }
    }

    private void RefreshVehicles()
    {
        vehicles.Clear();
        RCCP_CarController[] foundVehicles = FindObjectsByType<RCCP_CarController>();

        foreach (RCCP_CarController vehicle in foundVehicles)
        {
            vehicles.Add(vehicle);
            Debug.Log("tutur trouvé : " + vehicle.name);
        }
    }

    private void SwitchCam()
    {
        if (vehicles.Count == 0)
        {
            RefreshVehicles();
        }
        if (vehicles.Count == 0)
        {
            Debug.LogWarning("Pas de tutur");
            return;
        }

        currentTarget++;

        if (currentTarget >= vehicles.Count)
            currentTarget = 0;


        RCCP_CarController targetCar = vehicles[currentTarget];
        cam.cameraTarget.playerVehicle = targetCar;
        Debug.Log("Caméra maintenant sur : " + targetCar.name);
    }
}