using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class KeyRebindRow : MonoBehaviour
{
    public TextMeshProUGUI actionLabel;
    public TextMeshProUGUI keyLabel;
    public Button rebindButton;

    public Color conflictColor = new Color(0.9f, 0.25f, 0.25f, 1f);

    private string id;
    private Key defaultKey;
    private Action onRebound;
    private Color normalColor = Color.white;

    private InputAction captureAction;
    private InputActionRebindingExtensions.RebindingOperation op;

    public Key CurrentKey
    {
        get { return KeyBindingStore.Get(id, defaultKey); }
    }

    public void Setup(string display, string id, Key defaultKey, Action onRebound)
    {
        this.id = id;
        this.defaultKey = defaultKey;
        this.onRebound = onRebound;

        if (actionLabel != null)
            actionLabel.text = display;

        if (keyLabel != null)
            normalColor = keyLabel.color;

        rebindButton.onClick.RemoveAllListeners();
        rebindButton.onClick.AddListener(StartRebind);

        RefreshKeyLabel();
    }

    public void RefreshKeyLabel()
    {
        if (keyLabel != null)
            keyLabel.text = CurrentKey.ToString().ToUpperInvariant();
    }

    public void SetConflict(bool on)
    {
        if (keyLabel != null)
            keyLabel.color = on ? conflictColor : normalColor;
    }

    void StartRebind()
    {
        keyLabel.text = "...";

        captureAction = new InputAction(type: InputActionType.Button);
        captureAction.AddBinding("<Keyboard>/space");

        op = captureAction.PerformInteractiveRebinding(0)
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(o =>
            {
                string path = captureAction.bindings[0].effectivePath;
                Cleanup();

                Key key;
                if (TryPathToKey(path, out key))
                {
                    KeyBindingStore.Set(id, key);
                    onRebound?.Invoke();
                }

                RefreshKeyLabel();
            })
            .OnCancel(o =>
            {
                Cleanup();
                RefreshKeyLabel();
            })
            .Start();
    }

    void Cleanup()
    {
        op?.Dispose();
        op = null;

        captureAction?.Dispose();
        captureAction = null;
    }

    static bool TryPathToKey(string path, out Key key)
    {
        key = Key.None;

        if (string.IsNullOrEmpty(path))
            return false;

        int slash = path.LastIndexOf('/');
        string name = slash >= 0 ? path.Substring(slash + 1) : path;

        return Enum.TryParse(name, true, out key) && key != Key.None;
    }

    void OnDisable()
    {
        Cleanup();
    }
}
