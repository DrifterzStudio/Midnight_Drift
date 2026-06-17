using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class LoadCarModification : MonoBehaviour {

    public RCCP_CarController controller;

    void Update() {

        // Suspensions
        if (controller.Customizer.loadout.customizationData.suspensionDistanceFront == SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionDistanceFront)
            controller.Customizer.loadout.customizationData.suspensionDistanceFront = SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionDistanceFront;

        if (controller.Customizer.loadout.customizationData.suspensionDistanceRear == SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionDistanceRear)
            controller.Customizer.loadout.customizationData.suspensionDistanceRear = SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionDistanceRear;

        if (controller.Customizer.loadout.customizationData.suspensionSpringForceFront == SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionSpringForceFront)
            controller.Customizer.loadout.customizationData.suspensionSpringForceFront = SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionSpringForceFront;

        if (controller.Customizer.loadout.customizationData.suspensionSpringForceRear == SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionSpringForceRear)
            controller.Customizer.loadout.customizationData.suspensionSpringForceRear = SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionSpringForceRear;

        if (controller.Customizer.loadout.customizationData.suspensionTargetFront == SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionTargetFront)
            controller.Customizer.loadout.customizationData.suspensionTargetFront = SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionTargetFront;

        if (controller.Customizer.loadout.customizationData.suspensionTargetRear == SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionTargetRear)
            controller.Customizer.loadout.customizationData.suspensionTargetRear = SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionTargetRear;

        if (controller.Customizer.loadout.customizationData.suspensionDamperFront == SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionDamperFront)
            controller.Customizer.loadout.customizationData.suspensionDamperFront = SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionDamperFront;

        if (controller.Customizer.loadout.customizationData.suspensionDamperRear == SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionDamperRear)
            controller.Customizer.loadout.customizationData.suspensionDamperRear = SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.suspensionDamperRear;


        // Driving Aid

        if (controller.GetVehicleBehaviorType().ABS == SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().ABS)
            controller.GetVehicleBehaviorType().ABS = SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().ABS;

        if (controller.GetVehicleBehaviorType().TCS == SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().TCS)
            controller.GetVehicleBehaviorType().TCS = SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().TCS;

        if (controller.GetVehicleBehaviorType().ESP == SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().ESP)
            controller.GetVehicleBehaviorType().ESP = SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().ESP;

        if (controller.GetVehicleBehaviorType().steeringHelper == SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().steeringHelper)
            controller.GetVehicleBehaviorType().steeringHelper = SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().steeringHelper;

        if (controller.GetVehicleBehaviorType().tractionHelper == SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().tractionHelper)
            controller.GetVehicleBehaviorType().tractionHelper = SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().tractionHelper;

        if (controller.Stability.preserveSpeedFactor == SaveSetttings.vehiculeSettings.Stability.preserveSpeedFactor)
            controller.Stability.preserveSpeedFactor = SaveSetttings.vehiculeSettings.Stability.preserveSpeedFactor;


        // Wheels / camber

        if (controller.Customizer.loadout.customizationData.cambersFront == SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.cambersFront)
            controller.Customizer.loadout.customizationData.cambersFront = SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.cambersFront;

        if (controller.Customizer.loadout.customizationData.cambersRear == SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.cambersRear)
            controller.Customizer.loadout.customizationData.cambersRear = SaveSetttings.vehiculeSettings.Customizer.loadout.customizationData.cambersRear;

        //Wheels / grip

        if (controller.Stability.driftRearForwardStiffnessMin == SaveSetttings.vehiculeSettings.Stability.driftRearForwardStiffnessMin)
            controller.Stability.driftRearForwardStiffnessMin = SaveSetttings.vehiculeSettings.Stability.driftRearForwardStiffnessMin;

        if (controller.Stability.driftRearSidewaysStiffnessMin == SaveSetttings.vehiculeSettings.Stability.driftRearSidewaysStiffnessMin)
            controller.Stability.driftRearSidewaysStiffnessMin = SaveSetttings.vehiculeSettings.Stability.driftRearSidewaysStiffnessMin;

        if (controller.Stability.driftFrontSidewaysStiffnessMin == SaveSetttings.vehiculeSettings.Stability.driftFrontSidewaysStiffnessMin)
            controller.Stability.driftFrontSidewaysStiffnessMin = SaveSetttings.vehiculeSettings.Stability.driftFrontSidewaysStiffnessMin;

        //Wheels

        if (controller.GetVehicleBehaviorType().steeringSensitivity == SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().steeringSensitivity)
            controller.GetVehicleBehaviorType().steeringSensitivity = SaveSetttings.vehiculeSettings.GetVehicleBehaviorType().steeringSensitivity;

        // steering curve


        // Braking

        if (controller.RearAxle.isHandbrake == SaveSetttings.vehiculeSettings.RearAxle.isHandbrake)
            controller.RearAxle.isHandbrake = SaveSetttings.vehiculeSettings.RearAxle.isHandbrake;

        if (controller.RearAxle.handbrakeMultiplier == SaveSetttings.vehiculeSettings.RearAxle.handbrakeMultiplier)
            controller.RearAxle.handbrakeMultiplier = SaveSetttings.vehiculeSettings.RearAxle.handbrakeMultiplier;

        if (controller.FrontAxle.brakeMultiplier == SaveSetttings.vehiculeSettings.FrontAxle.brakeMultiplier)
            controller.FrontAxle.brakeMultiplier = SaveSetttings.vehiculeSettings.FrontAxle.brakeMultiplier;

        if (controller.RearAxle.brakeMultiplier == SaveSetttings.vehiculeSettings.RearAxle.brakeMultiplier)
            controller.RearAxle.brakeMultiplier = SaveSetttings.vehiculeSettings.RearAxle.brakeMultiplier;


        // Gearbox
        if (controller.Gearbox.currentGearState.gearState == SaveSetttings.vehiculeSettings.Gearbox.currentGearState.gearState)
            controller.Gearbox.currentGearState.gearState = SaveSetttings.vehiculeSettings.Gearbox.currentGearState.gearState;

        if (controller.Gearbox.transmissionType == SaveSetttings.vehiculeSettings.Gearbox.transmissionType)
            controller.Gearbox.transmissionType = SaveSetttings.vehiculeSettings.Gearbox.transmissionType;

        if (controller.Gearbox.shiftThreshold == SaveSetttings.vehiculeSettings.Gearbox.shiftThreshold)
            controller.Gearbox.shiftThreshold = SaveSetttings.vehiculeSettings.Gearbox.shiftThreshold;

        if (controller.Gearbox.shiftingTime == SaveSetttings.vehiculeSettings.Gearbox.shiftingTime)
            controller.Gearbox.shiftingTime = SaveSetttings.vehiculeSettings.Gearbox.shiftingTime;

        if (controller.Inputs.cutThrottleWhenShifting == SaveSetttings.vehiculeSettings.Inputs.cutThrottleWhenShifting)
            controller.Inputs.cutThrottleWhenShifting = SaveSetttings.vehiculeSettings.Inputs.cutThrottleWhenShifting;

        if (controller.RearAxle.antirollForce == SaveSetttings.vehiculeSettings.RearAxle.antirollForce)
            controller.RearAxle.antirollForce = SaveSetttings.vehiculeSettings.RearAxle.antirollForce;


        //Others

        if (controller.Inputs.steeringDeadzone == SaveSetttings.vehiculeSettings.Inputs.steeringDeadzone)
            controller.Inputs.steeringDeadzone = SaveSetttings.vehiculeSettings.Inputs.steeringDeadzone;

        if (controller.Inputs.throttleDeadzone == SaveSetttings.vehiculeSettings.Inputs.throttleDeadzone)
            controller.Inputs.throttleDeadzone = SaveSetttings.vehiculeSettings.Inputs.throttleDeadzone;

        if (controller.Inputs.brakeDeadzone == SaveSetttings.vehiculeSettings.Inputs.brakeDeadzone)
            controller.Inputs.brakeDeadzone = SaveSetttings.vehiculeSettings.Inputs.brakeDeadzone;

        if (controller.Inputs.handbrakeDeadzone == SaveSetttings.vehiculeSettings.Inputs.handbrakeDeadzone)
            controller.Inputs.handbrakeDeadzone = SaveSetttings.vehiculeSettings.Inputs.handbrakeDeadzone;

        if (controller.Inputs.clutchDeadzone == SaveSetttings.vehiculeSettings.Inputs.clutchDeadzone)
            controller.Inputs.clutchDeadzone = SaveSetttings.vehiculeSettings.Inputs.clutchDeadzone;

    }

}
