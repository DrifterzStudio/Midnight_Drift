using Mirror;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class NetworkCamera : NetworkBehaviour
{
    private bool _steamIdProcessed = false;
    public static NetworkCamera  LocalInstance = null;
    private static List<RCCP_CarController> _vehicles = new();

    private RCCP_Camera cam;
    public RCCP_CarController ActiveCar ;
    private int _currentTarget = 0;
    public bool IsPlayerActive = false;
   
    private void Awake()
    {
        LocalInstance = null;
        _vehicles = new();
        cam = FindAnyObjectByType<RCCP_Camera>();
        if (cam == null)
            Debug.LogWarning("error cam null");

    }

    public override void OnStartClient()
    {
        
        base.OnStartClient();

        //// sécurité si la valeur est déjà là au moment du spawn (ordre favorable)
        //ulong existingId = GetComponent<PlayerInfos>().SteamId;
        //if (existingId != 0)
        //     OnSteamIdReady(existingId);
    
}

    public void OnSteamIdReady(ulong steamId)
    {
          
        if (_steamIdProcessed) return;
        _steamIdProcessed = true;
      
        if (isLocalPlayer)
        {
            LocalInstance = this;
            IsPlayerActive = ActivePlayer_List.Instance.Contains(steamId);

            if (IsPlayerActive)
            {
                _vehicles.Clear();
                LocalInstance.cam.SetTarget(GetComponent<RCCP_CarController>());
            }

            if (_vehicles.Count != 0)
            {
                LocalInstance.cam.SetTarget(_vehicles[0]);
                LocalInstance.ActiveCar = _vehicles[0];
            }
            return;
        }

        if (LocalInstance != null && LocalInstance.IsPlayerActive)
            return;

        if (!ActivePlayer_List.Instance.Contains(steamId))
            return;

        _vehicles.Add(GetComponent<RCCP_CarController>());

        if (LocalInstance != null && _vehicles.Count == 1)
        {
            LocalInstance.cam.SetTarget(_vehicles[0]);
            LocalInstance.ActiveCar = _vehicles[0];
        }
    }


    private void Update()
    {

        if (!isLocalPlayer)
           return;



        if (Keyboard.current.uKey.wasPressedThisFrame && !IsPlayerActive)
        {
            Debug.Log("changement de tutut");
            SwitchCam();
        }
    }

    private void SwitchCam()
    {
        if(IsPlayerActive)
            Debug.LogWarning("error change cam request on active Player ");
        if(_vehicles.Count == 0)
            Debug.LogWarning("error no cam found ");
        _currentTarget++;

        if (_currentTarget >= _vehicles.Count)
            _currentTarget = 0;
        Debug.Log(_currentTarget);
        Debug.LogWarning(_vehicles.Count);

        RCCP_CarController targetCar = _vehicles[_currentTarget];
        if(targetCar == null)
            Debug.LogError("nulllll");
        LocalInstance.cam.cameraTarget.playerVehicle = targetCar;
        LocalInstance.ActiveCar = targetCar;
        Debug.Log("Caméra maintenant sur : " + targetCar.name);
    }

    

    public override void OnStopClient()
    {
        base.OnStopClient();
        HandleDisconnect();
    }
    private void HandleDisconnect()
    {
        Debug.Log("disconnect");

       

        if (!ActivePlayer_List.Instance.Contains(GetComponent<PlayerInfos>().SteamId))
            return;

        if (LocalInstance.IsPlayerActive)
            return;

        var car = GetComponent<RCCP_CarController>();
        _vehicles.Remove(car);

        if (_vehicles.Count == 0)
        {
            Debug.LogError("no player found force set");
            LocalInstance.IsPlayerActive = true;
            LocalInstance.cam.SetTarget(LocalInstance.gameObject.GetComponent<RCCP_CarController>());
        }

        if (LocalInstance.ActiveCar == car && _vehicles.Count!= 0)
        {
            LocalInstance._currentTarget = 0;
            LocalInstance.cam.SetTarget(_vehicles[0]);
            LocalInstance.ActiveCar = _vehicles[0];
        }

    }
}