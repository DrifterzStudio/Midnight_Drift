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
        Debug.Log("Appel de StopClient()");
        if (NetworkClient.active && !NetworkServer.active)
            {
             Transport.active.ClientDisconnect();
             Mirror_Manager.Instance.StopClient();
                isChangingScene = true;
            }   
            else if (NetworkServer.active)
            {
                Mirror_Manager.Instance.StopHost();
                isChangingScene = true;
            }
        }
<<<<<<< HEAD
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

=======
    
>>>>>>> parent of 68f12034 ([Kroktur] test de la mort)

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
