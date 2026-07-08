using UnityEngine;
using UnityEngine.UI;

public class AntiRollBar : MonoBehaviour, IDataPersistence {

    public DataPersistenceManager dataPersistence;

    public string dataFileName;

    public RCCP_CarController controller;

    public Text ARBText;

    public int antiRollBarValue = 500;

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
        return dataFileName;
    }

    void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);
    }

    private void Update() {
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
    }
}
