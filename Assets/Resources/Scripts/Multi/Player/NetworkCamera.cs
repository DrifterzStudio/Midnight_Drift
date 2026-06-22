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
    private static List<RCCP_CarController> _vehicles = new();

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
        base.OnStartClient();

       // Debug.LogWarning(GetComponent<PlayerName>()._steamName);
        if (isLocalPlayer)
        {
            _localInstance = this;
            _isPlayerActive = ActivePlayer_List.Instance.PlayerSteamId.Contains(SteamUser.GetSteamID().m_SteamID);
            if (_isPlayerActive)
            {
                Debug.LogWarning("pas sensé arriver la ");
                _vehicles.Clear();
                cam.SetTarget(GetComponent<RCCP_CarController>());
            }
            return;
        }
        Debug.LogWarning(SteamUser.GetSteamID().m_SteamID);
        //if(_localInstance != null && _localInstance._isPlayerActive)
        //    return;

        if (!ActivePlayer_List.Instance.PlayerSteamId.Contains(SteamUser.GetSteamID().m_SteamID))
            return;

    
        Debug.LogWarning("ADD");
        _vehicles.Add(GetComponent<RCCP_CarController>());
        
        if (_vehicles.Count == 1)
        {
            Debug.LogWarning("set Cam");
            cam.SetTarget(_vehicles[0]);
        }
    }

    private void Update()
    {
       
        if (!isLocalPlayer)
           return;
      
        Debug.Log(_isPlayerActive);

        
        
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