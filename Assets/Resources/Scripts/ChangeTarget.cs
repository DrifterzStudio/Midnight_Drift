using UnityEngine;

public class ChangeTarget : MonoBehaviour
{
    [SerializeField]
    private RCCP_Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //CarInstance.instance.gameObject.SetActive(true);
        //cam.cameraTarget.playerVehicle = CarInstance.instance;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
