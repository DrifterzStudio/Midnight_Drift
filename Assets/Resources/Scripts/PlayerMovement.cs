using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mouvement")]
    public float speed = 5f;

    [Header("Souris")]
    public float mouseSensitivity = 2f;

    [Header("Bruits de pas")]
    public float footstepThreshold = 0.5f;
    [Range(0f, 1f)] public float footstepVolume = 0.6f;

    private Rigidbody rb;
    private Camera cam;
    private float xRotation = 0f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool hasStarted = false;

    private AudioSource footstepSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();

        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.clip = Resources.Load<AudioClip>("Musics/Effects/steps-on-the-floor-with-the-sound-of-pants");
        footstepSource.loop = true;
        footstepSource.playOnAwake = false;
        footstepSource.spatialBlend = 0f;
        footstepSource.volume = footstepVolume;

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
        HandleFootsteps();

        // don't turn the camera while paused (timeScale 0)
        if (Time.timeScale == 0f)
            return;

        xRotation -= lookInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);
    }

    void HandleFootsteps()
    {
        if (footstepSource == null || footstepSource.clip == null)
            return;

        Vector3 flat = rb.linearVelocity;
        flat.y = 0f;
        bool walking = Time.timeScale > 0f && flat.magnitude > footstepThreshold;

        if (walking && !footstepSource.isPlaying)
            footstepSource.Play();
        else if (!walking && footstepSource.isPlaying)
            footstepSource.Pause();
    }

    void FixedUpdate()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move = move.normalized * speed;
        move.y = rb.linearVelocity.y;
        rb.linearVelocity = move;
    }
}