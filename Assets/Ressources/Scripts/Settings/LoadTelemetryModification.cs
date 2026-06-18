using UnityEngine;

public class LoadTelemetryModification : MonoBehaviour {

    public GameObject telemetry;

    void Start() {

        if (SaveSetttings.telemetrySettings) telemetry.SetActive(true);
        else telemetry.SetActive(false);
    }
}
