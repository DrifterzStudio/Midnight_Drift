using UnityEngine;
using UnityEngine.Rendering;

public class Differential : SaveUpgrades, IDataPersistence {

    public RCCP_CarController controller;

    public static Differential instance;

    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            //tmp.lightenedMass = lightenedMass;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            //lightenedMass = tmp.lightenedMass;
        }
    }

    public string getDataFileName() {
        return "";
    }


    void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);
    }

    public void OnButtonClicked() {

    }
}
