using UnityEngine;

public class Reinforcement : SaveUpgrades, IDataPersistence {

    public RCCP_CarController controller;

    private bool isNewYCenter = false;

    public static Reinforcement instance;


    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            tmp.newYCenter = newYCenter;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            newYCenter = tmp.newYCenter;
        }
    }

    public string getDataFileName() {
        return "";
    }

    void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);
        newYCenter = -0.5f;
}

    public void OnButtonClicked() {
        if (!isNewYCenter) {
            controller.AeroDynamics.COM.transform.up.Set(controller.AeroDynamics.COM.transform.up.x, newYCenter, controller.AeroDynamics.COM.transform.up.z);
            isNewYCenter = true;
        }
    }
}
