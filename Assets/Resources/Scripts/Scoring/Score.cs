using TMPro;
using UnityEngine;

// solo drift score hud. reads the car's slip/speed, runs a DriftScoreCalculator, and shows the
// score / popup / multiplier / sounds from it.
public class Score : RCCP_GenericComponent
{
    private RCCP_CarController carController;
    private DriftScoreCalculator calc;

    [Tooltip("Minimum sideways slip required to count as drifting.")]
    public float driftSlipThreshold = 0.25f;

    [Tooltip("Time without drifting before the current run banks into total score.")]
    public float driftTimeoutDelay = 2f;

    [Tooltip("Drift points earned per second (scaled by speed). Higher = score climbs faster.")]
    public float pointsRate = 4f;

    [Tooltip("Seconds of continuous drift to raise the multiplier by one (x2, x3...).")]
    public float multiplierStepTime = 0.8f;

    [Tooltip("Maximum drift multiplier.")]
    public int maxMultiplier = 10;

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

    [Tooltip("Layer used by circuit obstacles (props, barriers, tires, etc.).")]
    public LayerMask obstacleLayer;

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
        // a spawned car can't hold a scene UI ref, so pull the HUD texts from the scene if unset
        if (scoreText == null)
        {
            ScoreHUD hud = FindAnyObjectByType<ScoreHUD>();
            if (hud != null)
            {
                scoreText = hud.scoreText;
                scoreUpdateText = hud.scoreUpdateText;
                multiplierText = hud.multiplierText;
            }
        }

        calc = new DriftScoreCalculator
        {
            DriftSlipThreshold = driftSlipThreshold,
            DriftTimeoutDelay = driftTimeoutDelay,
            PointsRate = pointsRate,
            MultiplierStepTime = multiplierStepTime,
            MaxMultiplier = maxMultiplier,
        };
        calc.Banked += OnBanked;
        calc.Lost += OnLost;

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

        // no scoring during the crash shake
        if (!calc.Ended && !isShaking)
            calc.Tick(CurrentSlip(), carController.speed, Time.deltaTime);

        UpdateUI();
        UpdatePopupAnimation();
        UpdateShakeEffect();
    }

    public float ScoreTotal => calc != null ? calc.Score : 0f;

    // RaceManager calls this on the last lap. banks the pending drift and returns the total
    public float FinalizeAndGetScore()
    {
        return calc != null ? calc.Finalize() : 0f;
    }

    private float CurrentSlip()
    {
        if (carController.PoweredAxles == null || carController.PoweredAxles.Count == 0)
            return 0f;

        return carController.PoweredAxles[0].leftWheelCollider.SidewaysSlip;
    }

    private void OnBanked()
    {
        PlayScoreBankSound();
        StartPopupFadeOut();
    }

    private void OnLost()
    {
        StartScoreLostEffect();
    }

    private void UpdateUI()
    {
        scoreText.text = calc.Score.ToString("N0");

        if (isShaking || isFadingOut)
            return; // the shake / fade animations own the popup right now

        if (calc.PendingPoints > 0)
        {
            scoreUpdateText.gameObject.SetActive(true);
            scoreUpdateCanvasGroup.alpha = 1f;
            scoreUpdateRect.localScale = baseScale;
            scoreUpdateText.text = "+" + ((int)calc.PendingPoints).ToString("N0");

            if (calc.Multiplier > 1)
            {
                multiplierText.gameObject.SetActive(true);
                multiplierText.text = "x" + calc.Multiplier;
            }
            else
            {
                multiplierText.gameObject.SetActive(false);
            }
        }
        else
        {
            HidePopupImmediately();
        }
    }

    private void PlayScoreBankSound()
    {
        if (scoreBankSound && audioSource)
            audioSource.PlayOneShot(scoreBankSound, scoreBankSoundVolume);
    }

    private void StartPopupFadeOut()
    {
        fadeTimer = 0f;
        isFadingOut = true;
        scoreUpdateCanvasGroup.alpha = 1f;
        multiplierText.gameObject.SetActive(false); // multiplier is consumed on bank, don't let it linger
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
        if (calc != null && collider.CompareTag("Finish"))
            calc.Finalize();
    }

    private void OnTriggerStay(Collider collider)
    {
        if (calc != null && collider.CompareTag("Finish"))
            calc.Finalize();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isShaking || calc == null)
            return;

        if (IsInLayerMask(collision.gameObject.layer, obstacleLayer))
        {
            isFadingOut = false;
            calc.RegisterCollision();
        }
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    private void StartScoreLostEffect()
    {
        scoreUpdateText.gameObject.SetActive(true);
        scoreUpdateCanvasGroup.alpha = 1f;
        scoreUpdateText.color = scoreLostColor;
        multiplierText.gameObject.SetActive(false); // the drift is lost, drop the multiplier too

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
}
