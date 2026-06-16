using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Host_Script : MonoBehaviour
{
    private bool isReady = false;
    private bool alreadyPresseed = false;

    private void Start()
    {
        Mirror_Manager.Instance.RegisterPrefab("GameScene", Resources.Load<GameObject>("Prefabs/MyPrefab"));
        Mirror_Manager.Instance.RegisterPrefab("GameLobbyScene", Resources.Load<GameObject>("Prefabs/Player"));

    }
    public void OnClick()
    {
        if(alreadyPresseed)
            return;
        alreadyPresseed = true;
        Steam_Lobby.Instance.CreateLobby();
        isReady = true;
    }

    private void Update()
    {
        if(isReady && Keyboard.current.spaceKey.wasPressedThisFrame)
            Mirror_Manager.Instance.ChangeScene("Game","GameLobbyScene");

    }
}
