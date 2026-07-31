using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void ToMainMenu()
    {
        if (LoadingScreenManager.Instance != null)
            LoadingScreenManager.Instance.LoadScene("MainMenu");
        else
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
