using UnityEngine;

public class LoadTelemetryModification : MonoBehaviour {

    void Update() {

        if (SaveSetttings.telemetrySettings) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}
