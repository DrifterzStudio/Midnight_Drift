using UnityEngine;

// applies the saved settings to the car on spawn. put it on the vehicle root.
public class LoadCarModification : MonoBehaviour
{

    [Tooltip("Left empty, it resolves from this GameObject. Sub-components are reached through " +
             "the controller, which finds them on its own.")]
    public RCCP_CarController controller;

    void Awake()
    {
        if (controller == null)
            controller = GetComponent<RCCP_CarController>();

        if (controller == null)
            controller = GetComponentInParent<RCCP_CarController>();

        if (controller == null)
        {
            Debug.LogWarning("LoadCarModification: no RCCP_CarController found, vehicle keeps its default setup.", this);
            return;
        }

        SaveSettings data = FindFirstObjectByType<SaveSettings>(FindObjectsInactive.Include);
        bool tempData = false;

        // launched without the garage? load this vehicle's save straight from disk
        if (data == null && GameSession.SelectedVehicle != null)
        {
            data = TuningDisk.Load<SaveSettings>("Settings", "settings", GameSession.SelectedVehicle.vehicleId);
            tempData = data != null;
        }

        if (data == null)
        {
            Debug.LogWarning("LoadCarModification: no SaveSettings found, vehicle keeps its default setup.", this);
            return;
        }

        RCCP_CustomizationData custom = CustomizationData;

        // every block below is guarded on a non-zero value (like LoadUpgrades). a never-tuned save
        // holds zeros, and applying those would give a car with no suspension, grip or brakes.

        // Suspension
        if (custom != null)
        {
            if (data.distValue > 0f)
            {
                custom.suspensionDistanceFront = data.distValue;
                custom.suspensionDistanceRear = data.distValue;
            }

            if (data.forceValue > 0f)
            {
                custom.suspensionSpringForceFront = data.forceValue;
                custom.suspensionSpringForceRear = data.forceValue;
            }

            if (data.targetValue > 0f)
            {
                custom.suspensionTargetFront = data.targetValue;
                custom.suspensionTargetRear = data.targetValue;
            }

            if (data.damperValue > 0f)
            {
                custom.suspensionDamperFront = data.damperValue;
                custom.suspensionDamperRear = data.damperValue;
            }

            // camber. zero is a valid setting here (RCCP default), so no guard
            custom.cambersFront = data.frontAngle;
            custom.cambersRear = data.rearAngle;

            // Gearbox
            if (data.clutchThreshold > 0f)
                custom.clutchThreshold = data.clutchThreshold;
        }

        // grip and driving aid. RCCP copies the global behavior into Stability on spawn, so writing
        // here (after) is what makes these stick per car.
        if (controller.Stability != null)
        {
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
        if (controller.RearAxle != null)
        {
            controller.RearAxle.isHandbrake = data.isHandbrake;

            if (data.handbrakeMultiplier > 0f) controller.RearAxle.handbrakeMultiplier = data.handbrakeMultiplier;
            if (data.brakeMultiplier > 0f) controller.RearAxle.brakeMultiplier = data.brakeMultiplier;
        }

        if (controller.FrontAxle != null && data.handbrakeMultiplier > 0f)
            controller.FrontAxle.handbrakeMultiplier = data.handbrakeMultiplier;

        // steering sensitivity (RCCP_Axle.steerSpeed). both axles, since the rear steers on AWD
        if (data.sensitivityValue > 0f)
        {
            if (controller.FrontAxle != null) controller.FrontAxle.steerSpeed = data.sensitivityValue;
            if (controller.RearAxle != null) controller.RearAxle.steerSpeed = data.sensitivityValue;
        }

        // gearbox. gearState (Park/Reverse/Neutral/Forward) is left alone on purpose - it's runtime
        // state, not a tuning choice, and forcing Park would stall the car at the start.
        if (controller.Gearbox != null)
        {
            controller.Gearbox.transmissionType = data.transmissionType;

            if (data.GSTValue > 0f) controller.Gearbox.shiftThreshold = data.GSTValue;
            if (data.shiftingDelay > 0f) controller.Gearbox.shiftingTime = data.shiftingDelay;
        }

        if (controller.Inputs != null)
            controller.Inputs.cutThrottleWhenShifting = data.CTWS;

        // anti-roll force isn't here - AntiRollBar (the Chassis upgrade) owns it, LoadUpgrades applies it.

        // propulsion type. runs after Braking on purpose - both write the axle handbrake flags,
        // and the drive layout is what the player last picked.
        if (controller.FrontAxle != null)
        {
            controller.FrontAxle.isSteer = data.frontAxleSteer;
            controller.FrontAxle.isHandbrake = data.frontAxleHandbrake;
        }

        if (controller.RearAxle != null)
        {
            controller.RearAxle.isSteer = data.rearAxleSteer;
            controller.RearAxle.isHandbrake = data.rearAxleHandbrake;
        }

        if (tempData)
            Destroy(data.gameObject);
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
