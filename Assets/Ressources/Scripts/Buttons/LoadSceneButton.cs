using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public string sceneName;
    public bool isAdditive;

    public void ChangeScene()
    {
        if(isAdditive)
        {
            SceneManager.LoadScene(sceneName,LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.LoadScene(sceneName,LoadSceneMode.Single);
        }
    }
}
