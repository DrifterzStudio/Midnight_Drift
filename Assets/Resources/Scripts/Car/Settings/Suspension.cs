using UnityEngine;
using UnityEngine.UI;

public class Suspension : MonoBehaviour, IDataPersistence, IVehicleDependent
{

    public string dataFileName;

    public RCCP_CarController controller;

    [Tooltip("The button that change the suspensson distance.")]
    public Button distButton;
    [Tooltip("Text showing the current value of suspension distance.")]
    public Text distText;

    [Tooltip("The button that change the suspension froce.")]
    public Button forceButton;
    [Tooltip("Text showing the current value of suspension force.")]
    public Text forceText;

    [Tooltip("The button that change the suspension target.")]
    public Button targetButton;
    [Tooltip("Text showing the current value of suspension target.")]
    public Text targetText;

    [Tooltip("The button that change the suspension damper.")]
    public Button damperButton;
    [Tooltip("Text showing the current value of suspension damper.")]
    public Text damperText;

    // values within RCCP's own slider ranges (RCCP_Canvas): springs 10000-100000, dampers
    // 1000-10000, distances 0.05-0.4, targets 0-1
    private static readonly float[] DistSteps = { .1f, .2f, .3f };
    private static readonly float[] ForceSteps = { 40000f, 55000f, 70000f };
    private static readonly float[] TargetSteps = { .3f, .5f, .7f };
    private static readonly float[] DamperSteps = { 2500f, 3500f, 4500f };

    // one label set per param: Soft/Stiff is for a spring or damper, not a height
    private static readonly string[] DistLabels = { "Low", "Stock", "High" };
    private static readonly string[] ForceLabels = { "Soft", "Stock", "Stiff" };
    private static readonly string[] DamperLabels = { "Soft", "Stock", "Stiff" };

    // inverted on purpose: targetPosition goes 0 (fully extended) to 1 (fully compressed), so a
    // higher number sits the car lower.
    private static readonly string[] TargetLabels = { "High", "Stock", "Low" };

    // RCCP's sliders clamp ride height to 0.05 - 0.4, stock is 0.2
    private const float StockRideHeight = .2f;
    private const float MinRideHeight = .05f;
    private const float MaxRideHeight = .4f;

    public float distValue = .2f;
    public float forceValue = 55000f;
    public float targetValue = .5f;
    public float damperValue = 3500f;

    public static Suspension instance;

    public void LoadGame(IGameData data)
    {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null)
        {
            distValue = tmp.distValue;
            forceValue = tmp.forceValue;
            targetValue = tmp.targetValue;
            damperValue = tmp.damperValue;
        }

        ApplyToController();
        RefreshUI();
    }

    public void SaveGame(IGameData data)
    {
        SaveSettings tmp = data as SaveSettings;
        if (tmp != null)
        {
            tmp.distValue = distValue;
            tmp.forceValue = forceValue;
            tmp.targetValue = targetValue;
            tmp.damperValue = damperValue;
        }
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

        if (distButton != null) distButton.onClick.AddListener(OnDistButtonClicked);
        if (forceButton != null) forceButton.onClick.AddListener(OnForceButtonClicked);
        if (targetButton != null) targetButton.onClick.AddListener(OnTargetButtonClicked);
        if (damperButton != null) damperButton.onClick.AddListener(OnDamperButtonClicked);
    }

    private void Start()
    {
        RefreshUI();
    }

    private void OnDistButtonClicked()
    {
        distValue = NextStep(distValue, DistSteps);
        ApplyToController();
        RefreshUI();
    }

    private void OnForceButtonClicked()
    {
        forceValue = NextStep(forceValue, ForceSteps);
        ApplyToController();
        RefreshUI();
    }

    private void OnTargetButtonClicked()
    {
        targetValue = NextStep(targetValue, TargetSteps);
        ApplyToController();
        RefreshUI();
    }

    private void OnDamperButtonClicked()
    {
        damperValue = NextStep(damperValue, DamperSteps);
        ApplyToController();
        RefreshUI();
    }

    static float NextStep(float current, float[] steps)
    {
        return steps[(NearestStepIndex(current, steps) + 1) % steps.Length];
    }

    // the saved value is the raw setting, not an index, so the cursor is rebuilt from it
    static int NearestStepIndex(float value, float[] steps)
    {
        int nearest = 0;
        float smallestDelta = Mathf.Abs(value - steps[0]);

        for (int i = 1; i < steps.Length; i++)
        {
            float delta = Mathf.Abs(value - steps[i]);

            if (delta < smallestDelta)
            {
                smallestDelta = delta;
                nearest = i;
            }
        }

        return nearest;
    }

    // lets SuspensionUpgrade set a new base ride height through us (the single owner of
    // suspensionDistance) instead of writing the field itself.
    public void ReapplySuspension()
    {
        ApplyToController();
        RefreshUI();
    }

    void ApplyToController()
    {
        RCCP_CustomizationData custom = CustomizationData;

        if (custom == null)
            return;

        custom.suspensionDistanceFront = RideHeight;
        custom.suspensionDistanceRear = RideHeight;
        custom.suspensionSpringForceFront = forceValue;
        custom.suspensionSpringForceRear = forceValue;
        custom.suspensionTargetFront = targetValue;
        custom.suspensionTargetRear = targetValue;
        custom.suspensionDamperFront = damperValue;
        custom.suspensionDamperRear = damperValue;
    }

    // shows the label, not the raw number - "55000" means nothing to a player
    void RefreshUI()
    {
        if (distText != null) distText.text = DistLabels[NearestStepIndex(distValue, DistSteps)];
        if (forceText != null) forceText.text = ForceLabels[NearestStepIndex(forceValue, ForceSteps)];
        if (targetText != null) targetText.text = TargetLabels[NearestStepIndex(targetValue, TargetSteps)];
        if (damperText != null) damperText.text = DamperLabels[NearestStepIndex(damperValue, DamperSteps)];
    }

    // the Chassis upgrade sets the base ride height (Normal/Low/Drift), this tab trims around it.
    // distValue is that trim (offset from the stock 0.2 m), so buying Drift keeps the player's trim.
    float RideHeight
    {
        get
        {
            float baseHeight = SuspensionUpgrade.instance != null
                ? SuspensionUpgrade.instance.BaseRideHeight
                : StockRideHeight;

            float trim = distValue - StockRideHeight;

            return Mathf.Clamp(baseHeight + trim, MinRideHeight, MaxRideHeight);
        }
    }

    RCCP_CustomizationData CustomizationData
    {
        get
        {
            if (controller == null || controller.Customizer == null || controller.Customizer.loadout == null)
                return null;

            return controller.Customizer.loadout.customizationData;
        }
    }

    private void OnDestroy()
    {
        if (distButton != null) distButton.onClick.RemoveListener(OnDistButtonClicked);
        if (forceButton != null) forceButton.onClick.RemoveListener(OnForceButtonClicked);
        if (targetButton != null) targetButton.onClick.RemoveListener(OnTargetButtonClicked);
        if (damperButton != null) damperButton.onClick.RemoveListener(OnDamperButtonClicked);
    }
}
