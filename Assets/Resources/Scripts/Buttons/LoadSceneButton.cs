using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{

    public Button button;

    public string sceneName;
    public bool isAdditive;

    // stops a double load when both the inspector onClick and the Start listener fire
    private int _lastLoadFrame = -1;

    private void Start()
    {
        button.onClick.AddListener(OnButtonClicked);

    }

    public void ChangeScene()
    {
        if (_lastLoadFrame == Time.frameCount)
            return;
        _lastLoadFrame = Time.frameCount;

        if(isAdditive)
        {
            SceneManager.LoadScene(sceneName,LoadSceneMode.Additive);
            return;
        }

        // single loads go through the loading screen, plain load as a fallback
        if (LoadingScreenManager.Instance != null)
            LoadingScreenManager.Instance.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName,LoadSceneMode.Single);

    }

    private void OnButtonClicked()
    {
        ChangeScene();
    }

}
