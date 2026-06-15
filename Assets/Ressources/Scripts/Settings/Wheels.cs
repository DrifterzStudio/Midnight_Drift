using UnityEngine;
using UnityEngine.UI;

public class Wheels : MonoBehaviour {

    // camber (front / rear) / steering sansibility 
    // steering curve / grip (forward / sideways)
    [Tooltip("Camber button.")]
    public Button camberButton;

    [Tooltip("Camber.")]
    public GameObject camber;

    [Tooltip("Grip button.")]
    public Button gripButton;

    [Tooltip("Grip.")]
    public GameObject grip;

    [Tooltip("Steering sansibily button.")]
    public Button steeringSensitivityButton;

    [Tooltip("Steering curve button.")]
    public Button steeringCurveButton;

    /*[Tooltip("Text showing the current state of differential.")]
    public Text camberText;*/


    private void Awake() {
        camberButton.onClick.AddListener(OnCamberButtonClicked);
        steeringSensitivityButton.onClick.AddListener(OnSteerSensitivityButtonClicked);
        steeringCurveButton.onClick.AddListener(OnSteerCurveButtonClicked);
        gripButton.onClick.AddListener(OnGripButtonClicked);
    }
    private void Update() {
        
    }

    private void OnCamberButtonClicked() {
        gameObject.SetActive(false);
        camber.SetActive(true);
    }
    private void OnSteerSensitivityButtonClicked() { } // carcontroller
    private void OnSteerCurveButtonClicked() { } // carcontroller
    private void OnGripButtonClicked() {
        gameObject.SetActive(false);
        camber.SetActive(true);
    }

    private void OnDestroy() { }
}
