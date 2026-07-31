using Mirror;
using UnityEngine;

public class StopVIsibility : MonoBehaviour 
{
    [SerializeField] private GameObject hostButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if ((NetworkServer.active || NetworkClient.active) && hostButton.activeSelf)
            hostButton.SetActive(false);
    }
}

