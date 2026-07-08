using UnityEngine;
using UnityEngine.UI;

public class Slick : MonoBehaviour, IDataPersistence {

    public DataPersistenceManager dataPersistence;

    public string dataFileName;

    public RCCP_CarController controller;

    public float slickness = 10f;

    public Text slick;

    public static Slick instance;

    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            tmp.slickness = slickness;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            slickness = tmp.slickness;
        }
    }

    public string getDataFileName() {
        return dataFileName;
    }

    private void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);
}

    private void Update() {
        if (slickness == 10) slick.text = "Normal";
        else if (slickness == 5) slick.text = "Semi-Slick"; 
        else if (slickness == 1) slick.text = "Slick"; 
    }

    public void OnButtonClicked() {
        if (slickness == 10) {
            slickness = 5f;
        }
        else if (slickness == 5f) {
            slickness = 1f;
        }
        controller.AeroDynamics.wheelResistance = slickness;
    }
}
