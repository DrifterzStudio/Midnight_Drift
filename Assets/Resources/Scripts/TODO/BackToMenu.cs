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

        //TODO changement classique
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
                Debug.Log("Leave Server");
                transitionToMenuServer();
        }
          
    }

    void transitionToMenuServer()
    {
        Debug.Log("Appel de StopClient()");
        if (NetworkServer.active)
        {
            // Host ou serveur dédié
            Mirror_Manager.Instance.StopHost();
        }
        else if (NetworkClient.active)
        {
            // Client uniquement
            Mirror_Manager.Instance.StopClient();
        }
        else
        {
            transitionToMenu();
        }

        isChangingScene = true;
    }


    void transitionToMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Scene_Controller.Instance.NewTransition()
            .Load("Menu", "Menu", true)
            .Unload("Multi_Server")
            .Unload("Multi_Game")
            .EnableOverlay(true)
            .Execute();
    }


}
