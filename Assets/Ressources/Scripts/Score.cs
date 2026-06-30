using System;
using TMPro;
using UnityEngine;

public class Score : RCCP_GenericComponent
{
    private RCCP_CarController carController;

    private float metters = 0;
    private float distDrift = 0;
    private float timer = 0;

    private float score = 0;
    private float scoreUpdate = 0;
    private float scoreTotal = 0;

    private float distMultiplierModifier = 0;
    private float scoreMultiplierModifier = 0;
    private float multiplier = 1;
    private float challengeMultiplier = 1;
    private float scoreMultiplier = 1;

    private bool[] scoreAchievements = { false, false, false }; // 10000 / 5000 / 1000
    private bool[] distAchievements = { false, false, false }; // 200 / 100 / 50
    private bool[] parkingChallenge = { false, false }; // dist 30m / 5000 pts

    private bool isChallengeMultAply = false;
    private bool isEnd = false;

    [Tooltip("Minimum sideways slip required to count as drifting.")]
    public float driftSlipThreshold = 0.25f;

    [Tooltip("Time without drifting before the current run banks into total score.")]
    public float driftTimeoutDelay = 2f;

    [Tooltip("Text showing the current score.")]
    [Space()]
    public TMP_Text scoreText;

    [Tooltip("Text showing the points that will add to the score.")]
    [Space()]
    public TMP_Text scoreUpdateText;

    [Tooltip("Text showing the current multiplier, separated from the points popup.")]
    public TMP_Text multiplierText;

    [Header("Score Popup Animation")]
    public float popupFadeOutDuration = 0.5f;
    public float popupScalePunch = 1.3f;

    private CanvasGroup scoreUpdateCanvasGroup;
    private RectTransform scoreUpdateRect;
    private float fadeTimer = 0f;
    private bool isFadingOut = false;
    private Vector3 baseScale;

    private void Start()
    {
        scoreUpdateCanvasGroup = scoreUpdateText.GetComponent<CanvasGroup>();
        if (!scoreUpdateCanvasGroup)
            scoreUpdateCanvasGroup = scoreUpdateText.gameObject.AddComponent<CanvasGroup>();

        scoreUpdateRect = scoreUpdateText.GetComponent<RectTransform>();
        baseScale = scoreUpdateRect.localScale;

        scoreUpdateText.gameObject.SetActive(false);
        multiplierText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Getting active player car controller on the scene.
        carController = RCCPSceneManager.activePlayerVehicle;

        // If no active player car found, return.
        if (!carController)
            return;

        if (!isEnd)
        {
            if (IsDrifting())
            {
                AccumulateDrift();
            }
            else
            {
                HandleDriftTimeout();
            }
        }
        else if (!isChallengeMultAply)
        {
            ApplyFinalMultipliers();
        }

        UpdateMultiplier();
        UpdateUI();
        UpdatePopupAnimation();
    }

    private bool IsDrifting()
    {
        return Mathf.Abs((float)carController.PoweredAxles[0].leftWheelCollider.SidewaysSlip) >= driftSlipThreshold
            && carController.speed >= 0;
    }

    private void AccumulateDrift()
    {
        metters += (Mathf.Abs(carController.speed) / 3.6f) * Time.deltaTime;
        timer = 0;
        distDrift += metters;
        scoreUpdate = (int)distDrift * scoreMultiplier;
        metters = 0;
    }

    private void HandleDriftTimeout()
    {
        timer += Time.deltaTime;
        if (timer >= driftTimeoutDelay)
        {
            BankDriftScore();
        }
    }

    private void BankDriftScore()
    {
        score += (int)distDrift * scoreMultiplier;
        scoreUpdate = 0;
        distDrift = 0;
        distMultiplierModifier = 0;
        scoreMultiplierModifier = 0;
        scoreMultiplier = 1;
        timer = 0;

        StartPopupFadeOut();
    }

    private void ApplyFinalMultipliers()
    {
        score *= multiplier;
        score *= challengeMultiplier;
        isChallengeMultAply = true;
    }

    private void UpdateUI()
    {
        scoreTotal = score;
        scoreText.text = scoreTotal.ToString("N0");

        bool isCurrentlyDrifting = distDrift > 0 || metters > 0;

        if (isCurrentlyDrifting && !isFadingOut)
        {
            scoreUpdateText.gameObject.SetActive(true);
            scoreUpdateCanvasGroup.alpha = 1f;
            scoreUpdateRect.localScale = baseScale;
        }

        if (!isCurrentlyDrifting && !isFadingOut)
            return;

        scoreUpdateText.text = "+" + ((int)distDrift).ToString("N0");

        if (scoreMultiplier > 1)
        {
            multiplierText.gameObject.SetActive(true);
            multiplierText.text = "x" + scoreMultiplier.ToString("0.0");
        }
        else
        {
            multiplierText.gameObject.SetActive(false);
        }
    }

    private void StartPopupFadeOut()
    {
        fadeTimer = 0f;
        isFadingOut = true;
        scoreUpdateCanvasGroup.alpha = 1f;
    }

    private void UpdatePopupAnimation()
    {
        if (!isFadingOut)
            return;

        fadeTimer += Time.deltaTime;
        float t = fadeTimer / popupFadeOutDuration;

        scoreUpdateCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
        scoreUpdateRect.localScale = Vector3.Lerp(baseScale * popupScalePunch, baseScale, t);

        if (t >= 1f)
        {
            isFadingOut = false;
            scoreUpdateText.gameObject.SetActive(false);
            scoreUpdateRect.localScale = baseScale;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Finish"))
            isEnd = true;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Finish"))
            isEnd = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
            BankDriftScore();
    }

    private void UpdateMultiplier()
    {
        // Calculate the score achievements multiplier.
        if (score >= 10000 && !scoreAchievements[0]) { multiplier += 3; scoreAchievements[0] = true; }
        if (score >= 5000 && !scoreAchievements[1]) { multiplier += 2.5f; scoreAchievements[1] = true; }
        if (score >= 1000 && !scoreAchievements[2]) { multiplier += 2; scoreAchievements[2] = true; }

        // Calculate the distance achievements multiplier.
        if (metters >= 200 && !distAchievements[0]) { multiplier += 1.8f; distAchievements[0] = true; }
        if (metters >= 100 && !distAchievements[1]) { multiplier += 1.2f; distAchievements[1] = true; }
        if (metters >= 50 && !distAchievements[2]) { multiplier += 1; distAchievements[2] = true; }

        // Calculate the parking challenge multiplier.
        if (metters >= 30 && !parkingChallenge[0]) { challengeMultiplier *= 1.5f; parkingChallenge[0] = true; }
        if (score >= 5000 && !parkingChallenge[1]) { challengeMultiplier *= 2; parkingChallenge[1] = true; }

        // Calculate the score multiplier with distance.
        if (distMultiplierModifier < distDrift && distMultiplierModifier + 100 <= distDrift)
        {
            distMultiplierModifier += 100;
            multiplier += distMultiplierModifier / 200;
        }

        // Calculate the score multiplier with score.
        if (scoreMultiplierModifier < distDrift && scoreMultiplierModifier + 150 <= distDrift)
        {
            scoreMultiplierModifier += 150;
            scoreMultiplier = 1 + scoreMultiplierModifier / 300;
        }
    }
}