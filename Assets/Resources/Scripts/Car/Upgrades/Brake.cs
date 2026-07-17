using UnityEngine;
using UnityEngine.UI;

public class Brake : MonoBehaviour, IDataPersistence, IVehicleDependent
{
    public string dataFileName;

    public Text brakePowerText;

    public float brakePower = 0.15f;

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
            brakePower = tmp.brakePower;
            ApplyToController();
            RefreshUI();
        }
    }

    public void SaveGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
        {
            tmp.brakePower = brakePower;
        }
    }

    public string getDataFileName()
    {
        return dataFileName;
    }

    public void ApplyToController()
    {
        if (controller != null && controller.Engine != null)
            controller.Engine.engineBrakingCoefficient = brakePower;
    }

    // Called only when the value changes, never per frame.
    void RefreshUI()
    {
        if (brakePowerText == null)
            return;

        if (brakePower == 0.15f) brakePowerText.text = "Normal";
        else if (brakePower == 0.17f) brakePowerText.text = "Upgraded";
        else if (brakePower == 0.2f) brakePowerText.text = "Max";
    }

    public void OnButtonClicked()
    {
        if (brakePower == 0.15f) brakePower = 0.17f;
        else if (brakePower == 0.17f) brakePower = 0.2f;
        ApplyToController();
        RefreshUI();
    }
}