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

public class Singleton_Obj_MirrorManager<T> : Mirror.NetworkManager where T : Mirror.NetworkManager
{
    public static T Instance;

    protected void Awake()
    {
        base.Awake();
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this as T;
    }
}
