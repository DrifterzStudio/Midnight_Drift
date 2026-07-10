using Mirror;
using UnityEngine;

public class StopVIsibility : NetworkBehaviour
{
    [SerializeField] private GameObject hostButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        hostButton.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
       
    }
}
