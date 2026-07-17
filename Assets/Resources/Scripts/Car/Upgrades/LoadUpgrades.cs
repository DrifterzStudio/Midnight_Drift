using UnityEngine;

/// <summary>
/// Applies the upgrades bought in the garage to this vehicle when its scene loads.
/// Reads the SaveUpgrades container, which reaches this scene because SaveUpgradeData
/// carries DontDestroy. Attach to the vehicle root (the object holding RCCP_CarController).
/// </summary>
public class LoadUpgrades : MonoBehaviour {

    [Tooltip("Left empty, it resolves from this GameObject. Every sub-component (engine, gearbox, " +
             "axles...) is reached through the controller, which finds them on its own.")]
    public RCCP_CarController controller;

    private void Awake() {
        if (controller == null)
            controller = GetComponent<RCCP_CarController>();

        if (controller == null)
            controller = GetComponentInParent<RCCP_CarController>();

        if (controller == null) {
            Debug.LogWarning("LoadUpgrades: no RCCP_CarController found, vehicle keeps its default setup.", this);
            return;
        }

        SaveUpgrades data = FindFirstObjectByType<SaveUpgrades>(FindObjectsInactive.Include);

        if (data == null) {
            Debug.LogWarning("LoadUpgrades: no SaveUpgrades found, vehicle keeps its default setup.", this);
            return;
        }

        // Engine
        if (data.maxEnginePowerValue > 0 && controller.Engine != null) {
            controller.Engine.minEngineRPM = data.minEnginePowerValue;
            controller.Engine.maxEngineRPM = data.maxEnginePowerValue;
        }

        // Turbo
        if (controller.Engine != null) {
            controller.Engine.turbo1Charged = data.turbo1;
            controller.Engine.turbo2Charged = data.turbo2;
        }

        // Chassis. Both fields are deltas against the stock mass, not absolute values: the
        // original assigned them directly, setting the car to -50 or -200 kg. Read the stock
        // mass before touching it, and let carbon win when both are owned, since the two
        // upgrades replace each other rather than stack.
        if (controller.Rigid != null) {
            float stockMass = controller.Rigid.mass;

            if (data.carbonMass != 0)
                controller.Rigid.mass = stockMass + data.carbonMass;
            else if (data.lightenedMass != 0)
                controller.Rigid.mass = stockMass + data.lightenedMass;
        }

        // AntiRollBar
        if (data.antiRollBarValue > 0) {
            if (controller.FrontAxle != null) controller.FrontAxle.antirollForce = data.antiRollBarValue;
            if (controller.RearAxle != null) controller.RearAxle.antirollForce = data.antiRollBarValue;
        }

        // Suspension. newY is the ride height, mirroring SuspensionUpgrade: the original wrote
        // controller.transform.up.Set(..., newY, ...), a no-op on a returned Vector3 copy.
        // Cambers only come with the lowest (drift) level, as in SuspensionUpgrade.
        if (data.newY != 0 && CustomizationData != null) {
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
    }

    RCCP_CustomizationData CustomizationData {
        get {
            if (controller.Customizer == null || controller.Customizer.loadout == null)
                return null;

            return controller.Customizer.loadout.customizationData;
        }
    }
}
