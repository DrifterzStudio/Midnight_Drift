using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// escape pause menu, shared by scenes. each scene sets its title + buttons in Menus below. it
// spawns itself and builds its own ui, so there's nothing to wire in the editor.
public class PauseMenu : MonoBehaviour
{
    enum ActionType { Resume, LoadScene, Quit }

    struct ButtonDef
    {
        public string Label;
        public ActionType Action;
        public string Scene;

        public ButtonDef(string label, ActionType action, string scene = null)
        {
            Label = label;
            Action = action;
            Scene = scene;
        }
    }

    class MenuConfig
    {
        public string Title;
        public ButtonDef[] Buttons;
    }

    static readonly Dictionary<string, MenuConfig> Menus = new Dictionary<string, MenuConfig>
    {
        {
            "Circuit_Solo", new MenuConfig
            {
                Title = "PAUSE",
                Buttons = new[]
                {
                    new ButtonDef("Resume", ActionType.Resume),
                    new ButtonDef("Back to Garage", ActionType.LoadScene, "Garage"),
                    new ButtonDef("Back to Menu", ActionType.LoadScene, "MainMenu"),
                }
            }
        },
        {
            "Garage", new MenuConfig
            {
                Title = "PAUSE",
                Buttons = new[]
                {
                    new ButtonDef("Resume", ActionType.Resume),
                    new ButtonDef("Back to Menu", ActionType.LoadScene, "MainMenu"),
                    new ButtonDef("Quit Game", ActionType.Quit),
                }
            }
        },
    };

    static PauseMenu instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void SubscribeToScenes()
    {
        instance = null;
        SceneManager.sceneLoaded -= OnAnySceneLoaded;
        SceneManager.sceneLoaded += OnAnySceneLoaded;
    }

    // handles the boot scene (its sceneLoaded doesn't always fire). runs after the scene's objects
    // exist so we don't make an EventSystem before the scene's own one.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void SpawnForInitialScene()
    {
        TrySpawnFor(SceneManager.GetActiveScene());
    }

    static void OnAnySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrySpawnFor(scene);
    }

    static void TrySpawnFor(Scene scene)
    {
        if (!Menus.ContainsKey(scene.name)) return;
        if (instance != null) return;
        if (FindAnyObjectByType<PauseMenu>() != null) return;

        GameObject go = new GameObject("PauseMenu (auto)");
        if (scene.IsValid() && scene.isLoaded)
            SceneManager.MoveGameObjectToScene(go, scene);
        go.AddComponent<PauseMenu>();
    }

    MenuConfig config;
    GameObject canvasRoot;
    bool isPaused;
    bool isLeaving;

    CursorLockMode prevLockState;
    bool prevCursorVisible;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        Menus.TryGetValue(gameObject.scene.name, out config);
        if (config == null)
        {
            Destroy(gameObject);
            return;
        }

        BuildUI();
        ShowMenu(false);
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
        if (isPaused) Time.timeScale = 1f;
    }

    void Update()
    {
        if (isLeaving) return;

        Keyboard kb = Keyboard.current;
        if (kb != null && kb.escapeKey.wasPressedThisFrame)
            Toggle();
    }

    void Toggle()
    {
        if (isPaused) Resume();
        else Pause();
    }

    void Pause()
    {
        isPaused = true;

        prevLockState = Cursor.lockState;
        prevCursorVisible = Cursor.visible;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowMenu(true);
    }

    void Resume()
    {
        isPaused = false;

        Time.timeScale = 1f;
        Cursor.lockState = prevLockState;
        Cursor.visible = prevCursorVisible;

        ShowMenu(false);
    }

    void OnButtonPressed(ButtonDef def)
    {
        switch (def.Action)
        {
            case ActionType.Resume:
                Resume();
                break;
            case ActionType.LoadScene:
                LeaveTo(def.Scene);
                break;
            case ActionType.Quit:
                QuitGame();
                break;
        }
    }

    void LeaveTo(string sceneName)
    {
        if (isLeaving) return;
        isLeaving = true;
        isPaused = false;

        // put time back before loading, or LoadingScreenManager's WaitForSeconds hangs at timeScale 0
        Time.timeScale = 1f;

        SaveGarageIfAny();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowMenu(false);

        if (LoadingScreenManager.Instance != null)
            LoadingScreenManager.Instance.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }

    void QuitGame()
    {
        if (isLeaving) return;
        isLeaving = true;
        isPaused = false;

        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    static void SaveGarageIfAny()
    {
        GarageDisplayManager garage = FindAnyObjectByType<GarageDisplayManager>();
        if (garage != null)
            garage.SaveIfCustomizing();
    }

    void ShowMenu(bool visible)
    {
        if (canvasRoot != null)
            canvasRoot.SetActive(visible);
    }

    void BuildUI()
    {
        EnsureEventSystem();

        canvasRoot = new GameObject("PauseCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasRoot.transform.SetParent(transform, false);

        Canvas canvas = canvasRoot.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30000;

        CanvasScaler scaler = canvasRoot.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject dim = NewUI("Dim", canvasRoot.transform);
        Image dimImg = dim.AddComponent<Image>();
        dimImg.color = new Color(0f, 0f, 0f, 0.78f);
        Stretch(dim.GetComponent<RectTransform>());

        GameObject panel = NewUI("Panel", dim.transform);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.09f, 0.10f, 0.13f, 0.98f);
        RectTransform panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(560f, 160f + config.Buttons.Length * 88f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(45, 45, 45, 45);
        layout.spacing = 22f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        AddTitle(config.Title, panel.transform);

        foreach (ButtonDef def in config.Buttons)
        {
            ButtonDef captured = def;
            AddButton(def.Label, panel.transform, () => OnButtonPressed(captured));
        }
    }

    void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;
        if (FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 0) return;

        GameObject es = new GameObject("EventSystem", typeof(EventSystem));
        es.transform.SetParent(transform, false);
        InputSystemUIInputModule module = es.AddComponent<InputSystemUIInputModule>();
        module.AssignDefaultActions();
    }

    void AddTitle(string text, Transform parent)
    {
        GameObject go = NewUI("Title", parent);
        Text label = go.AddComponent<Text>();
        label.text = text;
        label.font = UIFont;
        label.fontSize = 46;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight = 70f;
        le.preferredHeight = 70f;
    }

    void AddButton(string text, Transform parent, System.Action onClick)
    {
        GameObject go = NewUI("Button_" + text, parent);
        Image bg = go.AddComponent<Image>();
        bg.color = new Color(0.16f, 0.18f, 0.24f, 1f);

        Button button = go.AddComponent<Button>();
        button.targetGraphic = bg;
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.16f, 0.18f, 0.24f, 1f);
        colors.highlightedColor = new Color(0.24f, 0.28f, 0.38f, 1f);
        colors.pressedColor = new Color(0.10f, 0.12f, 0.16f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        button.onClick.AddListener(() => onClick());

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight = 66f;
        le.preferredHeight = 66f;

        GameObject textGO = NewUI("Text", go.transform);
        Text label = textGO.AddComponent<Text>();
        label.text = text;
        label.font = UIFont;
        label.fontSize = 28;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        Stretch(textGO.GetComponent<RectTransform>());
    }

    static GameObject NewUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static Font cachedFont;
    static Font UIFont
    {
        get
        {
            if (cachedFont == null)
                cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (cachedFont == null)
                cachedFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return cachedFont;
        }
    }
}
