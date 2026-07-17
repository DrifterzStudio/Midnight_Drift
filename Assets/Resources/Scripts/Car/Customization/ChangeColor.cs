using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cycles the vehicle's body paint through a palette, for the Visuel tab.
/// </summary>
public class ChangeColor : MonoBehaviour, IDataPersistence, IVehicleDependent {

    [System.Serializable]
    public class PaintColor {

        [Tooltip("Name shown to the player.")]
        public string name = "Color";

        [Tooltip("Body colour applied to every painter of the vehicle.")]
        public Color color = Color.white;
    }

    public string dataFileName;

    public RCCP_CarController controller;

    [Tooltip("Button cycling to the next colour.")]
    public Button colorButton;

    [Tooltip("Text showing the current colour's name.")]
    public Text colorText;

    [Tooltip("Colours the player can pick, in order.")]
    public PaintColor[] palette = {
        new PaintColor { name = "Rouge",  color = new Color(.60f, .05f, .05f) },
        new PaintColor { name = "Noir",   color = new Color(.06f, .06f, .07f) },
        new PaintColor { name = "Blanc",  color = new Color(.90f, .90f, .92f) },
        new PaintColor { name = "Bleu",   color = new Color(.08f, .20f, .55f) },
        new PaintColor { name = "Jaune",  color = new Color(.85f, .70f, .10f) },
        new PaintColor { name = "Violet", color = new Color(.35f, .10f, .50f) }
    };

    // Alpha 0 means the player never picked a colour, matching RCCP's own convention on
    // RCCP_CustomizationLoadout.paint. The prefab's paint is then left alone.
    private Color currentColor = new Color(1f, 1f, 1f, 0f);

    public static ChangeColor instance;

    public void SaveGame(IGameData data) {
        SaveCustom tmp = data as SaveCustom;
        if (tmp != null)
            tmp.bodyColor = currentColor;
    }

    public void LoadGame(IGameData data) {
        SaveCustom tmp = data as SaveCustom;
        if (tmp != null)
            currentColor = tmp.bodyColor;

        ApplyToController();
        RefreshUI();
    }

    public string getDataFileName() {
        return dataFileName;
    }

    public void SetController(RCCP_CarController newController) {
        controller = newController;
        ApplyToController();
        RefreshUI();
    }

    private void Awake() {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);

        if (colorButton != null) colorButton.onClick.AddListener(OnColorButtonClicked);
    }

    private void Start() {
        RefreshUI();
    }

    private void OnColorButtonClicked() {
        if (palette == null || palette.Length == 0)
            return;

        int next = HasPaint ? (NearestPaletteIndex() + 1) % palette.Length : 0;

        currentColor = palette[next].color;
        currentColor.a = 1f;

        ApplyToController();
        RefreshUI();
    }

    void ApplyToController() {
        if (!HasPaint || controller == null || controller.Customizer == null)
            return;

        RCCP_VehicleUpgrade_PaintManager paintManager = controller.Customizer.PaintManager;

        if (paintManager == null)
            return;

        // Paint() rather than PaintWithoutSave(): it also updates RCCP's own loadout, which the
        // Customizer restores on init and would otherwise overwrite our colour.
        paintManager.Paint(currentColor);
    }

    // Called only when the colour changes, never per frame.
    void RefreshUI() {
        if (colorText == null)
            return;

        colorText.text = HasPaint ? palette[NearestPaletteIndex()].name : "Origine";
    }

    bool HasPaint {
        get { return currentColor.a > 0f; }
    }

    /// <summary>
    /// The colour itself is saved, not an index, so reordering the palette can't silently repaint
    /// the player's car. The cursor is rebuilt by finding the closest entry.
    /// </summary>
    int NearestPaletteIndex() {
        int nearest = 0;
        float smallestDelta = float.MaxValue;

        for (int i = 0; i < palette.Length; i++) {
            Color candidate = palette[i].color;

            float delta = Mathf.Abs(candidate.r - currentColor.r)
                        + Mathf.Abs(candidate.g - currentColor.g)
                        + Mathf.Abs(candidate.b - currentColor.b);

            if (delta < smallestDelta) {
                smallestDelta = delta;
                nearest = i;
            }
        }

        return nearest;
    }

    private void OnDestroy() {
        if (colorButton != null) colorButton.onClick.RemoveListener(OnColorButtonClicked);
    }
}
