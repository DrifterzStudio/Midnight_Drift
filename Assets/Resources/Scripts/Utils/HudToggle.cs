using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// press a key to hide/show every canvas at once, for clean trailer shots (the lap hud is built at
// runtime so it can't be turned off from the editor).
public class HudToggle : MonoBehaviour
{
    [Tooltip("Key that toggles the whole HUD. Change it if it clashes with a car control.")]
    public Key toggleKey = Key.H;

    private bool _hidden;
    private readonly List<Canvas> _hiddenCanvases = new List<Canvas>();

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current[toggleKey].wasPressedThisFrame)
            return;

        if (!_hidden)
            HideAll();
        else
            ShowBack();
    }

    void HideAll()
    {
        _hiddenCanvases.Clear();

        // only the ones currently visible, so we can put back exactly those
        foreach (Canvas c in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (c.enabled)
            {
                c.enabled = false;
                _hiddenCanvases.Add(c);
            }
        }

        _hidden = true;
    }

    void ShowBack()
    {
        foreach (Canvas c in _hiddenCanvases)
        {
            if (c != null)
                c.enabled = true;
        }

        _hiddenCanvases.Clear();
        _hidden = false;
    }
}
