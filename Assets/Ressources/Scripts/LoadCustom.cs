using UnityEngine;
using System.Collections.Generic;

public class LoadCustom : MonoBehaviour {

    public List<MeshRenderer> wheels;

    public List<GameObject> spoilers;

    void Awake() {
        if (ChangeSpoilers.instance != null) {
            if (ChangeSpoilers.instance.GetSpoilersIndex() != -1)
                spoilers[ChangeSpoilers.instance.GetSpoilersIndex()].gameObject.SetActive(true);
        }

        if (ChangeWheels.instance != null) {
            wheels[0].material = ChangeWheels.instance.GetCurrentMat();
            wheels[1].material = ChangeWheels.instance.GetCurrentMat();
            wheels[2].material = ChangeWheels.instance.GetCurrentMat();
            wheels[3].material = ChangeWheels.instance.GetCurrentMat();
        }

    }

    void Update() {
        
    }
}
