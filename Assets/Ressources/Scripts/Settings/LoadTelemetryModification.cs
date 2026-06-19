using UnityEngine;

public class LoadTelemetryModification : MonoBehaviour {

    public GameObject telemetry;

    void Start() {

        if (SaveSettings.telemetrySettings) telemetry.SetActive(true);
        else telemetry.SetActive(false);
    }
}
