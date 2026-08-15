using UnityEngine;
using UnityEngine.SceneManagement;

public class On_Start_Core : MonoBehaviour
{
  
    void Start()
    {
        SceneManager.LoadScene("Core", LoadSceneMode.Additive);
    }

 public void OnClick()
    {
        SceneManager.UnloadScene("MainMenu");
        Scene_Controller.Instance.NewTransition()
         .Load("Multi_Server", "MultiServerScene")
         .Load("Multi_Game", "MultiLobbyScene", true)
         .EnableOverlay(true)
         .Execute();
    }
}
