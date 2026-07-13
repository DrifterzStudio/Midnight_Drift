using UnityEngine;

public class SceneLoaderButton : MonoBehaviour, IInteractable
{
    [Header("Scène à charger")]
    public string sceneName = "Circuit";

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
        LoadingScreenManager.Instance.LoadScene(sceneName);
    }
}