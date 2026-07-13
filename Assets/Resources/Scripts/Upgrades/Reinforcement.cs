using UnityEngine;

public class Reinforcement : MonoBehaviour, IDataPersistence {

    public DataPersistenceManager dataPersistence;

    public string dataFileName;

    public RCCP_CarController controller;

    public float newYCenter = -0.5f;

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
        return dataFileName;
    }

    void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);
}

    public void OnButtonClicked() {
        if (!isNewYCenter) {
            controller.AeroDynamics.COM.transform.up.Set(controller.AeroDynamics.COM.transform.up.x, newYCenter, controller.AeroDynamics.COM.transform.up.z);
            isNewYCenter = true;
        }
    }
}
