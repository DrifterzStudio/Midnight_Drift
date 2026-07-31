using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindManager : MonoBehaviour
{
    public static RebindManager instance;

    [Serializable]
    public class Category
    {
        public string displayName = "Category";
        public InputActionAsset asset;
        public string saveKey = "rebinds";
    }

    public List<Category> categories = new List<Category>();

    public event Action OnReset;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAll();
    }

    public void LoadAll()
    {
        foreach (var c in categories)
        {
            if (c.asset == null)
                continue;

            string json = PlayerPrefs.GetString(c.saveKey, null);

            if (!string.IsNullOrEmpty(json))
                c.asset.LoadBindingOverridesFromJson(json);
        }
    }

    public void ResetAll()
    {
        foreach (var c in categories)
        {
            if (c.asset != null)
                c.asset.RemoveAllBindingOverrides();

            PlayerPrefs.DeleteKey(c.saveKey);
        }

        PlayerPrefs.Save();

        OnReset?.Invoke();
    }
}
