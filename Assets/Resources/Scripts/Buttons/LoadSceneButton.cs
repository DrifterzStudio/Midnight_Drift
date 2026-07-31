using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{

    public Button button;

    public string sceneName;
    public bool isAdditive;

    [Tooltip("If set, opens this menu instead of loading right away. The menu loads the scene once a car is chosen.")]
    public CarSelectionMenu carSelectionMenu;

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

        // pick a car first if a menu is set - it loads the scene once a car is chosen
        if (carSelectionMenu != null)
        {
            carSelectionMenu.Open();
            return;
        }

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
