using UnityEngine;

public class VehicleCustomizerButton : MonoBehaviour, IInteractable
{
    [Header("Canvas to display")]
    public GameObject vehicleCustomizerCanvas;
    public PlayerMovement player;

    [Header("Highlight")]
    public Renderer targetRenderer;
    public Color highlightColor = new Color(0.5f, 0f, 0f);
    [Range(0f, 5f)] public float emissionIntensity = 1.5f;

    private MaterialPropertyBlock _propBlock;
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        vehicleCustomizerCanvas.SetActive(false);
    }

    public void OnHoverEnter()
    {
        targetRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(EmissionColorId, highlightColor * emissionIntensity);
        targetRenderer.SetPropertyBlock(_propBlock);
    }

    public void OnHoverExit()
    {
        targetRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(EmissionColorId, Color.black);
        targetRenderer.SetPropertyBlock(_propBlock);
    }

    public void Interact()
    {
        bool willBeActive = !vehicleCustomizerCanvas.activeSelf;
        vehicleCustomizerCanvas.SetActive(willBeActive);

        if (player != null)
            player.enabled = !willBeActive;

        Cursor.lockState = willBeActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = willBeActive;
    }
}