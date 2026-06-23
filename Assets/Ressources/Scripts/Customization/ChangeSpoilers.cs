using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSpoilers : MonoBehaviour, IDataPersistence {

    [Header("Customization")]
    public RCCP_CarController controller;

    public Button spoilersButton;

    public Text spoilersText;

    public List<GameObject> spoilers;

    private int spoilersIndex = -1;


    public void LoadGame(IGameData data) {
        Debug.Log("Load spoilers");
        SaveCustom tmp = data as SaveCustom;
        if (tmp != null) {
            spoilers = tmp.spoilers;
            spoilersIndex = tmp.currentSpoiler;
        }
    }

    public void SaveGame(IGameData data) {
        Debug.Log("Save spoilers");
        SaveCustom tmp = data as SaveCustom;
        if (tmp != null) {
            tmp.spoilers = spoilers;
            tmp.currentSpoiler = spoilersIndex;
        }
    }

    public string getDataFileName() {
        return "";
    }



    private void Awake() {
        if (spoilersButton != null) spoilersButton.onClick.AddListener(OnButtonClicked);
    }

    void Update() {

        if (spoilersIndex != -1) controller.Customizer.SpoilerManager.spoilers[spoilersIndex].gameObject.SetActive(true);

        if (spoilersText != null) {
            if (spoilersIndex == -1) spoilersText.text = "None";
            else spoilersText.text = "" + spoilersIndex;
        }
    }

    private void OnButtonClicked() {
        if (spoilersIndex != -1) controller.Customizer.SpoilerManager.spoilers[spoilersIndex].gameObject.SetActive(false);

        if (spoilersIndex + 1 > spoilers.Count - 1) spoilersIndex = -1;
        else spoilersIndex += 1;
    }

    public int GetSpoilersIndex() { 
        return spoilersIndex; 
    }
}
