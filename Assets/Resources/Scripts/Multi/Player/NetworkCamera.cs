using Mirror;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class NetworkCamera : NetworkBehaviour
{ 
    private RCCP_Camera cam;

    private List<RCCP_CarController> vehicles = new();
    private int currentTarget = -1;
    private bool isPlayerActive = false;
   
    private void Awake()
    {
        cam = FindAnyObjectByType<RCCP_Camera>();
        if (cam == null)
            Debug.LogWarning("error null");

    }

    public override void OnStartClient()
    {

        // si je suis le player local
        if (isLocalPlayer)
        {

            // demander si player est actif...

            // si le player est actif clear les vehicules et set la camera
            isPlayerActive = ActivePlayer_List.Instance.PlayerIdSteam.Contains(SteamFriends.GetPersonaName());

            if (isPlayerActive)
            {
                vehicles.Clear();
                RCCP_CarController controller = GetComponent<RCCP_CarController>();
                if(controller == null)
                    Debug.LogWarning("error null");
                cam.SetTarget(controller);
            }
            // sinon on return
            return;
        }

        // si le player n'est pas local 

        // si le player est actif pas besoin de regarder les camera
        if(isPlayerActive)
            return;

        // si vehicule non actif return
        if(!ActivePlayer_List.Instance.PlayerIdSteam.Contains(GetComponent<PlayerName>().GetName()))
            return;
        // si vehicule actif j'ajoute ca camera + je set si c'est le premier 
        vehicles.Add(GetComponent<RCCP_CarController>());
        if (vehicles.Count == 1)
        {
            cam.SetTarget(vehicles[0]);
        }
    }

    private void Update()
    {
       if(!isLocalPlayer)
           return;

        if (Keyboard.current.uKey.wasPressedThisFrame && !isPlayerActive)
        {
            Debug.Log("changement de tutur");
            SwitchCam();
        }
    }

    private void SwitchCam()
    {
        if(isPlayerActive)
            Debug.LogWarning("error change cam request on active Player ");
        if(vehicles.Count == 0)
            Debug.LogWarning("error no cam found ");
        currentTarget++;

        if (currentTarget >= vehicles.Count)
            currentTarget = 0;


        RCCP_CarController targetCar = vehicles[currentTarget];
        cam.cameraTarget.playerVehicle = targetCar;
        Debug.Log("Caméra maintenant sur : " + targetCar.name);
    }
}