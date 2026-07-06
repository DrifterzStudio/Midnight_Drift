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
            if (NetworkServer.active)
            {
                Debug.Log("Leave Server");
                transitionToMenuServer();
            }
            else if (!NetworkServer.active)
            {
                transitionToMenu();
            }
        }
          
    }
    void transitionToMenuServer()
    {

         if (NetworkClient.active && !NetworkServer.active)
            {
                Mirror_Manager.Instance.StopClient();
                isChangingScene = true;
            }   
            else if (NetworkServer.active)
            {
                Mirror_Manager.Instance.StopHost();
                isChangingScene = true;
            }
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
