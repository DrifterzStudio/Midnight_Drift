using UnityEngine;
using UnityEngine.UI;

public class Brake : SaveUpgrades, IDataPersistence {

    public RCCP_CarController controller;

    public Text brakePowerText;
    
    public static Brake instance;


    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            tmp.brakePower = brakePower;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            brakePower = tmp.brakePower;
        }
    }

    public string getDataFileName() {
        return "";
    }


    void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);
        brakePower = 0.15f;
}

    private void Update() {
        if (brakePower == 0.15f) brakePowerText.text = "Normal";
        else if (brakePower == 0.17f) brakePowerText.text = "Upgraded";
        else if (brakePower == 0.2f) brakePowerText.text = "Max";
    }

    public void OnButtonClicked() {
        if (brakePower == 0.15f) {
            brakePower = 0.17f;
        }
        else if (brakePower == 0.17f) {
            brakePower = 0.2f;
        }
        controller.Engine.engineBrakingCoefficient = brakePower;
    }
}
