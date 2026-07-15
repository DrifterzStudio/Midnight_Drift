using UnityEngine;
using UnityEngine.UI;

public class AddTurbo : MonoBehaviour, IDataPersistence, IVehicleDependent {

    public string dataFileName;

    public RCCP_CarController controller;

    public Text turboNumber;

    public bool turbo1 = false;
    public bool turbo2 = false;
    public static AddTurbo instance;


    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            tmp.turbo1 = turbo1;
            tmp.turbo2 = turbo2;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            turbo1 = tmp.turbo1;
            turbo2 = tmp.turbo2;
        }
    }

    public string getDataFileName() {
        return dataFileName;
    }

    public void SetController(RCCP_CarController newController) {
        controller = newController;
        if (turbo2) controller.Engine.turbo2Charged = true;
        else if (turbo1) controller.Engine.turbo1Charged = true;
    }

    private void Awake() {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);
    }

    private void Update() {
        if (turbo2) turboNumber.text = "2";        
        else if (turbo1) turboNumber.text = "1";        
        else turboNumber.text = "0";        
    }

    public void onButtonClicked() {
        if (turbo1) {
            controller.Engine.turbo2Charged = true;
            turbo2 = true;
        }
        else {
            controller.Engine.turbo1Charged = true;
            turbo1 = true;
        }
    }
}
