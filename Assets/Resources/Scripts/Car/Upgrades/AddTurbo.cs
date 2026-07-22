using UnityEngine;
using UnityEngine.UI;

public class AddTurbo : MonoBehaviour, IDataPersistence, IVehicleDependent
{

    public string dataFileName;

    public RCCP_CarController controller;

    public Text turboNumber;

    public bool turbo1 = false;
    public bool turbo2 = false;
    public static AddTurbo instance;

    public void SaveGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
        {
            tmp.turbo1 = turbo1;
            tmp.turbo2 = turbo2;
        }
    }

    public void LoadGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
        {
            turbo1 = tmp.turbo1;
            turbo2 = tmp.turbo2;
        }

        ApplyToController();
        RefreshUI();
    }

    public string getDataFileName()
    {
        return dataFileName;
    }

    public void SetController(RCCP_CarController newController)
    {
        controller = newController;
        ApplyToController();
        RefreshUI();
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);
    }

    private void Start()
    {
        RefreshUI();
    }

    public void onButtonClicked()
    {
        // cycle none -> turbo 1 -> turbo 1+2 -> none, past the last one removes it
        if (!turbo1) turbo1 = true;                    // none -> 1
        else if (!turbo2) turbo2 = true;               // 1 -> 2
        else { turbo1 = false; turbo2 = false; }       // 2 -> none

        ApplyToController();
        RefreshUI();
    }

    void ApplyToController()
    {
        if (controller == null || controller.Engine == null)
            return;

        // write both flags so the turbo can be turned back off
        controller.Engine.turbo1Charged = turbo1;
        controller.Engine.turbo2Charged = turbo2;
    }

    void RefreshUI()
    {
        if (turboNumber == null)
            return;

        if (turbo2) turboNumber.text = "2";
        else if (turbo1) turboNumber.text = "1";
        else turboNumber.text = "0";
    }
}
