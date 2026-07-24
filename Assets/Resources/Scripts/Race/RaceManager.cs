using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// solo race for Circuit_Solo. runs a LapTracker, shows the lap hud + the end screen with the
// scoreboard. spawns itself into the scene and reads the finish line from the FinishLine marker.
public class RaceManager : MonoBehaviour
{
    const string RaceScene = "Circuit_Solo";

    [Header("Race")]
    public int maxLaps = 5;

    [Tooltip("Distance (m) the car must reach from the line before a crossing counts.")]
    public float leaveDistance = 25f;

    [Tooltip("Half-width (m) of the auto line; crossings farther than this from the start don't count.")]
    public float lineHalfWidth = 25f;

    [Header("Countdown")]
    [Tooltip("Seconds shown per countdown number (3, 2, 1).")]
    public float countdownStep = 1f;

    [Header("Score medals")]
    public float bronzeScore = 40000f;
    public float silverScore = 65000f;
    public float goldScore = 90000f;
    public float platinumScore = 125000f;
    public float diamondScore = 165000f;

    static RaceManager instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void SubscribeToScenes()
    {
        instance = null;
        SceneManager.sceneLoaded -= OnAnySceneLoaded;
        SceneManager.sceneLoaded += OnAnySceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void SpawnForInitialScene()
    {
        TrySpawnFor(SceneManager.GetActiveScene());
    }

    static void OnAnySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrySpawnFor(scene);
    }

    static void TrySpawnFor(Scene scene)
    {
        if (scene.name != RaceScene) return;
        if (instance != null) return;
        if (FindAnyObjectByType<RaceManager>() != null) return;

        GameObject go = new GameObject("RaceManager (auto)");
        if (scene.IsValid() && scene.isLoaded)
            SceneManager.MoveGameObjectToScene(go, scene);
        go.AddComponent<RaceManager>();
    }

    RCCP_CarController playerCar;
    Score scoreComponent;
    LapTracker tracker;

    bool isSetup;
    bool leaving;

    Canvas hudCanvas;
    Text lapText;
    Text timeText;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    void Update()
    {
        if (!isSetup)
        {
            TrySetup();
            return;
        }

        if (tracker.Finished) return;

        tracker.Tick(playerCar.transform.position, Time.deltaTime);

        if (!tracker.Finished)
            UpdateHUD();
    }

    void TrySetup()
    {
        RCCP_CarController car = GetPlayerCar();
        if (car == null) return;

        playerCar = car;
        scoreComponent = FindAnyObjectByType<Score>();

        tracker = new LapTracker
        {
            MaxLaps = maxLaps,
            LeaveDistance = leaveDistance,
            LineHalfWidth = lineHalfWidth,
        };
        tracker.RaceFinished += EndRace;

        FinishLine line = FindAnyObjectByType<FinishLine>();
        if (line != null)
        {
            tracker.SetLine(line.transform.position, line.transform.forward);
        }
        else
        {
            Debug.LogWarning("RaceManager: no FinishLine in the scene — falling back to the car's start pose. Place a FinishLine on the track.");
            tracker.SetLine(car.transform.position, car.transform.forward);
        }

        BuildHUD();

        playerCar.canControl = false;
        StartCoroutine(CountdownRoutine());

        isSetup = true;
    }

    IEnumerator CountdownRoutine()
    {
        Canvas canvas = SimpleUI.CreateOverlayCanvas("Countdown", transform, 20000);
        Text label = SimpleUI.AddText(canvas.transform, "", 160, TextAnchor.MiddleCenter, FontStyle.Bold);
        RectTransform rt = label.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        for (int n = 3; n >= 1; n--)
        {
            label.text = n.ToString();
            yield return new WaitForSeconds(countdownStep);
        }

        label.text = "GO!";
        label.color = new Color(0.4f, 1f, 0.4f);

        if (playerCar != null)
            playerCar.canControl = true;

        yield return new WaitForSeconds(0.6f);

        if (canvas != null)
            Destroy(canvas.gameObject);
    }

    static RCCP_CarController GetPlayerCar()
    {
        RCCP_SceneManager sm = RCCP_SceneManager.Instance;
        if (sm == null) return null;
        return sm.activePlayerVehicle;
    }

    void EndRace()
    {
        if (playerCar != null)
            playerCar.canControl = false;

        float finalScore = scoreComponent != null ? scoreComponent.FinalizeAndGetScore() : 0f;

        Scoreboard.Entry entry = new Scoreboard.Entry
        {
            score = finalScore,
            totalTime = tracker.RaceTime,
            bestLap = tracker.BestLap,
            vehicle = GameSession.SelectedVehicle != null ? GameSession.SelectedVehicle.displayName : "?",
            dateTicks = System.DateTime.UtcNow.Ticks,
        };
        int rank = Scoreboard.Add(entry);

        if (hudCanvas != null)
            hudCanvas.gameObject.SetActive(false);

        BuildResults(finalScore, tracker.RaceTime, tracker.BestLap, rank);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void BuildHUD()
    {
        hudCanvas = SimpleUI.CreateOverlayCanvas("RaceHUD", transform, 100);

        GameObject holder = SimpleUI.NewUI("LapHolder", hudCanvas.transform);
        Image bg = holder.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.35f);

        RectTransform rt = holder.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-24f, -24f);
        rt.sizeDelta = new Vector2(300f, 110f);

        VerticalLayoutGroup vlg = holder.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(12, 12, 10, 10);
        vlg.spacing = 2f;
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        lapText = SimpleUI.AddText(holder.transform, "LAP 1/" + maxLaps, 38, TextAnchor.MiddleCenter, FontStyle.Bold);
        AddLayoutHeight(lapText.gameObject, 48f);

