using UnityEngine;

/// <summary>
/// Applies the settings tuned in the garage to this vehicle when its scene loads.
/// Attach to the vehicle root (the object holding RCCP_CarController).
///
/// Reads the SaveSettings container, which persists via DontDestroy, rather than the garage's
/// Gearbox/PropulsionType/... statics which are destroyed with the garage scene.
/// </summary>
public class LoadCarModification : MonoBehaviour {

    [Tooltip("Left empty, it resolves from this GameObject. Sub-components are reached through " +
             "the controller, which finds them on its own.")]
    public RCCP_CarController controller;

    void Awake() {
        if (controller == null)
            controller = GetComponent<RCCP_CarController>();

        if (controller == null)
            controller = GetComponentInParent<RCCP_CarController>();

        if (controller == null) {
            Debug.LogWarning("LoadCarModification: no RCCP_CarController found, vehicle keeps its default setup.", this);
            return;
        }

        SaveSettings data = FindFirstObjectByType<SaveSettings>(FindObjectsInactive.Include);

        if (data == null) {
            Debug.LogWarning("LoadCarModification: no SaveSettings found, vehicle keeps its default setup.", this);
            return;
        }

        RCCP_CustomizationData custom = CustomizationData;

        // Every block below is guarded on a non-zero value, like LoadUpgrades does. A save the
        // player has never tuned holds zeros, and applying those verbatim would ship a car with
        // no suspension travel, no spring, no damping, no grip and no brakes.

        // Suspension
        if (custom != null) {
            if (data.distValue > 0f) {
                custom.suspensionDistanceFront = data.distValue;
                custom.suspensionDistanceRear = data.distValue;
            }

            if (data.forceValue > 0f) {
                custom.suspensionSpringForceFront = data.forceValue;
                custom.suspensionSpringForceRear = data.forceValue;
            }

            if (data.targetValue > 0f) {
                custom.suspensionTargetFront = data.targetValue;
                custom.suspensionTargetRear = data.targetValue;
            }

            if (data.damperValue > 0f) {
                custom.suspensionDamperFront = data.damperValue;
                custom.suspensionDamperRear = data.damperValue;
            }

            // Camber. Zero is a legitimate setting here (it is the RCCP default), so no guard.
            custom.cambersFront = data.frontAngle;
            custom.cambersRear = data.rearAngle;

            // Gearbox
            if (data.clutchThreshold > 0f)
                custom.clutchThreshold = data.clutchThreshold;
        }

        // Grip and driving aid. RCCP_CarController copies the global behavior into Stability on
        // spawn, so writing here (afterwards) is what makes these stick, per vehicle.
        if (controller.Stability != null) {
            controller.Stability.ABS = data.ABS;
            controller.Stability.TCS = data.TCS;
            controller.Stability.ESP = data.ESP;
            controller.Stability.steeringHelper = data.SH;
            controller.Stability.tractionHelper = data.TH;

            if (data.ASPValue > 0f) controller.Stability.preserveSpeedFactor = data.ASPValue;
            if (data.forwardValue > 0f) controller.Stability.driftRearForwardStiffnessMin = data.forwardValue;
            if (data.rearSidewaysValue > 0f) controller.Stability.driftRearSidewaysStiffnessMin = data.rearSidewaysValue;
            if (data.frontSidewaysValue > 0f) controller.Stability.driftFrontSidewaysStiffnessMin = data.frontSidewaysValue;
        }

        // Braking
        if (controller.RearAxle != null) {
            controller.RearAxle.isHandbrake = data.isHandbrake;

            if (data.handbrakeMultiplier > 0f) controller.RearAxle.handbrakeMultiplier = data.handbrakeMultiplier;
            if (data.brakeMultiplier > 0f) controller.RearAxle.brakeMultiplier = data.brakeMultiplier;
        }

        if (controller.FrontAxle != null && data.handbrakeMultiplier > 0f)
            controller.FrontAxle.handbrakeMultiplier = data.handbrakeMultiplier;

        // Steering sensitivity (RCCP_Axle.steerSpeed). Both axles, since the rear steers on AWD.
        if (data.sensitivityValue > 0f) {
            if (controller.FrontAxle != null) controller.FrontAxle.steerSpeed = data.sensitivityValue;
            if (controller.RearAxle != null) controller.RearAxle.steerSpeed = data.sensitivityValue;
        }

        // Gearbox. gearState (Park/Reverse/Neutral/Forward) is deliberately not applied: it is
        // transient runtime state, not a tuning choice, and forcing it (Park especially) would
        // stall the car at the start of a race. RCCP manages it from the prefab default.
        if (controller.Gearbox != null) {
            controller.Gearbox.transmissionType = data.transmissionType;

            if (data.GSTValue > 0f) controller.Gearbox.shiftThreshold = data.GSTValue;
            if (data.shiftingDelay > 0f) controller.Gearbox.shiftingTime = data.shiftingDelay;
        }

        if (controller.Inputs != null)
            controller.Inputs.cutThrottleWhenShifting = data.CTWS;

        // Anti-roll force is not here: AntiRollBar (the Chassis upgrade) owns it, and
        // LoadUpgrades applies it.

        // Propulsion Type. Runs after Braking on purpose: both write the axle handbrake flags,
        // and the drive layout is what the player last chose.
        if (controller.FrontAxle != null) {
            controller.FrontAxle.isSteer = data.frontAxleSteer;
            controller.FrontAxle.isHandbrake = data.frontAxleHandbrake;
        }

        if (controller.RearAxle != null) {
            controller.RearAxle.isSteer = data.rearAxleSteer;
            controller.RearAxle.isHandbrake = data.rearAxleHandbrake;
        }

    }

    RCCP_CustomizationData CustomizationData {
        get {
            if (controller.Customizer == null || controller.Customizer.loadout == null)
                return null;

            return controller.Customizer.loadout.customizationData;
        }
    }
}
