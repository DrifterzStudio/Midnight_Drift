using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class Host_Script : MonoBehaviour
{
    private bool isReady = false;
    private bool alreadyPresseed = false;
    
    private void Start()
    {
        Mirror_Manager.Instance.RegisterPrefab("MultiGameScene", Resources.Load<GameObject>("Prefabs/Multi/PlayerCar"));
        Mirror_Manager.Instance.RegisterPrefab("MultiGameLobbyScene", Resources.Load<GameObject>("Prefabs/Multi/PlayerFps"));

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
        {
            Mirror_Manager.Instance.ChangeScene("Multi_Game", "MultiGameLobbyScene");
            Steam_Lobby.Instance.LockLobby();
        }

    }
}
