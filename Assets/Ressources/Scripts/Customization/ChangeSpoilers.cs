using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSpoilers : MonoBehaviour {

    public RCCP_CarController controller;

    public Button spoilersButton;

    public Text spoilersText;

    public List<GameObject> spoilers;

    private int spoilersIndex = -1;

    private void Awake() {
        spoilersButton.onClick.AddListener(OnButtonClicked);
    }

    void Update() {
        if (spoilersIndex != -1) controller.Customizer.SpoilerManager.spoilers[spoilersIndex].gameObject.SetActive(true);

        if (spoilersIndex == -1) spoilersText.text = "None";
        else spoilersText.text = "" + spoilersIndex;
    }

    private void OnButtonClicked() {
        if (spoilersIndex != -1) controller.Customizer.SpoilerManager.spoilers[spoilersIndex].gameObject.SetActive(false);
        if (spoilersIndex + 1 > spoilers.Count) spoilersIndex = -1;
        else spoilersIndex += 1;
    }
}
