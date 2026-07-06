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
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("Transitioning to Menu");
                transitionToMenu();
            }
        }
    }
    [Server]
    void transitionToMenu()
    {
        Scene_Controller.Instance.NewTransition()
            .Load("Menu", "Menu",true)
            .Unload("Multi_Server")
            .Unload("Multi_Game")
            .EnableOverlay(true)
            .Execute();
        Mirror_Manager.Instance.StopHost();
    }
    
}
