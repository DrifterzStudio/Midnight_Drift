using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class GearboxRatio : MonoBehaviour, IDataPersistence {

    public DataPersistenceManager dataPersistence;

    public string dataFileName;

    public RCCP_CarController controller;

    public Text ratioText;

    public float[] gearRatios;

    private int currentUpgrade = 0;
    private int currentUpgradeText = 0;

    public static GearboxRatio instance;


    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            tmp.gearRatios = gearRatios;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            gearRatios = tmp.gearRatios;
        }
    }

    public string getDataFileName() {
        return dataFileName;
    }

    private void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);
}

    void Update() {
        if (currentUpgrade < 4) {
            currentUpgradeText = currentUpgrade + 1;
            ratioText.text = currentUpgradeText.ToString();
        }
    }

    public void OnClick() {
        currentUpgrade += 1;
        if (currentUpgrade == 1) 
            controller.Gearbox.gearRatios = new float[] { 2.86f, 1.62f, 1.0f, .72f };
        if (currentUpgrade == 2) 
            controller.Gearbox.gearRatios = new float[] { 4.35f, 2.5f, 1.66f, 1.23f, 1.0f, .85f };
        if (currentUpgrade == 3) 
            controller.Gearbox.gearRatios = new float[] { 4.5f, 2.5f, 1.66f, 1.23f, 1.0f, .9f, .8f };
        if (currentUpgrade == 4) 
            controller.Gearbox.gearRatios = new float[] { 4.6f, 2.5f, 1.86f, 1.43f, 1.23f, 1.05f, .9f, .72f };
        gearRatios = controller.Gearbox.gearRatios;
    }
}
