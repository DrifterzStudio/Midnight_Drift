using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    public RCCP_CarController car;

    public Button button;

    public string sceneName;
    public bool isAdditive;

    private bool isTelemetry;

    private void Start() {
        button.onClick.AddListener(OnButtonClicked);
        

        if (car != null) {
            if (SaveSetttings.vehiculeSettings == null) {
                SaveSetttings.vehiculeSettings = car;
            }

            else {
                car = SaveSetttings.vehiculeSettings;
            }
        }

        isTelemetry = SaveSetttings.telemetrySettings;
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

    public bool IsTelemetry() {
        return isTelemetry; 
    }

}
