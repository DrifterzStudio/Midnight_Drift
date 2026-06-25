using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using System.Collections.Generic;

public class LoadCarModification : MonoBehaviour {

    public RCCP_CarController controller;
    public RCCP_CustomizationData customData;
    public RCCP_Stability stability;
    [Tooltip("idx 0 = rear / idx 1 = front")]
    public List<RCCP_Axle> axles;
    public RCCP_Gearbox gearbox;
    public RCCP_Input inputs;

    void Awake() {
        if (controller != null) {

            // Suspension
            if (Suspension.instance != null) {
                customData.suspensionDistanceFront = Suspension.instance.distValue;
                customData.suspensionDistanceRear = Suspension.instance.distValue;
                customData.suspensionSpringForceFront = Suspension.instance.forceValue;
                customData.suspensionSpringForceRear = Suspension.instance.forceValue;
                customData.suspensionTargetFront = Suspension.instance.targetValue;
                customData.suspensionTargetRear = Suspension.instance.targetValue;
                customData.suspensionDamperFront = Suspension.instance.damperValue;
                customData.suspensionDamperRear = Suspension.instance.damperValue;
            }

            // Driving Aid
            if (DrivingAid.instance != null) {
                controller.GetVehicleBehaviorType().ABS = DrivingAid.instance.carController.GetVehicleBehaviorType().ABS;
                controller.GetVehicleBehaviorType().TCS = DrivingAid.instance.carController.GetVehicleBehaviorType().TCS;
                controller.GetVehicleBehaviorType().ESP = DrivingAid.instance.carController.GetVehicleBehaviorType().ESP;
                controller.GetVehicleBehaviorType().steeringHelper = DrivingAid.instance.carController.GetVehicleBehaviorType().steeringHelper;
                controller.GetVehicleBehaviorType().tractionHelper = DrivingAid.instance.carController.GetVehicleBehaviorType().tractionHelper;
                stability.preserveSpeedFactor = DrivingAid.instance.ASPValue;
            }

            //Wheels
            if (Wheels.instance != null) {
                controller.GetVehicleBehaviorType().steeringSensitivity = Wheels.instance.sensitivityValue;
                //steeringCurve
            }

            // Camber
            if (Camber.instance != null) {
                customData.cambersFront = Camber.instance.frontAngle;
                customData.cambersRear = Camber.instance.rearAngle;
            }

            // Grip
            if (Grip.instance != null) {
                stability.driftRearForwardStiffnessMin = Grip.instance.forwardValue;
                stability.driftRearSidewaysStiffnessMin = Grip.instance.rearSidewaysValue;
                stability.driftFrontSidewaysStiffnessMin = Grip.instance.frontSidewaysValue;
            }

            //Braking
            if (Braking.instance != null && axles.Count == 2) {
                axles[0].isHandbrake = Braking.instance.isHandbrake;
                axles[0].handbrakeMultiplier = Braking.instance.handbrakeMultiplier;
                axles[1].handbrakeMultiplier = Braking.instance.handbrakeMultiplier;
                axles[0].brakeMultiplier = Braking.instance.brakeMultiplier;
            }

            // Gearbox
            if (Gearbox.instance != null) {
                gearbox.currentGearState.gearState = Gearbox.instance.gearState;
                gearbox.transmissionType = Gearbox.instance.transmissionType;
                gearbox.shiftThreshold = Gearbox.instance.GSTValue;
                gearbox.shiftingTime = Gearbox.instance.shiftingDelay;
                inputs.cutThrottleWhenShifting = Gearbox.instance.CTWS;
                customData.clutchThreshold = Gearbox.instance.clutchThreshold;
                if (axles.Count == 2)
                    axles[1].antirollForce = Gearbox.instance.ARFValue;
            }

            // Propulsion Type
            if (PropulsionType.instance != null && axles.Count == 2) {
                axles[1].isSteer = PropulsionType.instance.fIsSteer;
                axles[1].isHandbrake = PropulsionType.instance.fIsHandbrake;
                axles[0].isSteer = PropulsionType.instance.rIsSteer;
                axles[0].isHandbrake = PropulsionType.instance.rIsHandbrake;
            }

            // Others
            if (Others.instance != null) {
                inputs.steeringDeadzone = Others.instance.steerValue;
                inputs.throttleDeadzone = Others.instance.throttleValue;
                inputs.brakeDeadzone = Others.instance.brakeValue;
                inputs.handbrakeDeadzone = Others.instance.handbrakeValue;
                inputs.clutchDeadzone = Others.instance.clutchValue;
            }
        }
    }


}
