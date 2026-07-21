using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// "choose your car" popup shown when the player hits Play. builds one button per vehicle, and the
// pick sets GameSession + loads the circuit. works from any scene (MainMenu, Garage) since it only
// needs the vehicle list, not the garage.
public class CarSelectionMenu : MonoBehaviour
{
    [Tooltip("The popup panel, hidden until Play is pressed.")]
    public GameObject panel;

    [Tooltip("All the cars the player can pick.")]
    public List<VehicleDefinition> vehicles;

    [Tooltip("One button per car is built from this.")]
    public Button buttonPrefab;

    [Tooltip("Where the buttons get spawned.")]
    public Transform buttonContainer;

    [Tooltip("Scene loaded once a car is chosen.")]
    public string sceneToLoad = "Circuit_Solo";

    [Header("Garage only")]
    [Tooltip("Controllers switched off while the menu is open (e.g. the garage player movement / interactor). Leave empty in menu scenes.")]
    public MonoBehaviour[] disableWhileOpen;

    [Tooltip("Re-lock the cursor when closing, for the garage's first-person control. Leave off in menu scenes.")]
    public bool lockCursorOnClose = false;

    private bool _built;

    // hook this on the Play buttons (MainMenu, Garage)
    public void Open()
    {
        if (!_built)
        {
            BuildButtons();
            _built = true;
        }

        if (panel != null)
            panel.SetActive(true);

        // free the cursor so the buttons are clickable, and stop the player from moving/looking
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetControllers(false);
    }

    // hook this on a back/cancel button
    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);

        SetControllers(true);

        if (lockCursorOnClose)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void SetControllers(bool on)
    {
        if (disableWhileOpen == null)
            return;

        foreach (MonoBehaviour b in disableWhileOpen)
        {
            if (b != null)
                b.enabled = on;
        }
    }

    void BuildButtons()
    {
        if (buttonContainer == null || buttonPrefab == null)
        {
            Debug.LogError("CarSelectionMenu: Button Container or Button Prefab not assigned.", this);
            return;
        }

        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        foreach (VehicleDefinition vehicle in vehicles)
        {
            Button b = Instantiate(buttonPrefab, buttonContainer);

            TMP_Text label = b.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = vehicle.displayName;

            // capture for the lambda, otherwise every button would use the last vehicle
            VehicleDefinition captured = vehicle;
            b.onClick.AddListener(() => OnCarChosen(captured));
        }
    }

    void OnCarChosen(VehicleDefinition vehicle)
    {
        GameSession.SelectVehicle(vehicle);

        // loading bar if we have it, plain load as a fallback
        if (LoadingScreenManager.Instance != null)
            LoadingScreenManager.Instance.LoadScene(sceneToLoad);
        else
            SceneManager.LoadScene(sceneToLoad);
    }
}
