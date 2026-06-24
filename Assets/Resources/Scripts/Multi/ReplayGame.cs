using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReplayGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    [ServerCallback]
    private void Update()
    {
        if (Keyboard.current.rKey.IsPressed())
        {
            Mirror_Manager.Instance.ChangeScene("Game", "GameLobbyScene");
        }
    }
}
