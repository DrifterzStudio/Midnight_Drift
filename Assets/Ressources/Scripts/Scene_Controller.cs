using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class Scene_Controller : MonoBehaviour
{
    #region Singleton
    public static Scene_Controller Instance;
    private void Awake()
    {
       
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    [SerializeField] private Loading_Overlay overlay;
    [SerializeField] private float waitingTime = 0.5f;
    private Dictionary<string, string> loadedSceneBySlot = new Dictionary<string, string>();
    private bool isBusy = false;

    public SceneTransition NewTransition()
    {
        return new SceneTransition();
    }
    private Coroutine ExecutePlan(SceneTransition transition)
    {
        if(isBusy)
        {
            Debug.LogWarning("already in transition");
            return null;
        }
        isBusy = true;
        return StartCoroutine(ChangeSceneRoutine(transition));
    }
    private IEnumerator ChangeSceneRoutine(SceneTransition transition)
    {
        if(transition.Overlay)
        {
            yield return overlay.FadeInBlack();
            yield return new WaitForSeconds(waitingTime);
        }
        foreach(var slotKey in transition.ScenesToUnload)
        {
            yield return UnloadSceneRoutine(slotKey);
        }
        if(transition.ClearUnusedAssets)
        {
            yield return CleanUnusedAssetsRoutine();
        }
        foreach (var key in transition.ScenesToLoad)
        {
            if (loadedSceneBySlot.ContainsKey(key.Key))
            {
                yield return UnloadSceneRoutine(key.Key);
            }
            yield return LoadAdditiveRoutine(key.Key,key.Value,transition.ActiveSceneName == key.Value);
        }
        if (transition.Overlay)
        {
            yield return overlay.FadeOutBlack();
        }
        isBusy = false;
    }

    private IEnumerator LoadAdditiveRoutine(string key,string sceneName,bool isActive)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
        if (loadOp == null) yield break;
        loadOp.allowSceneActivation = false;
        while (loadOp.progress < 0.9f)
        {
            yield return null;
        }
        loadOp.allowSceneActivation = true;
        while(!loadOp.isDone)
        {
            yield return null;
        }
        if(isActive)
        {
            UnityEngine.SceneManagement.Scene newScene = SceneManager.GetSceneByName(sceneName);
            if(newScene.IsValid() && newScene.isLoaded)
            {
                SceneManager.SetActiveScene(newScene);
            }
        }
        loadedSceneBySlot[key] = sceneName;
    }

    private IEnumerator UnloadSceneRoutine(string key)
    {
        if (!loadedSceneBySlot.TryGetValue(key, out string sceneName)) yield break;
        if(string.IsNullOrEmpty(sceneName)) yield break;
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);
        if (unloadOp != null)
        {
            while(!unloadOp.isDone)
            {
                yield return null;
            }
        }
        loadedSceneBySlot.Remove(key);
    }

    private IEnumerator CleanUnusedAssetsRoutine()
    {
        AsyncOperation cleanupOp = Resources.UnloadUnusedAssets();
        while(!cleanupOp.isDone)
        {
            yield return null;
        }
    }

    public class SceneTransition
    {
        public Dictionary<string, string> ScenesToLoad { get; } = new Dictionary<string, string>();
        public List<string> ScenesToUnload { get; } = new List<string>();
        public string ActiveSceneName { get; private set; } = "";
        public bool ClearUnusedAssets { get; private set; } = false;
        public bool Overlay { get; private set; } = false;

        public SceneTransition Load(string key,string sceneName, bool setActive = false)
        {
            ScenesToLoad[key] = sceneName;
            if(setActive) ActiveSceneName = sceneName;
            return this;
        }

        public SceneTransition Unload(string key)
        {
            ScenesToUnload.Add(key);
            return this;
        }

        public SceneTransition EnableOverlay(bool enable)
        {
            Overlay = enable;
            return this;
        }

        public SceneTransition ClearAssets(bool enable)
        {
            ClearUnusedAssets = enable;
            return this;
        }

        public Coroutine Execute()
        {
            return Scene_Controller.Instance.ExecutePlan(this);

        }

    }


}
