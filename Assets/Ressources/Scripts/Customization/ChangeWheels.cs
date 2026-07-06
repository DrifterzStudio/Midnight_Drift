using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWheels : SaveCustom, IDataPersistence {

    [Header("Customization")]
    public List<MeshRenderer> meshRenderer;

    public Button wheelsButton;

    public Text wheelsText;

    private int materialIndex = 0;

    public static ChangeWheels instance;

    public RCCP_CarController controller;

    public void LoadGame(IGameData data) {
        SaveCustom tmp = data as SaveCustom;
        if (tmp != null) {
            materials = tmp.materials;
            materialIndex = tmp.currentMat;
        }
    }

    public void SaveGame(IGameData data) {
        SaveCustom tmp = data as SaveCustom;
        if (tmp != null) {
            tmp.materials = materials;
            tmp.currentMat = materialIndex;
        }
    }

    public string getDataFileName() {
        return dataFileName;
    }



    private void Awake() {
        if (instance == null) instance = this;
        dataPersistence.dataPersistenceObjects.Add(instance);

        if (wheelsButton != null) wheelsButton.onClick.AddListener(OnButtonClicked);
        
    }

    void Update() {
        instance = this;

        meshRenderer[0].material = materials[materialIndex];
        meshRenderer[1].material = materials[materialIndex];
        meshRenderer[2].material = materials[materialIndex];
        meshRenderer[3].material = materials[materialIndex];

        if (wheelsText != null) wheelsText.text = materialIndex.ToString();

    }

    private void OnButtonClicked() {
        if (materialIndex + 1 > materials.Count - 1) materialIndex = 0;
        else materialIndex += 1;
    }


    public int GetMatIndex() {
        return materialIndex;
    }

    public Material GetCurrentMat() { 
        return materials[materialIndex]; 
    }
}
