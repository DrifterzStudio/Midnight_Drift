using Mirror;
using Steamworks;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BackToMenu : Singleton_Obj<BackToMenu>
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

   public void transitionToMenuServer()
    {
        if (isChangingScene) return;
        Debug.Log("Appel de StopClient()");
        if (NetworkServer.active)
        {
            Mirror_Manager.Instance.StopHost();
        }
        else if (NetworkClient.active)
        {
            Mirror_Manager.Instance.StopClient();
        }
      
         transitionToMenu();


        isChangingScene = true;
    }


    void transitionToMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        if (LoadingScreenManager.Instance != null)
            LoadingScreenManager.Instance.LoadScene("MainMenu");
        else
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        Cursor.visible = true;
    }


}
