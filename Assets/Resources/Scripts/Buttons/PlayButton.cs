using UnityEngine;

public class SceneLoaderButton : MonoBehaviour, IInteractable
{
    [Header("Scene to load")]
    public string sceneName = "Circuit_Solo";

    [Header("Optional - pick a car first")]
    [Tooltip("If set, interacting opens this menu instead of loading straight away. The menu loads the scene once a car is chosen.")]
    public CarSelectionMenu carSelectionMenu;

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
        if (carSelectionMenu != null)
            carSelectionMenu.Open();
        else
            LoadingScreenManager.Instance.LoadScene(sceneName);
    }
}