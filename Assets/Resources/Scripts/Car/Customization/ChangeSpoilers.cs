using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSpoilers : MonoBehaviour, IDataPersistence {

    public string dataFileName;

    [Header("Customization")]
    public RCCP_CarController controller;

    public List<GameObject> spoilers;

    public Text spoilersText;

    public static ChangeSpoilers instance;

    public int spoilersIndex = -1;


    public void LoadGame(IGameData data) {
        SaveCustom tmp = data as SaveCustom;
        if (tmp != null) {
            spoilersIndex = tmp.currentSpoiler;
        }
    }

    public void SaveGame(IGameData data) {
        SaveCustom tmp = data as SaveCustom;
        if (tmp != null) {
            tmp.currentSpoiler = spoilersIndex;
        }
    }

    public string getDataFileName() {
        return dataFileName;
    }



    private void Awake() {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);
    }

    void Update() {
        instance = this;

        if (spoilersIndex != -1) controller.Customizer.SpoilerManager.spoilers[spoilersIndex].gameObject.SetActive(true);

        if (spoilersText != null) {
            if (spoilersIndex == -1) spoilersText.text = "None";
            else spoilersText.text = "" + spoilersIndex;
        }
    }

    public void OnButtonClicked() {
        if (spoilersIndex != -1) controller.Customizer.SpoilerManager.spoilers[spoilersIndex].gameObject.SetActive(false);

        if (spoilersIndex + 1 > spoilers.Count - 1) spoilersIndex = -1;
        else spoilersIndex += 1;
    }

    public int GetSpoilersIndex() { 
        return spoilersIndex; 
    }
}
