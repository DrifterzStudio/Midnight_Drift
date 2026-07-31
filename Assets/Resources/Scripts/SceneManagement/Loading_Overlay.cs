using UnityEngine;
using System.Collections;
public class Loading_Overlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 0.5f;
    public IEnumerator FadeInBlack()
    {
        canvasGroup.blocksRaycasts = true;
        yield return FadeTo(1f,fadeInTime);
    }
    public IEnumerator FadeOutBlack()
    {
        yield return FadeTo(0.0f, fadeOutTime);
        canvasGroup.blocksRaycasts = false;
    }
    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}
