using Mirror;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class NetworkCamera : NetworkBehaviour
{
    private static NetworkCamera  _localInstance = null;
    private static readonly List<RCCP_CarController> _vehicles = new();

    private RCCP_Camera cam;

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
        if (isLocalPlayer)
        {
            _localInstance = this;
            _localInstance._isPlayerActive = ActivePlayer_List.Instance.PlayerIdSteam.Contains(SteamFriends.GetPersonaName());
            if (_localInstance._isPlayerActive)
            {
                _vehicles.Clear();
                cam.SetTarget(GetComponent<RCCP_CarController>());
            }
            return;
        }
        if(_localInstance != null && _localInstance._isPlayerActive)
            return;

        if(!ActivePlayer_List.Instance.PlayerIdSteam.Contains(GetComponent<PlayerName>().GetName()))
            return;

        _vehicles.Add(GetComponent<RCCP_CarController>());
        if (_vehicles.Count == 1)
        {
            cam.SetTarget(_vehicles[0]);
        }
    }

    private void Update()
    {
       if(!isLocalPlayer)
           return;

        if (Keyboard.current.uKey.wasPressedThisFrame && !_isPlayerActive)
        {
            Debug.Log("changement de tutur");
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
        Debug.Log("Caméra maintenant sur : " + targetCar.name);
    }
}