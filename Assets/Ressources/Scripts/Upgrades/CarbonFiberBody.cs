using UnityEngine;

public class CarbonFiberBody : SaveUpgrades, IDataPersistence {

    public RCCP_CarController controller;

    private bool isCarbon = false;

    public static CarbonFiberBody instance;


    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            tmp.carbonMass = carbonMass;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            carbonMass = tmp.carbonMass;
        }
    }

    public string getDataFileName() {
        return "";
    }

    void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);
        carbonMass = -200;
}

    public void OnButtonClicked() {
        if (!isCarbon) {
            controller.Rigid.mass = carbonMass;
            isCarbon = true;
        }
    }
}
