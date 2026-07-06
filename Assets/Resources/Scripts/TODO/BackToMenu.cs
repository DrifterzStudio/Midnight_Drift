using System.Collections;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackToMenu : MonoBehaviour
{
    private bool isChangingScene = false;
    private void Update()
    {


        if (isChangingScene) return;

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                
                Debug.Log("Transitioning to Menu");
                transitionToMenu();
            }
        }
    }
    [Server]
    void transitionToMenu()
    {
    
        Mirror_Manager.Instance.StopHost();
    }
    
}
