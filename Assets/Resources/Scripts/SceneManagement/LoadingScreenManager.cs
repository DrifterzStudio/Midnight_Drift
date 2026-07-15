using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance { get; private set; }

    [Header("UI")]
    public GameObject loadingPanel;
    public Slider progressBar;
    public TMP_Text progressText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        loadingPanel.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingPanel.SetActive(true);
        progressBar.value = 0f;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            progressBar.value = progress;
            if (progressText != null)
                progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";

            if (op.progress >= 0.9f)
            {
                progressBar.value = 1f;
                yield return new WaitForSeconds(0.2f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }

        loadingPanel.SetActive(false);
    }
}