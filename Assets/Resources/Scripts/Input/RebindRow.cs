using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RebindRow : MonoBehaviour
{
    public TextMeshProUGUI actionLabel;
    public TextMeshProUGUI keyLabel;
    public Button rebindButton;

    public Color conflictColor = new Color(0.9f, 0.25f, 0.25f, 1f);

    private InputAction action;
    private int bindingIndex = -1;
    private Action onRebound;
    private string device = "Keyboard";

    private Color normalColor = Color.white;
    private InputActionRebindingExtensions.RebindingOperation op;

    public string EffectivePath
    {
        get { return (action != null && bindingIndex >= 0) ? action.bindings[bindingIndex].effectivePath : null; }
    }

    public string KeyDisplay
    {
        get { return keyLabel != null ? keyLabel.text : ""; }
    }

    public void Setup(string displayName, InputAction action, int bindingIndex, string device, Action onRebound)
    {
        this.action = action;
        this.bindingIndex = bindingIndex;
        this.device = string.IsNullOrEmpty(device) ? "Keyboard" : device;
        this.onRebound = onRebound;

        if (actionLabel != null)
            actionLabel.text = displayName;

        if (keyLabel != null)
            normalColor = keyLabel.color;

        rebindButton.onClick.RemoveAllListeners();
        rebindButton.onClick.AddListener(StartRebind);

        RefreshKeyLabel();
    }

    public void RefreshKeyLabel()
    {
        if (keyLabel == null || action == null || bindingIndex < 0)
            return;

        keyLabel.text = action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions).ToUpperInvariant();
    }

    public void SetConflict(bool on)
    {
        if (keyLabel != null)
            keyLabel.color = on ? conflictColor : normalColor;
    }

    void StartRebind()
    {
        if (action == null || bindingIndex < 0)
            return;

        keyLabel.text = "...";
        action.Disable();

        var rebind = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape");

        if (device == "Gamepad")
            rebind = rebind.WithControlsHavingToMatchPath("<Gamepad>").WithControlsExcluding("<Keyboard>").WithControlsExcluding("<Mouse>");
        else
            rebind = rebind.WithControlsExcluding("<Gamepad>").WithControlsExcluding("Mouse");

        op = rebind
            .OnComplete(o =>
            {
                Cleanup();
                RefreshKeyLabel();
                onRebound?.Invoke();
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
        action.Enable();
    }

    void OnDisable()
    {
        op?.Dispose();
        op = null;
    }
}