        timeText = SimpleUI.AddText(holder.transform, SimpleUI.FormatTime(0f), 30, TextAnchor.MiddleCenter);
        AddLayoutHeight(timeText.gameObject, 38f);
    }

    void UpdateHUD()
    {
        if (lapText != null)
            lapText.text = "LAP " + Mathf.Min(tracker.CurrentLap, maxLaps) + "/" + maxLaps;
        if (timeText != null)
            timeText.text = SimpleUI.FormatTime(tracker.RaceTime);
    }

    void BuildResults(float score, float totalTime, float bestLapTime, int rank)
    {
        SimpleUI.EnsureEventSystem(transform);
        Canvas canvas = SimpleUI.CreateOverlayCanvas("RaceResults", transform, 25000);

        GameObject dim = SimpleUI.NewUI("Dim", canvas.transform);
        Image dimImg = dim.AddComponent<Image>();
        dimImg.color = new Color(0f, 0f, 0f, 0.85f);
        SimpleUI.Stretch(dim.GetComponent<RectTransform>());

        GameObject panel = SimpleUI.NewUI("Panel", dim.transform);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.09f, 0.10f, 0.13f, 0.98f);
        RectTransform prt = panel.GetComponent<RectTransform>();
        prt.anchorMin = prt.anchorMax = new Vector2(0.5f, 0.5f);
        prt.pivot = new Vector2(0.5f, 0.5f);
        prt.sizeDelta = new Vector2(760f, 0f);

        VerticalLayoutGroup vlg = panel.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(45, 45, 36, 36);
        vlg.spacing = 12f;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        ContentSizeFitter fitter = panel.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        AddResultLine(panel.transform, "RACE FINISHED", 44, FontStyle.Bold, 64f);
        AddResultLine(panel.transform, "Score: " + score.ToString("N0"), 32, FontStyle.Bold, 46f);
        AddResultLine(panel.transform, GetMedalText(score), 28, FontStyle.Bold, 42f).color = GetMedalColor(score);
        AddResultLine(panel.transform, "Total time: " + SimpleUI.FormatTime(totalTime), 26, FontStyle.Normal, 36f);
        AddResultLine(panel.transform, "Best lap: " + SimpleUI.FormatTime(bestLapTime), 26, FontStyle.Normal, 36f);
        AddResultLine(panel.transform, "Rank: #" + rank, 26, FontStyle.Normal, 36f);

        AddResultLine(panel.transform, "— LEADERBOARD —", 24, FontStyle.Bold, 44f);

        List<Scoreboard.Entry> top = Scoreboard.Top(5);
        if (top.Count == 0)
            AddResultLine(panel.transform, "(none)", 22, FontStyle.Normal, 30f);

        for (int i = 0; i < top.Count; i++)
        {
            Scoreboard.Entry e = top[i];
            string medal = "<color=#" + ColorUtility.ToHtmlStringRGB(GetMedalColor(e.score)) + ">" + GetMedalName(e.score) + "</color>";
            string row = (i + 1) + ".  " + medal + "   " + e.score.ToString("N0") + "   " + SimpleUI.FormatTime(e.totalTime) + "   " + e.vehicle;
            AddResultLine(panel.transform, row, 22, FontStyle.Normal, 30f);
        }

        SimpleUI.AddButton(panel.transform, "Restart", () => LeaveTo(RaceScene));
        SimpleUI.AddButton(panel.transform, "Back to Garage", () => LeaveTo("Garage"));
        SimpleUI.AddButton(panel.transform, "Back to Menu", () => LeaveTo("MainMenu"));
    }

    static Text AddResultLine(Transform parent, string text, int size, FontStyle style, float height)
    {
        Text label = SimpleUI.AddText(parent, text, size, TextAnchor.MiddleCenter, style);
        AddLayoutHeight(label.gameObject, height);
        return label;
    }

    string GetMedalName(float score)
    {
        if (score >= diamondScore) return "DIAMOND";
        if (score >= platinumScore) return "PLATINUM";
        if (score >= goldScore) return "GOLD";
        if (score >= silverScore) return "SILVER";
        if (score >= bronzeScore) return "BRONZE";
        return "-";
    }

    string GetMedalText(float score)
    {
        if (score >= diamondScore) return "DIAMOND MEDAL";
        if (score >= platinumScore) return "PLATINUM - DIAMOND at " + diamondScore.ToString("N0");
        if (score >= goldScore) return "GOLD - PLATINUM at " + platinumScore.ToString("N0");
        if (score >= silverScore) return "SILVER - GOLD at " + goldScore.ToString("N0");
        if (score >= bronzeScore) return "BRONZE - SILVER at " + silverScore.ToString("N0");
        return "no medal - BRONZE at " + bronzeScore.ToString("N0");
    }

    Color GetMedalColor(float score)
    {
        if (score >= diamondScore) return new Color(0.5f, 0.9f, 1f);      // diamond - cyan
        if (score >= platinumScore) return new Color(0.9f, 0.93f, 0.97f); // platinum - near white
        if (score >= goldScore) return new Color(1f, 0.84f, 0.2f);        // gold
        if (score >= silverScore) return new Color(0.75f, 0.76f, 0.8f);   // silver
        if (score >= bronzeScore) return new Color(0.85f, 0.55f, 0.25f);  // bronze
        return new Color(0.7f, 0.7f, 0.7f);
    }

    static void AddLayoutHeight(GameObject go, float height)
    {
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight = height;
        le.preferredHeight = height;
    }

    void LeaveTo(string sceneName)
    {
        if (leaving) return;
        leaving = true;

        Time.timeScale = 1f;

        if (LoadingScreenManager.Instance != null)
            LoadingScreenManager.Instance.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}
