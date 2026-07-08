using UnityEngine;

public class Lightened : MonoBehaviour, IDataPersistence {

    public DataPersistenceManager dataPersistence;

    public string dataFileName;

    public RCCP_CarController controller;

    public int lightenedMass = -50;

    private bool isLightened = false;

    public static Lightened instance;

    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            tmp.lightenedMass = lightenedMass;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            lightenedMass = tmp.lightenedMass;
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
        if (!isLightened) {
            controller.Rigid.mass = lightenedMass;
            isLightened = true;
        }
    }
}
