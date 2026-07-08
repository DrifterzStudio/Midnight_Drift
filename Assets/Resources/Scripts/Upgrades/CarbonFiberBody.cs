using UnityEngine;

public class CarbonFiberBody : MonoBehaviour, IDataPersistence {

    public DataPersistenceManager dataPersistence;

    public string dataFileName;

    public RCCP_CarController controller;

    public int carbonMass = -200;

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
        return dataFileName;
    }

    void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);
}

    public void OnButtonClicked() {
        if (!isCarbon) {
            controller.Rigid.mass = carbonMass;
            isCarbon = true;
        }
    }
}
