using System.Collections.Generic;
using UnityEngine;

public class LoadUpgrades : MonoBehaviour {

    public RCCP_Engine engine;
    public RCCP_Gearbox gearbox;
    [Tooltip("idx 0 = rear / idx 1 = front")]
    public List<RCCP_Axle> axles;
    public RCCP_AeroDynamics aero;

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
        if (AntiRollBar.instance != null && axles.Count == 2) {
            axles[0].antirollForce = AntiRollBar.instance.antiRollBarValue;
            axles[1].antirollForce = AntiRollBar.instance.antiRollBarValue;
        }

        // Tire
        if (Slick.instance != null) {
            aero.wheelResistance = Slick.instance.slickness;
        }

        // Transmision {
        // Gearbox
        if (GearboxRatio.instance != null) {
            gearbox.gearRatios = GearboxRatio.instance.gearRatios;
        }

        // Brake
        if (Brake.instance != null) {
            engine.engineBrakingCoefficient = Brake.instance.brakePower;
        }

        //}
    }
}
