using UnityEngine;

public class AddTurbo : MonoBehaviour, IDataPersistence {

    public RCCP_CarController controller;

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
        return "";
    }

    private void Awake() {
        if (instance == null) instance = this;
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
