using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Host_Script : MonoBehaviour
{
    private bool isReady = false;
    private bool alreadyPresseed = false;
    [SerializeField] private GameObject hostButton;
    [SerializeField] private GameObject startButton;
    private void Start()
    {
        Mirror_Manager.Instance.RegisterPrefab("MultiGameScene", Resources.Load<GameObject>("Prefabs/Multi/PlayerCar"));
        Mirror_Manager.Instance.RegisterPrefab("MultiGameLobbyScene", Resources.Load<GameObject>("Prefabs/Multi/PlayerFps"));
        startButton.SetActive(false);
    }

    public void OnClick()
    {
        if(alreadyPresseed)
            return;
        alreadyPresseed = true;
        Steam_Lobby.Instance.CreateLobby();
        startButton.SetActive(true);
    }

    public void OnClickStart()
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("Serveur pas encore prêt, réessaie dans un instant.");
            return;
        }

        Mirror_Manager.Instance.ChangeScene("Multi_Game", "MultiGameLobbyScene");
        Steam_Lobby.Instance.LockLobby();
    }

    public void OnClickExit()
    {
        Steam_Lobby.Instance.LeaveLobby();
        Scene_Controller.Instance.NewTransition()
            .Load("Menu", "Menu", true)
            .Unload("Multi_Server")
            .Unload("Multi_Game")
            .EnableOverlay(true)
            .Execute();
    }
}