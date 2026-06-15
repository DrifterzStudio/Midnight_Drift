using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gestionnaire du fondu noir entre les scènes.
/// Créé automatiquement par ZoneChangementScene — ne pas placer en scène manuellement.
/// Survit aux changements de scène (DontDestroyOnLoad).
/// </summary>
public class FonduTransition : MonoBehaviour
{
    // ──────────────────────────────────────────
    // SINGLETON
    // ──────────────────────────────────────────

    private static FonduTransition _instance;

    public static FonduTransition Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("FonduTransition");
                _instance = go.AddComponent<FonduTransition>();
            }
            return _instance;
        }
    }

    // ──────────────────────────────────────────
    // COMPOSANTS UI
    // ──────────────────────────────────────────

    private Image ecranNoir;

    // ──────────────────────────────────────────
    // INITIALISATION
    // ──────────────────────────────────────────

    void Awake()
    {
        // Éviter les doublons si la scène est rechargée
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        ConstruireCanvas();

        // S'abonner à l'événement de chargement de scène → fondu d'apparition
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void ConstruireCanvas()
    {
        // Créer le Canvas (overlay — toujours au premier plan)
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();

        // Créer l'image noire plein écran
        GameObject imgGO = new GameObject("EcranNoir");
        imgGO.transform.SetParent(transform, false);

        ecranNoir = imgGO.AddComponent<Image>();
        ecranNoir.color = new Color(0f, 0f, 0f, 0f);   // invisible au départ

        RectTransform rt = ecranNoir.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // ──────────────────────────────────────────
    // ÉVÉNEMENTS
    // ──────────────────────────────────────────

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Dès qu'une nouvelle scène est chargée → fondu vers la clarté
        StartCoroutine(FondreDepuisNoir(0.6f));
    }

    // ──────────────────────────────────────────
    // API PUBLIQUE
    // ──────────────────────────────────────────

    /// <summary>Anime le fondu de transparent vers noir opaque.</summary>
    public IEnumerator FondreVersNoir(float duree)
    {
        yield return StartCoroutine(AnimerAlpha(0f, 1f, duree));
    }

    /// <summary>Anime le fondu de noir opaque vers transparent.</summary>
    public IEnumerator FondreDepuisNoir(float duree)
    {
        yield return StartCoroutine(AnimerAlpha(1f, 0f, duree));
    }

    // ──────────────────────────────────────────
    // ANIMATION INTERNE
    // ──────────────────────────────────────────

    IEnumerator AnimerAlpha(float depart, float arrivee, float duree)
    {
        float elapsed = 0f;

        while (elapsed < duree)
        {
            elapsed += Time.deltaTime;
            // SmoothStep = accélération/décélération douce
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duree));
            ecranNoir.color = new Color(0f, 0f, 0f, Mathf.Lerp(depart, arrivee, t));
            yield return null;
        }

        // Garantir la valeur finale exacte
        ecranNoir.color = new Color(0f, 0f, 0f, arrivee);
    }

    // ──────────────────────────────────────────
    // NETTOYAGE
    // ──────────────────────────────────────────

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}