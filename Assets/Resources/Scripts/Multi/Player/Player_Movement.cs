using Mirror;
using Mirror.BouncyCastle.Asn1.Crmf;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Player_Movement : NetworkBehaviour
{

    [Header("Ref")]
    [SerializeField] private Transform cameraHolder;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;

    [Header("Camera")]
    [SerializeField] private float lookSensitivity = 0.12f;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;

    [SyncVar] private Vector3 _position;

    private Vector2 _input;
    private Vector2 _look;

    private Rigidbody _rigidBody;

    // not syncVar the server store his own
    private float _pitch;


  
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //if (!isLocalPlayer)
        //    return;

        //transform.Rotate(Vector3.up * (_look.x * lookSensitivity));
        //_pitch -= _look.y * lookSensitivity;
        //_pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        //cameraHolder.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        //HandleMove();
    }

    private void HandleMove()
    {
        CmdSendInput(_input);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        _input = context.ReadValue<Vector2>();
    }
   public void OnLook(InputAction.CallbackContext context )
    {
        if (!isLocalPlayer) return;
        _look = context.ReadValue<Vector2>();
    }
    [Command]
    void CmdSendInput(Vector2 input)
    {
        Vector3 mooveDir = (transform.right * input.x + transform.forward *input.y) * walkSpeed;
        _rigidBody.MovePosition(_rigidBody.position + mooveDir *Time.fixedDeltaTime);
        _position = _rigidBody.position;
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Collided with: " + col.gameObject.name);
    }
}
