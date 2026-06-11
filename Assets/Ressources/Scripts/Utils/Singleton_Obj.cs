using System;
using UnityEngine;

public class Singleton_Obj<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;

    protected void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this as T;
    }
}
