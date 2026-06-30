using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class ReplayGame : MonoBehaviour
{
 
    // Update is called once per frame
    [ServerCallback]
    private void Update()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (Keyboard.current.rKey.IsPressed())
            {
                Mirror_Manager.Instance.ChangeScene("Game", "GameLobbyScene");
            }
        }
    }
}
