using UnityEngine;

public class Dont_Delete_Obj : MonoBehaviour
{
    protected void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
