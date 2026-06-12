using Unity.VisualScripting;
using UnityEngine;

public class MenuEvents : MonoBehaviour
{
   public void Quit()
    {
        Application.Quit();
    }
    public void ToGame()
    {
        Scene_Controller.Instance.NewTransition()
            .Load("Server","ServerScene")
        .Load("Game", "LobbyScene", true)
        .Unload("Menu")
        .EnableOverlay(true)
        .Execute();
    }
}
