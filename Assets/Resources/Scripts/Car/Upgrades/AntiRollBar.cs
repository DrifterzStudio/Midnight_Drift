using UnityEngine;
using UnityEngine.UI;

public class AntiRollBar : MonoBehaviour, IDataPersistence, IVehicleDependent
{
    public string dataFileName;

    public Text ARBText;

    public int antiRollBarValue = 500;

    private RCCP_CarController controller;

    private void Awake()
    {
        DataPersistenceManager.instance.dataPersistenceObjects.Add(this);
    }

    private void Start()
    {
        RefreshUI();
    }

    public void SetController(RCCP_CarController newController)
    {
        controller = newController;
        ApplyToController();
        RefreshUI();
    }

    public void LoadGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
        {
            // 0 = not saved yet, fall back to stock
            antiRollBarValue = tmp.antiRollBarValue > 0 ? tmp.antiRollBarValue : 500;
            ApplyToController();
            RefreshUI();
        }
    }

    public void SaveGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
        {
            tmp.antiRollBarValue = antiRollBarValue;
        }
    }

    public string getDataFileName()
    {
        return dataFileName;
    }

    public void ApplyToController()
    {
        if (controller == null)
            return;

        if (controller.FrontAxle != null) controller.FrontAxle.antirollForce = antiRollBarValue;
        if (controller.RearAxle != null) controller.RearAxle.antirollForce = antiRollBarValue;
    }

    void RefreshUI()
    {
        if (ARBText != null) ARBText.text = antiRollBarValue.ToString();
    }

    public void OnButtonClicked()
    {
        // cycle 500 -> 1000 -> 1500 -> 500, past the last one goes back to stock
        if (antiRollBarValue == 500) antiRollBarValue = 1000;
        else if (antiRollBarValue == 1000) antiRollBarValue = 1500;
        else antiRollBarValue = 500;
        ApplyToController();
        RefreshUI();
    }
}