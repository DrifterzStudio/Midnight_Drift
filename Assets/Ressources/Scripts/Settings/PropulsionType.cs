using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class PropulsionType : MonoBehaviour, IDataPersistence {

    public DataPersistenceManager dataPersistence;

    public string dataFileName;

    public RCCP_CarController controller;

    [Header("Drive Wheels Type")]
    [Tooltip("DWT Button.")]
    public Button DWTButton;
    [Tooltip("DWT Text.")]
    public Text DWTText;

    private int driveType = 1;
    public bool fIsSteer = true;
    public bool fIsHandbrake = false;
    public bool rIsSteer = false;
    public bool rIsHandbrake = true;

    public static PropulsionType instance;


    public void LoadGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            controller.FrontAxle.isSteer = tmp.frontAxleSteer;
            controller.FrontAxle.isHandbrake = tmp.frontAxleHandbrake;
            controller.RearAxle.isSteer = tmp.rearAxleSteer;
            controller.RearAxle.isHandbrake = tmp.rearAxleHandbrake;
        }
    }

    public void SaveGame(IGameData data) {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null) {
            tmp.frontAxleSteer = controller.FrontAxle.isSteer;
            tmp.frontAxleHandbrake = controller.FrontAxle.isHandbrake;
            tmp.rearAxleSteer = controller.RearAxle.isSteer;
            tmp.rearAxleHandbrake = controller.RearAxle.isHandbrake;
        }
    }

    public string getDataFileName() {
        return dataFileName;
    }





    private void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);

        DWTButton.onClick.AddListener(OnDWTButtonClicked);
    }

    private void Update() {
        instance = this;

        if (driveType == 0) {
            DWTText.text = "Front wheels drive";
            controller.FrontAxle.isSteer = true;
            fIsSteer = true;
            controller.FrontAxle.isHandbrake = true;
            fIsHandbrake = true;
            controller.RearAxle.isSteer = false;
            rIsSteer = false;
            controller.RearAxle.isHandbrake = false;
            rIsHandbrake = false;
        }
        if (driveType == 1) {
            DWTText.text = "Rear wheels drive"; 
            controller.FrontAxle.isSteer = true;
            fIsSteer = true;
            controller.FrontAxle.isHandbrake = false;
            fIsHandbrake = false;
            controller.RearAxle.isSteer = false;
            rIsSteer = false;
            controller.RearAxle.isHandbrake = true;
            rIsHandbrake = true;
        }
        if (driveType == 2) {
            DWTText.text = "All wheels drive";
            controller.FrontAxle.isSteer = true;
            fIsSteer = true;
            controller.FrontAxle.isHandbrake = true;
            fIsHandbrake = true;
            controller.RearAxle.isSteer = true;
            rIsSteer = true;
            controller.RearAxle.isHandbrake = true;
            rIsHandbrake = true;
        }

    }

    private void OnDWTButtonClicked() {
        if (driveType + 1 > 2) driveType = 0;
        else driveType += 1;
        
    }

    private void OnDestroy() {
         DWTButton.onClick.RemoveAllListeners();
    }

}
