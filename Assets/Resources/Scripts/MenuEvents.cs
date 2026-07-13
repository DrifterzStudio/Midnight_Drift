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
            .Load("Multi_Server","MultiServerScene")
            .Load("Multi_Game", "MultiLobbyScene", true)
            .Unload("Menu")
            .EnableOverlay(true)
            .Execute();
    }
}
