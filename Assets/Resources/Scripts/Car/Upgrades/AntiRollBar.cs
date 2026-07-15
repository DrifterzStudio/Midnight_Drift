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

    public void SetController(RCCP_CarController newController)
    {
        controller = newController;
        ApplyToController();
    }

    public void LoadGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
        {
            antiRollBarValue = tmp.antiRollBarValue;
            ApplyToController();
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
        if (controller != null)
        {
            controller.FrontAxle.antirollForce = antiRollBarValue;
            controller.RearAxle.antirollForce = antiRollBarValue;
        }
    }

    private void Update()
    {
        ARBText.text = antiRollBarValue.ToString();
    }

    public void OnButtonClicked()
    {
        if (antiRollBarValue == 500) antiRollBarValue = 1000;
        else if (antiRollBarValue == 1000) antiRollBarValue = 1500;
        ApplyToController();
    }
}