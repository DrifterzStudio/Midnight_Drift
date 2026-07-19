using UnityEngine;

// applies the garage upgrades to the car when the scene loads. reads the SaveUpgrades container
// (it survives the scene change thanks to DontDestroy). put this on the vehicle root (the RCCP_CarController).
public class LoadUpgrades : MonoBehaviour
{
    [Tooltip("Left empty, it resolves from this GameObject. Every sub-component (engine, gearbox, " +
             "axles...) is reached through the controller, which finds them on its own.")]
    public RCCP_CarController controller;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<RCCP_CarController>();

        if (controller == null)
            controller = GetComponentInParent<RCCP_CarController>();

        if (controller == null)
        {
            Debug.LogWarning("LoadUpgrades: no RCCP_CarController found, vehicle keeps its default setup.", this);
            return;
        }

        SaveUpgrades data = FindFirstObjectByType<SaveUpgrades>(FindObjectsInactive.Include);

        if (data == null)
        {
            Debug.LogWarning("LoadUpgrades: no SaveUpgrades found, vehicle keeps its default setup.", this);
            return;
        }

        // Engine
        if (data.maxEnginePowerValue > 0 && controller.Engine != null)
        {
            controller.Engine.minEngineRPM = data.minEnginePowerValue;
            controller.Engine.maxEngineRPM = data.maxEnginePowerValue;
        }

        // Turbo
        if (controller.Engine != null)
        {
            controller.Engine.turbo1Charged = data.turbo1;
            controller.Engine.turbo2Charged = data.turbo2;
        }

        // Chassis. the mass values are deltas from the stock mass, not absolute. read stock first,
        // and let carbon win over lightened when both are owned (they replace each other, don't stack)
        if (controller.Rigid != null)
        {
            float stockMass = controller.Rigid.mass;

            if (data.carbonMass != 0)
                controller.Rigid.mass = stockMass + data.carbonMass;
            else if (data.lightenedMass != 0)
                controller.Rigid.mass = stockMass + data.lightenedMass;
        }

        // AntiRollBar
        if (data.antiRollBarValue > 0)
        {
            if (controller.FrontAxle != null) controller.FrontAxle.antirollForce = data.antiRollBarValue;
            if (controller.RearAxle != null) controller.RearAxle.antirollForce = data.antiRollBarValue;
        }

        // Suspension. newY is the ride height (same as SuspensionUpgrade). cambers only on the
        // lowest (drift) level.
        if (data.newY != 0 && CustomizationData != null)
        {
            CustomizationData.suspensionDistanceFront = data.newY;
            CustomizationData.suspensionDistanceRear = data.newY;

            if (data.newY <= 0.1f)
                CustomizationData.cambersFront = data.tilts;
        }

        // Tire
        if (data.slickness > 0 && controller.AeroDynamics != null)
            controller.AeroDynamics.wheelResistance = data.slickness;

        // Transmission
        if (data.gearRatios != null && data.gearRatios.Length > 0 && controller.Gearbox != null)
            controller.Gearbox.gearRatios = data.gearRatios;

        if (data.brakePower > 0 && controller.Engine != null)
            controller.Engine.engineBrakingCoefficient = data.brakePower;

        // Differential. same mode list as the Differential upgrade. Differentials is an array
        // (awd cars have one per axle + a centre one).
        if (controller.Differentials != null)
        {
            RCCP_Differential.DifferentialType mode = DifferentialModeFor(data.differentialType);

            foreach (RCCP_Differential differential in controller.Differentials)
            {
                if (differential != null)
                    differential.differentialType = mode;
            }
        }
    }

    static RCCP_Differential.DifferentialType DifferentialModeFor(int index)
    {
        if (index <= 0) return RCCP_Differential.DifferentialType.Open;
        if (index == 1) return RCCP_Differential.DifferentialType.Limited;

        return RCCP_Differential.DifferentialType.FullLocked;
    }

    RCCP_CustomizationData CustomizationData
    {
        get
        {
            if (controller.Customizer == null || controller.Customizer.loadout == null)
                return null;

            return controller.Customizer.loadout.customizationData;
        }
    }
}
