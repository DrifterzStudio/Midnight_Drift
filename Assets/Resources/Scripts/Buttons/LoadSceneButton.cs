using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{

    public Button button;

    public string sceneName;
    public bool isAdditive;

    private void Start() {
        button.onClick.AddListener(OnButtonClicked);

    }

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

    private void OnButtonClicked() {
        ChangeScene();
    }

}
