using UnityEngine;
using UnityEngine.UI;

public class SuspensionUpgrade : SaveUpgrades, IDataPersistence {

    public RCCP_CarController controller;

    public Text typeText;

    private int suspensionIdx = 0;

    public static SuspensionUpgrade instance;

    public void SaveGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            tmp.newY = newY;
            tmp.tilts = tilts;
        }
    }

    public void LoadGame(IGameData data) {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null) {
            newY = tmp.newY;
            tilts = tmp.tilts;
        }
    }

    public string getDataFileName() {
        return "";
    }

    void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);
        newY = 0.16f;
        tilts = 5f;
        typeText.text = "Normal";
    }

    public void OnButtonClicked() {
        if (suspensionIdx < 2) suspensionIdx += 1;
        if (suspensionIdx == 0) {
            newY = 0.2f;
            typeText.text = "Normal";
        }
        else if (suspensionIdx == 1) {
            newY = 0.15f;
            typeText.text = "Low";
        }
        else if (suspensionIdx == 2) {
            newY = 0.1f;
            controller.Customizer.loadout.customizationData.cambersFront = tilts;
            typeText.text = "Drift";
        }
        controller.transform.up.Set(controller.transform.up.x, newY, controller.transform.up.z);
    }
}
