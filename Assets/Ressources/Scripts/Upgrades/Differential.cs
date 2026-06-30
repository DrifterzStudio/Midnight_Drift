using UnityEngine;
using UnityEngine.Rendering;

public class Differential : MonoBehaviour//, IDataPersistence
{

    public RCCP_CarController controller;

    public static Differential instance;

    void Awake() {
        if (instance == null) instance = this;    
    }

    public void OnButtonClicked() {

    }
}
