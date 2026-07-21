using UnityEngine;
using UnityEngine.UI;

public class GearboxRatio : MonoBehaviour, IDataPersistence, IVehicleDependent
{

    public string dataFileName;

    public RCCP_CarController controller;

    public Text ratioText;

    public float[] gearRatios;

    // original ratios, grabbed on assign, for the "back to stock" step
    private float[] stockGearRatios;

    private int currentUpgrade = 0;

    public static GearboxRatio instance;

    public void SaveGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
        {
            tmp.gearRatios = gearRatios;
            tmp.gearboxUpgrade = currentUpgrade;
        }
    }

    public void LoadGame(IGameData data)
    {
        SaveUpgrades tmp = data as SaveUpgrades;
        if (tmp != null)
        {
            gearRatios = tmp.gearRatios;
            currentUpgrade = Mathf.Clamp(tmp.gearboxUpgrade, 0, 4);
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

        // car still has its stock ratios here (before LoadGame), so grab them now
        if (controller != null && controller.Gearbox != null && controller.Gearbox.gearRatios != null)
            stockGearRatios = (float[])controller.Gearbox.gearRatios.Clone();

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

    public void OnClick()
    {
        // cycle stock -> tier 1..4 -> stock, past the last one cancels it
        currentUpgrade = (currentUpgrade + 1) % 5;

        if (currentUpgrade == 1)
            gearRatios = new float[] { 2.86f, 1.62f, 1.0f, .72f };
        else if (currentUpgrade == 2)
            gearRatios = new float[] { 4.35f, 2.5f, 1.66f, 1.23f, 1.0f, .85f };
        else if (currentUpgrade == 3)
            gearRatios = new float[] { 4.5f, 2.5f, 1.66f, 1.23f, 1.0f, .9f, .8f };
        else if (currentUpgrade == 4)
            gearRatios = new float[] { 4.6f, 2.5f, 1.86f, 1.43f, 1.23f, 1.05f, .9f, .72f };
        else
            gearRatios = stockGearRatios; // 0 -> back to stock

        ApplyToController();
        RefreshUI();
    }

    void ApplyToController()
    {
        if (controller == null || controller.Gearbox == null)
            return;

        if (currentUpgrade > 0 && gearRatios != null && gearRatios.Length > 0)
            controller.Gearbox.gearRatios = gearRatios;
        else if (stockGearRatios != null)
            controller.Gearbox.gearRatios = stockGearRatios; // stock tier
    }

    void RefreshUI()
    {
        if (ratioText == null)
            return;

        ratioText.text = currentUpgrade.ToString();
    }
}
