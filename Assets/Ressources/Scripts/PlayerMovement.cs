using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mouvement")]
    public float speed = 5f;

    [Header("Souris")]
    public float mouseSensitivity = 2f;

    private Rigidbody rb;
    private Camera cam;
    private float xRotation = 0f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool hasStarted = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        if (!hasStarted)
        {
            hasStarted = true;
            return;
        }

        lookInput = value.Get<Vector2>();
    }

    void Update()
    {
        xRotation -= lookInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);
    }

    void FixedUpdate()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move = move.normalized * speed;
        move.y = rb.linearVelocity.y;
        rb.linearVelocity = move;
    }
}