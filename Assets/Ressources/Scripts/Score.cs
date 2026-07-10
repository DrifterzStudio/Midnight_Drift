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

    [Header("Score Banking Sound")]
    [Tooltip("Sound played when drift points are added to the total score.")]
    public AudioClip scoreBankSound;
    [Range(0f, 1f)]
    public float scoreBankSoundVolume = 1f;

    [Header("Score Lost Effect")]
    [Tooltip("Sound played when drift points are lost due to a collision.")]
    public AudioClip scoreLostSound;
    [Range(0f, 1f)]
    public float scoreLostSoundVolume = 1f;
    public Color scoreLostColor = Color.red;
    public float scoreLostShakeDuration = 0.3f;
    public float scoreLostShakeStrength = 10f;

    private AudioSource audioSource;

    private CanvasGroup scoreUpdateCanvasGroup;
    private RectTransform scoreUpdateRect;
    private float fadeTimer = 0f;
    private bool isFadingOut = false;
    private Vector3 baseScale;

    private Color baseTextColor;
    private float shakeTimer = 0f;
    private bool isShaking = false;
    private Vector3 popupBasePosition;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        scoreUpdateCanvasGroup = scoreUpdateText.GetComponent<CanvasGroup>();
        if (!scoreUpdateCanvasGroup)
            scoreUpdateCanvasGroup = scoreUpdateText.gameObject.AddComponent<CanvasGroup>();

        scoreUpdateRect = scoreUpdateText.GetComponent<RectTransform>();
        baseScale = scoreUpdateRect.localScale;
        popupBasePosition = scoreUpdateRect.anchoredPosition;
        baseTextColor = scoreUpdateText.color;

        scoreUpdateText.gameObject.SetActive(false);
        multiplierText.gameObject.SetActive(false);
    }

    private void Update()
    {
        carController = RCCPSceneManager.activePlayerVehicle;

        if (!carController)
            return;

        if (!isEnd)
        {
            if (IsDrifting() && !isShaking)
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
        UpdateShakeEffect();
    }

    private bool IsDrifting()
    {
        if (carController.PoweredAxles == null || carController.PoweredAxles.Count == 0)
            return false;

        return Mathf.Abs(carController.PoweredAxles[0].leftWheelCollider.SidewaysSlip) >= driftSlipThreshold
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
        if (isShaking || isFadingOut)
        {
            distDrift = 0;
            metters = 0;
            timer = 0;
            return;
        }

        if ((int)distDrift <= 0)
        {
            distDrift = 0;
            metters = 0;
            timer = 0;
            HidePopupImmediately();
            return;
        }

        timer += Time.deltaTime;
        if (timer >= driftTimeoutDelay)
        {
            BankDriftScore();
        }
    }

    private void BankDriftScore()
    {
        bool hasPointsToBank = (int)distDrift > 0;

        if (hasPointsToBank)
        {
            score += (int)distDrift * scoreMultiplier;
            PlayScoreBankSound();
        }

        scoreUpdate = 0;
        distDrift = 0;
        distMultiplierModifier = 0;
        scoreMultiplierModifier = 0;
        scoreMultiplier = 1;
        timer = 0;

        if (hasPointsToBank)
            StartPopupFadeOut();
        else
            HidePopupImmediately();
    }

    private void PlayScoreBankSound()
    {
        if (scoreBankSound && audioSource)
            audioSource.PlayOneShot(scoreBankSound, scoreBankSoundVolume);
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

        if (isShaking)
            return;

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

    [Tooltip("Layer used by circuit obstacles (props, barriers, tires, etc.).")]
    public LayerMask obstacleLayer;

    private void OnCollisionEnter(Collision collision)
    {
        if (IsInLayerMask(collision.gameObject.layer, obstacleLayer))
            CancelDriftScore();
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    private void CancelDriftScore()
    {
        if (isShaking)
            return;

        bool hadPointsToLose = (int)distDrift > 0;

        distDrift = 0;
        metters = 0;
        distMultiplierModifier = 0;
        scoreMultiplierModifier = 0;
        scoreMultiplier = 1;
        timer = 0;
        scoreUpdate = 0;

        if (hadPointsToLose)
            StartScoreLostEffect();
        else
            HidePopupImmediately();

        isFadingOut = false;
    }

    private void StartScoreLostEffect()
    {
        scoreUpdateText.gameObject.SetActive(true);
        scoreUpdateCanvasGroup.alpha = 1f;
        scoreUpdateText.color = scoreLostColor;

        shakeTimer = 0f;
        isShaking = true;

        PlayScoreLostSound();
    }

    private void PlayScoreLostSound()
    {
        if (scoreLostSound && audioSource)
            audioSource.PlayOneShot(scoreLostSound, scoreLostSoundVolume);
    }

    private void UpdateShakeEffect()
    {
        if (!isShaking)
            return;

        shakeTimer += Time.deltaTime;
        float t = shakeTimer / scoreLostShakeDuration;

        if (t >= 1f)
        {
            isShaking = false;
            scoreUpdateRect.anchoredPosition = popupBasePosition;
            scoreUpdateText.color = baseTextColor;
            HidePopupImmediately();
            return;
        }

        float damper = 1f - t;
        float offsetX = Mathf.Sin(t * 40f) * scoreLostShakeStrength * damper;
        scoreUpdateRect.anchoredPosition = popupBasePosition + new Vector3(offsetX, 0f, 0f);
    }

    private void HidePopupImmediately()
    {
        scoreUpdateText.gameObject.SetActive(false);
        multiplierText.gameObject.SetActive(false);
        scoreUpdateRect.anchoredPosition = popupBasePosition;
        scoreUpdateText.color = baseTextColor;
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