using UnityEngine;
using UnityEngine.UI;

public class SuspensionUpgrade : MonoBehaviour, IDataPersistence {

    public DataPersistenceManager dataPersistence;

    public string dataFileName;

    public RCCP_CarController controller;

    public Text typeText;

    public float newY = 0.16f;

    public float tilts = 5f;

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
        return dataFileName;
    }

    void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);
    }

    private void Update() {
        if (suspensionIdx == 0) typeText.text = "Normal";
        if (suspensionIdx == 1) typeText.text = "Low";
        if (suspensionIdx == 2) typeText.text = "Drift";
    }

    public void OnButtonClicked() {
        if (suspensionIdx < 2) suspensionIdx += 1;
        if (suspensionIdx == 0) {
            newY = 0.2f;
        }
        else if (suspensionIdx == 1) {
            newY = 0.15f;
        }
        else if (suspensionIdx == 2) {
            newY = 0.1f;
            controller.Customizer.loadout.customizationData.cambersFront = tilts;
        }
        controller.transform.up.Set(controller.transform.up.x, newY, controller.transform.up.z);
    }
}
