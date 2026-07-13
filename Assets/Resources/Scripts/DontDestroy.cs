using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour {

    private static List<GameObject> persistentObjects = new List<GameObject>();

    private void Awake() {
        GameObject tmp = null;
        foreach (GameObject persistentObject in persistentObjects) {
            if (persistentObject.name == gameObject.name) {
                tmp = persistentObject;
            }
        }
        if (tmp == null) {
            persistentObjects.Add(gameObject);
            DontDestroyOnLoad(gameObject);
        }
        else if (tmp != gameObject) {
            Destroy(gameObject);
        }

    }
}