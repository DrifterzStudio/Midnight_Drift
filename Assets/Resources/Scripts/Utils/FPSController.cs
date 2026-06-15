using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Transform cameraHolder;

    [Header("Déplacement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Vue (souris)")]
    [SerializeField] private float lookSensitivity = 0.12f;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;

    private CharacterController controller;
    private PlayerControls controls;
    private Vector3 verticalVelocity;
    private float pitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
        HandleMove();
    }

    private void HandleLook()
    {
        Vector2 look = controls.Player.Look.ReadValue<Vector2>();

        // Rotation gauche/droite (yaw) sur le corps du joueur
        transform.Rotate(Vector3.up * (look.x * lookSensitivity));

        // Rotation haut/bas (pitch) sur la caméra uniquement
        pitch -= look.y * lookSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraHolder.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMove()
    {
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();
        Vector3 horizontalMove = (transform.right * input.x + transform.forward * input.y) * walkSpeed;

        if (controller.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;

        if (controls.Player.Jump.WasPressedThisFrame() && controller.isGrounded)
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity.y += gravity * Time.deltaTime;

        controller.Move((horizontalMove + verticalVelocity) * Time.deltaTime);
    }
}