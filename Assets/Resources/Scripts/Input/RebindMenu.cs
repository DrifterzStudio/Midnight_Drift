using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindMenu : MonoBehaviour
{
    [Serializable]
    public class Entry
    {
        public string displayName = "Action";
        public string actionName = "";
        public string compositeGroup = "";
        public string part = "";

        public Entry() { }
        public Entry(string display, string action, string group, string part)
        {
            displayName = display;
            actionName = action;
            compositeGroup = group;
            this.part = part;
        }
    }

    [Header("Category")]
    public InputActionAsset asset;
    public string mapName = "Vehicle";
    public string saveKey = "rebinds";
    [Tooltip("Keyboard or Gamepad. Picks which device bindings this menu edits.")]
    public string device = "Keyboard";

    [Header("UI")]
    public RebindRow rowPrefab;
    public Transform content;
    public TextMeshProUGUI warningLabel;

    [Header("Section header (optional)")]
    public GameObject sectionHeaderPrefab;
    public string sectionTitle = "";

    [Header("Keys (empty = default list for this map)")]
    public List<Entry> entries = new List<Entry>();

    private readonly List<RebindRow> rows = new List<RebindRow>();
    private bool built = false;

    void OnEnable()
    {
        if (RebindManager.instance != null)
            RebindManager.instance.OnReset += OnGlobalReset;

        Build();
        RefreshAll();
    }

    void OnDisable()
    {
        if (RebindManager.instance != null)
            RebindManager.instance.OnReset -= OnGlobalReset;
    }

    void Build()
    {
        if (built || asset == null || rowPrefab == null || content == null)
            return;

        var map = asset.FindActionMap(mapName, false);

        if (map == null)
        {
            Debug.LogError("[RebindMenu] Map not found: " + mapName + " in " + asset.name);
            return;
        }

        var list = (entries != null && entries.Count > 0) ? entries : DefaultEntriesFor(mapName, device);

        SpawnSectionHeader();

        foreach (var e in list)
        {
            var action = map.FindAction(e.actionName, false);

            if (action == null)
            {
                Debug.LogWarning("[RebindMenu] Action not found: " + e.actionName);
                continue;
            }

            int idx = ResolveBindingIndex(action, e.compositeGroup, e.part, device);

            if (idx < 0)
            {
                Debug.LogWarning("[RebindMenu] Binding not found for " + e.actionName + " (group=" + e.compositeGroup + ", part=" + e.part + ", device=" + device + ")");
                continue;
            }

            var row = Instantiate(rowPrefab, content);
            row.Setup(e.displayName, action, idx, device, OnRowRebound);
            rows.Add(row);
        }

        built = true;
    }

    void OnRowRebound()
    {
        if (asset != null)
        {
            PlayerPrefs.SetString(saveKey, asset.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        }

        RefreshAll();
    }

    void OnGlobalReset()
    {
        RefreshAll();
    }

    public void ResetToDefault()
    {
        if (asset == null)
            return;

        var map = asset.FindActionMap(mapName, false);

        if (map != null)
            map.RemoveAllBindingOverrides();

        PlayerPrefs.DeleteKey(saveKey);
        PlayerPrefs.Save();

        RefreshAll();
    }

    void RefreshAll()
    {
        for (int i = 0; i < rows.Count; i++)
            rows[i].RefreshKeyLabel();

        RefreshConflicts();
    }

    void RefreshConflicts()
    {
        string firstConflictKey = null;

        for (int i = 0; i < rows.Count; i++)
        {
            string pathI = rows[i].EffectivePath;
            bool conflict = false;

            if (!string.IsNullOrEmpty(pathI))
            {
                for (int j = 0; j < rows.Count; j++)
                {
                    if (j == i)
                        continue;

                    if (rows[j].EffectivePath == pathI)
                    {
                        conflict = true;
                        break;
                    }
                }
            }

            rows[i].SetConflict(conflict);

            if (conflict && firstConflictKey == null)
                firstConflictKey = rows[i].KeyDisplay;
        }

        if (warningLabel != null)
            warningLabel.text = firstConflictKey != null ? ("Duplicate key: " + firstConflictKey) : "";
    }

    void SpawnSectionHeader()
    {
        if (sectionHeaderPrefab == null || string.IsNullOrEmpty(sectionTitle))
            return;

        var header = Instantiate(sectionHeaderPrefab, content);
        var label = header.GetComponentInChildren<TextMeshProUGUI>();

        if (label != null)
            label.text = sectionTitle;
    }

    int ResolveBindingIndex(InputAction action, string group, string part, string device)
    {
        var bindings = action.bindings;

        bool simple = string.IsNullOrEmpty(group) && string.IsNullOrEmpty(part);

        if (simple)
        {
            for (int i = 0; i < bindings.Count; i++)
            {
                var b = bindings[i];

                if (b.isComposite || b.isPartOfComposite)
                    continue;

                if (string.IsNullOrEmpty(b.effectivePath))
                    continue;

                if (string.IsNullOrEmpty(device) || b.effectivePath.Contains(device))
                    return i;
            }

            return -1;
        }

        for (int i = 0; i < bindings.Count; i++)
        {
            bool groupOk = bindings[i].isComposite &&
                           (string.IsNullOrEmpty(group) || string.Equals(bindings[i].name, group, StringComparison.OrdinalIgnoreCase));

            if (!groupOk)
                continue;

            for (int j = i + 1; j < bindings.Count && bindings[j].isPartOfComposite; j++)
            {
                if (!string.Equals(bindings[j].name, part, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (string.IsNullOrEmpty(device) || (bindings[j].effectivePath != null && bindings[j].effectivePath.Contains(device)))
                    return j;
            }
        }

        return -1;
    }

    static List<Entry> DefaultEntriesFor(string map, string device)
    {
        var list = new List<Entry>();

        if (map == "Vehicle" && device == "Gamepad")
        {
            // buttons only: throttle/brake/steering stay on triggers + stick
            list.Add(new Entry("Handbrake", "Handbrake", "gamepads", "positive"));
            list.Add(new Entry("Nitro", "NOS", "gamepads", "positive"));
            list.Add(new Entry("Start Engine", "Start/Stop Engine", "", ""));
            list.Add(new Entry("Gear Up", "Gear Shift Up", "", ""));
            list.Add(new Entry("Gear Down", "Gear Shift Down", "", ""));
            list.Add(new Entry("Low Beams", "Low Beam Lights", "", ""));
            list.Add(new Entry("High Beams", "High Beam Lights", "", ""));
            list.Add(new Entry("Left Blinker", "Indicator Left", "", ""));
            list.Add(new Entry("Right Blinker", "Indicator Right", "", ""));
            list.Add(new Entry("Hazards", "Indicator Hazard", "", ""));
        }
        else if (map == "Vehicle")
        {
            list.Add(new Entry("Accelerate", "Throttle", "wasd", "positive"));
            list.Add(new Entry("Brake / Reverse", "Brake", "wasd", "positive"));
            list.Add(new Entry("Steer Left", "Steering", "wasd", "Negative"));
            list.Add(new Entry("Steer Right", "Steering", "wasd", "Positive"));
            list.Add(new Entry("Handbrake", "Handbrake", "keyboard", "positive"));
            list.Add(new Entry("Nitro", "NOS", "keyboard", "positive"));
            list.Add(new Entry("Start Engine", "Start/Stop Engine", "", ""));
            list.Add(new Entry("Gear Up", "Gear Shift Up", "", ""));
            list.Add(new Entry("Gear Down", "Gear Shift Down", "", ""));
            list.Add(new Entry("Low Beams", "Low Beam Lights", "", ""));
            list.Add(new Entry("High Beams", "High Beam Lights", "", ""));
            list.Add(new Entry("Left Blinker", "Indicator Left", "", ""));
            list.Add(new Entry("Right Blinker", "Indicator Right", "", ""));
            list.Add(new Entry("Hazards", "Indicator Hazard", "", ""));
        }
        else if (map == "Player")
        {
            list.Add(new Entry("Forward", "Move", "2D Vector", "up"));
            list.Add(new Entry("Backward", "Move", "2D Vector", "down"));
            list.Add(new Entry("Left", "Move", "2D Vector", "left"));
            list.Add(new Entry("Right", "Move", "2D Vector", "right"));
            list.Add(new Entry("Jump", "Jump", "", ""));
        }

        return list;
    }
}
