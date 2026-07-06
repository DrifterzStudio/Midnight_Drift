using UnityEngine;
using UnityEngine.UI;

public class AntiRollBar : SaveUpgrades, IDataPersistence {
    
    public RCCP_CarController controller;

    public Text ARBText;


    public static AntiRollBar instance;


    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            tmp.antiRollBarValue = antiRollBarValue;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            antiRollBarValue = tmp.antiRollBarValue;
        }
    }

    public string getDataFileName() {
        return "";
    }

    void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);
        antiRollBarValue = 500;
        ARBText.text = antiRollBarValue.ToString();
    }

    public void OnButtonClicked() {
        if (antiRollBarValue == 500) {
            antiRollBarValue = 1000;
        }
        else if (antiRollBarValue == 1000) {
            antiRollBarValue = 1500;
        }
        controller.FrontAxle.antirollForce = antiRollBarValue;
        controller.FrontAxle.antirollForce = antiRollBarValue;
        ARBText.text = antiRollBarValue.ToString();
    }
}
