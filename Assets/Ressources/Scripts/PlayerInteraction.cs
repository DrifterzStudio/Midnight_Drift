using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public Camera cam;
    public float interactionDistance = 3f;
    public LayerMask interactableLayer = ~0;

    private IInteractable _currentHover;

    void Update()
    {
        UpdateHover();

        if (Mouse.current.leftButton.wasPressedThisFrame && _currentHover != null)
        {
            _currentHover.Interact();
        }
    }

    void UpdateHover()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        IInteractable hit = null;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionDistance, interactableLayer))
        {
            hit = hitInfo.collider.GetComponent<IInteractable>();
        }

        if (hit != _currentHover)
        {
            _currentHover?.OnHoverExit();
            _currentHover = hit;
            _currentHover?.OnHoverEnter();
        }
    }
}