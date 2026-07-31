using System;
using UnityEngine;
using UnityEngine.InputSystem;

public static class KeyBindingStore
{
    public static event Action OnChanged;

    public static Key Get(string id, Key fallback)
    {
        string s = PlayerPrefs.GetString("key_" + id, "");

        Key k;
        if (!string.IsNullOrEmpty(s) && Enum.TryParse(s, out k) && k != Key.None)
            return k;

        return fallback;
    }

    public static void Set(string id, Key key)
    {
        PlayerPrefs.SetString("key_" + id, key.ToString());
        PlayerPrefs.Save();
        OnChanged?.Invoke();
    }

    public static void Clear(string id)
    {
        PlayerPrefs.DeleteKey("key_" + id);
        PlayerPrefs.Save();
        OnChanged?.Invoke();
    }
}
