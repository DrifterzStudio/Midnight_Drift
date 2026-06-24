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
    private static NetworkCamera  _localInstance = null;
    private static List<RCCP_CarController> _vehicles = new();

    private RCCP_Camera cam;
    private RCCP_CarController _activeCar = new();
    private int _currentTarget = 0;
    private bool _isPlayerActive = false;
   
    private void Awake() 
    {
        cam = FindAnyObjectByType<RCCP_Camera>();
        if (cam == null)
            Debug.LogWarning("error cam null");

    }

    public override void OnStartClient()
    {
        
        base.OnStartClient();

        // sécurité si la valeur est déjà là au moment du spawn (ordre favorable)
        ulong existingId = GetComponent<PlayerInfos>().SteamId;
        if (existingId != 0)
            OnSteamIdReady(existingId);
    
}

    public void OnSteamIdReady(ulong steamId)
    {
        if (_steamIdProcessed) return;
        _steamIdProcessed = true;

        if (isLocalPlayer)
        {
            _localInstance = this;
            _isPlayerActive = ActivePlayer_List.Instance.Contains(steamId);

            if (_isPlayerActive)
            {
                _vehicles.Clear();
                cam.SetTarget(GetComponent<RCCP_CarController>());
            }
            return;
        }

        if (_localInstance != null && _localInstance._isPlayerActive)
            return;

        if (!ActivePlayer_List.Instance.Contains(steamId))
            return;

        _vehicles.Add(GetComponent<RCCP_CarController>());
        if (_vehicles.Count == 1)
        {
            cam.SetTarget(_vehicles[0]);
            _activeCar = _vehicles[0];
        }
    }


    private void Update()
    {
        if (!isLocalPlayer)
           return;
        if (Keyboard.current.uKey.wasPressedThisFrame && !_isPlayerActive)
        {
            Debug.Log("changement de tutut");
            SwitchCam();
        }
    }

    private void SwitchCam()
    {
        if(_isPlayerActive)
            Debug.LogWarning("error change cam request on active Player ");
        if(_vehicles.Count == 0)
            Debug.LogWarning("error no cam found ");
        _currentTarget++;

        if (_currentTarget >= _vehicles.Count)
            _currentTarget = 0;
        Debug.Log(_currentTarget);
        Debug.LogWarning(_vehicles.Count);

        RCCP_CarController targetCar = _vehicles[_currentTarget];
        cam.cameraTarget.playerVehicle = targetCar;
        _activeCar = targetCar;
        Debug.Log("Caméra maintenant sur : " + targetCar.name);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        HandleDisconnect();
    }
    private void HandleDisconnect()
    {
        if (!ActivePlayer_List.Instance.Contains(GetComponent<PlayerInfos>().SteamId))
            return;

        if (isLocalPlayer)
            ActivePlayer_List.Instance.CmdRemove(GetComponent<PlayerInfos>().SteamId);

        if (_localInstance._isPlayerActive)
            return;

        var car = GetComponent<RCCP_CarController>();
        _vehicles.Remove(car);

        if (_vehicles.Count == 0)
        {
            Debug.Log("Plus aucune voiture");
            return;
        }

        if (_localInstance._activeCar == car)
        {
            _localInstance._currentTarget = 0;
            _localInstance.cam.SetTarget(_vehicles[0]);
            _localInstance._activeCar = _vehicles[0];
        }
        if(_vehicles.Count == 0)
        {
            Debug.LogError("no player found force set");
            _localInstance.cam.SetTarget(_localInstance.gameObject.GetComponent<RCCP_CarController>());
        }
    }
}