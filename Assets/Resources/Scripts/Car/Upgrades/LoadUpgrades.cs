using System.Collections.Generic;
using UnityEngine;

public class LoadUpgrades : MonoBehaviour
{

    public RCCP_CarController controller;
    public RCCP_Engine engine;
    public RCCP_Gearbox gearbox;
    [Tooltip("idx 0 = rear / idx 1 = front")]
    public List<RCCP_Axle> axles;
    public RCCP_AeroDynamics aero;
    public RCCP_CustomizationData custom;

    private void Awake()
    {
        SaveUpgrades data = FindFirstObjectByType<SaveUpgrades>(FindObjectsInactive.Include);

        if (data == null)
        {
            Debug.LogWarning("LoadUpgrades: no SaveUpgrades found, vehicle keeps its default setup.");
            return;
        }

        // Engine
        if (data.maxEnginePowerValue > 0)
        {
            engine.minEngineRPM = data.minEnginePowerValue;
            engine.maxEngineRPM = data.maxEnginePowerValue;
        }

        // Turbo
        engine.turbo1Charged = data.turbo1;
        engine.turbo2Charged = data.turbo2;

        // Chassis
        if (data.lightenedMass != 0)
        {
            controller.Rigid.mass = data.lightenedMass;
        }

        if (data.carbonMass != 0)
        {
            controller.Rigid.mass = data.carbonMass;
        }

        if (data.newYCenter != 0)
        {
            aero.COM.transform.up.Set(controller.AeroDynamics.COM.transform.up.x, data.newYCenter, controller.AeroDynamics.COM.transform.up.z);
        }

        // AntiRollBar
        if (data.antiRollBarValue > 0 && axles.Count == 2)
        {
            axles[0].antirollForce = data.antiRollBarValue;
            axles[1].antirollForce = data.antiRollBarValue;
        }

        // Suspension
        if (data.newY != 0)
        {
            controller.transform.up.Set(controller.transform.up.x, data.newY, controller.transform.up.z);
            custom.cambersFront = data.tilts;
        }

        // Tire
        if (data.slickness > 0)
        {
            aero.wheelResistance = data.slickness;
        }

        // Transmission
        if (data.gearRatios != null && data.gearRatios.Length > 0)
        {
            gearbox.gearRatios = data.gearRatios;
        }

        if (data.brakePower > 0)
        {
            engine.engineBrakingCoefficient = data.brakePower;
        }
    }
}