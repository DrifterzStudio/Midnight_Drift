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
        .Load("Game", "Tom_Scene", true)
        .Unload("Menu")
        .EnableOverlay(true)
        .Execute();
    }
}
