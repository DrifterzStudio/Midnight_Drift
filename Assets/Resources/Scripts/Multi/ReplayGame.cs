using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class ReplayGame : NetworkBehaviour
{

    // Update is called once per frame
    private bool isChangingScene = false;

    [ServerCallback]
    private void Update()
    {
        if (isChangingScene) return;

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                isChangingScene = true;
                ActivePlayer_List.Instance.PlayerSteamId.Clear();
                Mirror_Manager.Instance.ChangeScene("Game", "GameLobbyScene");
            }
        }
    }
}
