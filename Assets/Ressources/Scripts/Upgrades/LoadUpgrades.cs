using UnityEngine;

public class LoadUpgrades : MonoBehaviour {

    public RCCP_Engine engine;
    public RCCP_Gearbox gearbox;

    private void Awake() {
        // Turbo
        if (AddTurbo.instance != null) {
            engine.turbo1Charged = AddTurbo.instance.turbo1;
            engine.turbo2Charged = AddTurbo.instance.turbo2;
        }

        // Transmision
        // Gearbox
        if (GearboxRatio.instance != null) {
            gearbox.gearRatios = GearboxRatio.instance.gearRatios;
        }
    }
}
