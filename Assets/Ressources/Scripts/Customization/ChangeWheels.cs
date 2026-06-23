using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWheels : MonoBehaviour {

    public List<Material> materials;

    public List<MeshRenderer> meshRenderer;

    public Button wheelsButton;

    public Text wheelsText;

    private int materialIndex = 0;


    private void Awake() {
        wheelsButton.onClick.AddListener(OnButtonClicked);
    }

    void Update() {

        meshRenderer[0].material = materials[materialIndex];
        meshRenderer[1].material = materials[materialIndex];
        meshRenderer[2].material = materials[materialIndex];
        meshRenderer[3].material = materials[materialIndex];

        wheelsText.text = materialIndex.ToString();

    }

    private void OnButtonClicked() {
        if (materialIndex + 1 > materials.Count - 1) materialIndex = 0;
        else materialIndex += 1;
    }
}
