using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class ReplayGame : NetworkBehaviour
{
 
    // Update is called once per frame
    [ServerCallback]
    private void Update()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (Keyboard.current.rKey.IsPressed())
            {


                ActivePlayer_List.Instance.PlayerSteamId.Clear();
                Mirror_Manager.Instance.ChangeScene("Game", "GameLobbyScene");
            }
        }
    }
}
