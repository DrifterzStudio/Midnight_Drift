using UnityEngine;

public class SuspensionUpgrade : MonoBehaviour { // IDataPersistence

    public RCCP_CarController controller;

    public static SuspensionUpgrade instance;

    void Awake() {
        if (instance == null) instance = this;
    }

    public void OnButtonClicked() {

    }
}
