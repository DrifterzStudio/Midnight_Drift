using Mirror.Examples.RigidbodyBenchmark;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Mirror;
using Unity.VisualScripting;


//[RequireComponent(typeof(CharacterController))]
public class FPSController : NetworkBehaviour
{
    [Header("Références")]
    [SerializeField] private Transform cameraHolder;

    [Header("Déplacement")]
    [SerializeField] private float walkSpeed = 5f;

    [Header("Vue (souris)")]
    [SerializeField] private float lookSensitivity = 0.12f;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;

    //private CharacterController controller;
    private Rigidbody Rigidbody;
    private float pitch;

    //hook 
    private Vector2 _input = new Vector2();
    private Vector2 _look = new Vector2();
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();

    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (!isLocalPlayer)
            cameraHolder.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(isLocalPlayer && NetworkClient.ready)
        {
            HandleLook();
            HandleMove();
        }
          
    }

    private void OnCollisionEnter(Collision collision)
    {
        

    }

    private void HandleLook()
    {

        // Rotation gauche/droite (yaw) sur le corps du joueur
        transform.Rotate(Vector3.up * (_look.x * lookSensitivity));

        // Rotation haut/bas (pitch) sur la caméra uniquement
        pitch -= _look.y * lookSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraHolder.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    [Command]
    private void ServerMove(Vector2 input, Vector3 forward,Vector3 right)
    {
        Vector3 move = forward * input.y + right * input.x;
        Rigidbody.Move(Rigidbody.position + move * walkSpeed * Time.deltaTime, Rigidbody.rotation);
    }
    private void HandleMove()
    {
        Vector3 camRight = new Vector3(cameraHolder.right.x, 0, cameraHolder.right.z);
        Vector3 camForward = new Vector3(cameraHolder.forward.x, 0, cameraHolder.forward.z);

        ServerMove(_input, camForward, camRight);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _look = context.ReadValue<Vector2>();
    }
}