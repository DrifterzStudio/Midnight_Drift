using UnityEngine;
using UnityEngine.InputSystem;

// free-fly camera for a circuit tour. hold right mouse to look, WASD move, Q/E up/down, shift boost,
// scroll speed. put it on a Camera and enable it when filming.
public class FreeCamera : MonoBehaviour
{
    [Tooltip("Base move speed, in units per second.")]
    public float moveSpeed = 15f;

    [Tooltip("Speed multiplier while holding shift.")]
    public float boostMultiplier = 3f;

    [Tooltip("Up/down speed for Q/E (or Ctrl/Space).")]
    public float verticalSpeed = 10f;

    [Tooltip("Mouse look sensitivity.")]
    public float lookSensitivity = 0.1f;

    private float _yaw;
    private float _pitch;

    void OnEnable()
    {
        // start from wherever the camera is currently facing
        Vector3 angles = transform.eulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        Keyboard kb = Keyboard.current;

        if (mouse == null || kb == null)
            return;

        // lock the cursor while flying so the mouse doesn't hit the screen edge
        if (mouse.rightButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (mouse.rightButton.wasReleasedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // only fly while holding right mouse, like the scene view
        if (!mouse.rightButton.isPressed)
            return;

        Look(mouse);
        Move(mouse, kb);
    }

    void Look(Mouse mouse)
    {
        Vector2 delta = mouse.delta.ReadValue() * lookSensitivity;
        _yaw += delta.x;
        _pitch -= delta.y;
        _pitch = Mathf.Clamp(_pitch, -89f, 89f);
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    void Move(Mouse mouse, Keyboard kb)
    {
        // scroll nudges the base speed up/down
        float scroll = mouse.scroll.ReadValue().y;
        if (scroll != 0f)
            moveSpeed = Mathf.Clamp(moveSpeed + scroll * 0.01f, 1f, 200f);

        float speed = moveSpeed;
        if (kb.leftShiftKey.isPressed)
            speed *= boostMultiplier;

        Vector3 dir = Vector3.zero;
        if (kb.wKey.isPressed) dir += transform.forward;
        if (kb.sKey.isPressed) dir -= transform.forward;
        if (kb.dKey.isPressed) dir += transform.right;
        if (kb.aKey.isPressed) dir -= transform.right;

        transform.position += dir.normalized * speed * Time.deltaTime;

        // up / down, independent of where the camera looks
        if (kb.eKey.isPressed || kb.spaceKey.isPressed)
            transform.position += Vector3.up * verticalSpeed * Time.deltaTime;
        if (kb.qKey.isPressed || kb.leftCtrlKey.isPressed)
            transform.position -= Vector3.up * verticalSpeed * Time.deltaTime;
    }
}
