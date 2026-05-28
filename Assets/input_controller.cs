// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerMovement : MonoBehaviour
// {
//     Vector2 moveInput;
//     public float speed = 5f;

//     void Update()
//     {
//         Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

//         transform.Translate(move * speed * Time.deltaTime);
//     }

//     void OnMove(InputAction.CallbackContext context)
//     {
//         moveInput = context.ReadValue<Vector2>();
//     }
// }

// using UnityEngine;
// using UnityEngine.InputSystem;
// using Unity.Netcode;

// public class PlayerMovement : NetworkBehaviour
// {
//     Vector2 moveInput;
//     public float speed = 5f;

//     void Update()
//     {
//      Debug.Log($"Owner: {IsOwner}, input: {moveInput}");
//         if (!IsOwner) return;

//         Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
//         transform.Translate(move * speed * Time.deltaTime);
//     }

//     public void OnMove(InputAction.CallbackContext context)
//     {
//         if (!IsOwner) return;

//         Debug.Log("MOVE CALLED");

//         moveInput = context.ReadValue<Vector2>();
//     }
// }

// using UnityEngine;
// using Unity.Netcode;
// using UnityEngine.InputSystem;

// public class PlayerMovement : NetworkBehaviour
// {
//     Vector2 moveInput;

//     void Update()
//     {
//         if (!IsOwner) return;

//         transform.Translate(new Vector3(moveInput.x, 0, moveInput.y));
//     }

//     public void OnMove(InputAction.CallbackContext context)
//     {
//         if (!IsOwner) return;

//         moveInput = context.ReadValue<Vector2>();
//     }

//     public override void OnNetworkSpawn()
//     {
//         if (!IsOwner)
//         {
//             GetComponent<PlayerInput>().enabled = false;
//         }
//     }
// }

// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerMovement : NetworkBehaviour
// {
//     Vector2 moveInput;

//     public override void OnNetworkSpawn()
//     {
//         if (!IsOwner)
//         {
//             GetComponent<PlayerInput>().enabled = false;
//         }
//         else
//         {
//             GetComponent<PlayerInput>().enabled = true;
//         }
//     }

//     void Update()
//     {
//             Debug.Log($"obj:{gameObject.name} owner:{IsOwner} input:{moveInput}");

//         if (!IsOwner) return;

//         transform.Translate(new Vector3(moveInput.x, 0, moveInput.y));
//     }

//     public void OnMove(InputAction.CallbackContext context)
//     {
//         if (!IsOwner) return;

//         moveInput = context.ReadValue<Vector2>();
//     }
// }

// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerMovement : NetworkBehaviour
// {
//     Vector2 moveInput;
//     public float speed = 5f;

//     void Update()
//     {
//         if (!IsOwner) return;

//         // client envoie input au serveur
//         SubmitMoveServerRpc(moveInput);
//     }

//     public void OnMove(InputAction.CallbackContext context)
//     {
//         if (!IsOwner) return;

//         moveInput = context.ReadValue<Vector2>();
//         SubmitMoveServerRpc(moveInput);
//     }
//     [ServerRpc]
//     void SubmitMoveServerRpc(Vector2 input)
//     {
//         Debug.Log("SERVER RECEIVED INPUT: " + input);

//         Move(input);
//     }
//     void Move(Vector2 input)
//     {
//         Vector3 move = new Vector3(input.x, 0, input.y);
//         transform.position += move * speed * Time.deltaTime;
//     }

// }

using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    Vector2 moveInput;
    public float speed = 5f;

    void Update()
    {
        if (!IsOwner) return;

        // envoie en continu au serveur
        SubmitMoveServerRpc(moveInput);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        moveInput = context.ReadValue<Vector2>();
        SubmitMoveServerRpc(moveInput);
    }

    [ServerRpc]
    void SubmitMoveServerRpc(Vector2 input)
    {
        Vector3 move = new Vector3(input.x, 0, input.y);

        transform.position += move * speed * Time.deltaTime;
    }
}