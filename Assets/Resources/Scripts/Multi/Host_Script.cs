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

        if (NetworkServer.active)
        {
            hostButton.SetActive(false);
        }

        startButton.SetActive(false);

    }
    public void OnClick()
    {
        if(alreadyPresseed)
            return;
        alreadyPresseed = true;
        Steam_Lobby.Instance.CreateLobby();
        hostButton.SetActive(false);
        startButton.SetActive(true);
        isReady = true;
    }

    private void Update()
    {
        if(isReady && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Mirror_Manager.Instance.ChangeScene("Multi_Game", "MultiGameLobbyScene");
            Steam_Lobby.Instance.LockLobby();
        }

    }
}
