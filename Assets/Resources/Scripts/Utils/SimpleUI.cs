using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// little helpers to build ui from code (canvas, text, buttons)
public static class SimpleUI
{
    static Font cachedFont;

    public static Font Font
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

    public static GameObject NewUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    public static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public static Canvas CreateOverlayCanvas(string name, Transform parent, int sortingOrder)
    {
        GameObject go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        go.transform.SetParent(parent, false);

        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    public static Text AddText(Transform parent, string text, int fontSize, TextAnchor alignment, FontStyle style = FontStyle.Normal)
    {
        GameObject go = NewUI("Text", parent);
        Text label = go.AddComponent<Text>();
        label.text = text;
        label.font = Font;
        label.fontSize = fontSize;
        label.fontStyle = style;
        label.alignment = alignment;
        label.color = Color.white;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        return label;
    }

    public static Button AddButton(Transform parent, string text, Action onClick, float height = 66f)
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
        le.minHeight = height;
        le.preferredHeight = height;

        Text label = AddText(go.transform, text, 28, TextAnchor.MiddleCenter);
        Stretch(label.GetComponent<RectTransform>());

        return button;
    }

    // new input system only, so clicks need an InputSystemUIInputModule. only make one if there's none
    public static void EnsureEventSystem(Transform parent)
    {
        if (EventSystem.current != null) return;
        if (UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 0) return;

        GameObject es = new GameObject("EventSystem", typeof(EventSystem));
        es.transform.SetParent(parent, false);
        InputSystemUIInputModule module = es.AddComponent<InputSystemUIInputModule>();
        module.AssignDefaultActions();
    }

    // mm:ss.mmm for lap / race times
    public static string FormatTime(float seconds)
    {
        if (seconds < 0f) seconds = 0f;
        int minutes = (int)(seconds / 60f);
        float rem = seconds - minutes * 60f;
        return string.Format("{0:0}:{1:00.000}", minutes, rem);
    }
}
