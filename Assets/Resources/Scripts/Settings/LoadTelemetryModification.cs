using UnityEngine;

public class LoadTelemetryModification : MonoBehaviour {

    public GameObject telemetry;

    void Awake() {
        if (Others.instance != null) {
            if (Others.instance.isTelemetry) telemetry.SetActive(true);
            else telemetry.SetActive(false);
        }
    }
}
