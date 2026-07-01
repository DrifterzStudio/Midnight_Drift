using System.Collections.Generic;
using UnityEngine;

public class LoadUpgrades : MonoBehaviour {

    public RCCP_CarController controller;
    public RCCP_Engine engine;
    public RCCP_Gearbox gearbox;
    [Tooltip("idx 0 = rear / idx 1 = front")]
    public List<RCCP_Axle> axles;
    public RCCP_AeroDynamics aero;
    public RCCP_CustomizationData custom;

    private void Awake() {
        // Engine
        if (EnginePower.instance != null) {
            engine.minEngineRPM = EnginePower.instance.minEnginePowerValue;
            engine.maxEngineRPM = EnginePower.instance.maxEnginePowerValue;
        }

        // Turbo
        if (AddTurbo.instance != null) {
            engine.turbo1Charged = AddTurbo.instance.turbo1;
            engine.turbo2Charged = AddTurbo.instance.turbo2;
        }

        // Chassis
        if (Lightened.instance != null) {
            controller.Rigid.mass = Lightened.instance.lightenedMass;
        }

        if (CarbonFiberBody.instance != null) {
            controller.Rigid.mass = CarbonFiberBody.instance.carbonMass;
        }

        if (Reinforcement.instance != null) {
            aero.COM.transform.up.Set(controller.AeroDynamics.COM.transform.up.x, Reinforcement.instance.newYCenter, controller.AeroDynamics.COM.transform.up.z);
        }

        if (AntiRollBar.instance != null && axles.Count == 2) {
            axles[0].antirollForce = AntiRollBar.instance.antiRollBarValue;
            axles[1].antirollForce = AntiRollBar.instance.antiRollBarValue;
        }

        // Suspension
        if (SuspensionUpgrade.instance != null) {
            controller.transform.up.Set(controller.transform.up.x, SuspensionUpgrade.instance.newY, controller.transform.up.z);
            custom.cambersFront = SuspensionUpgrade.instance.tilts;
        }

        // Tire
        if (Slick.instance != null) {
            aero.wheelResistance = Slick.instance.slickness;
        }

        // Transmision 
        if (GearboxRatio.instance != null) {
            gearbox.gearRatios = GearboxRatio.instance.gearRatios;
        }

        if (Brake.instance != null) {
            engine.engineBrakingCoefficient = Brake.instance.brakePower;
        }
    }
}
