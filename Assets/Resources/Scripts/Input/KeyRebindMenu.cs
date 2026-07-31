using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class KeyRebindMenu : MonoBehaviour
{
    [Serializable]
    public class Entry
    {
        public string displayName = "Action";
        public string id = "";
        public Key defaultKey = Key.None;

        public Entry() { }
        public Entry(string display, string id, Key defaultKey)
        {
            displayName = display;
            this.id = id;
            this.defaultKey = defaultKey;
        }
    }

    [Header("UI")]
    public KeyRebindRow rowPrefab;
    public Transform content;
    public TextMeshProUGUI warningLabel;

    [Header("Section header (optional)")]
    public GameObject sectionHeaderPrefab;
    public string sectionTitle = "";

    [Header("Keys (empty = default radio list)")]
    public List<Entry> entries = new List<Entry>();

    private readonly List<KeyRebindRow> rows = new List<KeyRebindRow>();
    private bool built = false;

    void OnEnable()
    {
        KeyBindingStore.OnChanged += RefreshAll;

        if (RebindManager.instance != null)
            RebindManager.instance.OnReset += ResetToDefault;

        Build();
        RefreshAll();
    }

    void OnDisable()
    {
        KeyBindingStore.OnChanged -= RefreshAll;

        if (RebindManager.instance != null)
            RebindManager.instance.OnReset -= ResetToDefault;
    }

    void Build()
    {
        if (built || rowPrefab == null || content == null)
            return;

        SpawnSectionHeader();

        foreach (var e in GetList())
        {
            var row = Instantiate(rowPrefab, content);
            row.Setup(e.displayName, e.id, e.defaultKey, RefreshAll);
            rows.Add(row);
        }

        built = true;
    }

    public void ResetToDefault()
    {
        foreach (var e in GetList())
            KeyBindingStore.Clear(e.id);
    }

    void RefreshAll()
    {
        for (int i = 0; i < rows.Count; i++)
            rows[i].RefreshKeyLabel();

        RefreshConflicts();
    }

    void RefreshConflicts()
    {
        string dup = null;

        for (int i = 0; i < rows.Count; i++)
        {
            Key ki = rows[i].CurrentKey;
            bool conflict = false;

            for (int j = 0; j < rows.Count; j++)
            {
                if (j == i)
                    continue;

                if (rows[j].CurrentKey == ki)
                {
                    conflict = true;
                    break;
                }
            }

            rows[i].SetConflict(conflict);

            if (conflict && dup == null)
                dup = ki.ToString().ToUpperInvariant();
        }

        if (warningLabel != null)
            warningLabel.text = dup != null ? ("Duplicate key: " + dup) : "";
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

    List<Entry> GetList()
    {
        if (entries != null && entries.Count > 0)
            return entries;

        return new List<Entry>
        {
            new Entry("Radio On/Off", "radio_power", Key.R),
            new Entry("Next Track", "radio_next", Key.T),
            new Entry("Volume Up", "radio_volup", Key.PageUp),
            new Entry("Volume Down", "radio_voldown", Key.PageDown),
        };
    }
}
