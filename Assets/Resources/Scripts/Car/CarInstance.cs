using UnityEngine;

public class CarInstance : MonoBehaviour
{
    public static RCCP_CarController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<RCCP_CarController>();
        }

    }
}
