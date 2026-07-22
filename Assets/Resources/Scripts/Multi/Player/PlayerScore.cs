using System;
using Mirror;
using TMPro;
using UnityEngine;


public class PlayerScore : NetworkBehaviour
{
    static private PlayerScore _localInstance = null;

    [SyncVar(hook = nameof(OnScoreUpdateChanged))]
    private float _syncScoreUpdate = 0f;
    [SyncVar]
    private int _syncScoreMultiplier = 1;
    [SyncVar]
    private int _syncCurrentLap = 1;

    public int CurrentLap => _syncCurrentLap;

    private DriftScoreCalculator _calc;
    private LapTracker _lapTracker;
    private float _pushedScore = 0f;

    private TMP_Text _scoreText;
    private TMP_Text _scoreUpdateText;
    private TMP_Text _multiplierText;
    private TMP_Text _lapText;

    private RCCP_CarController _carController;


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

    [Header("Laps")]
    [Tooltip("Number of laps before this player's race ends. Keep it the same as solo.")]
    public int maxLaps = 5;

    [Tooltip("How far from the line the car must get before a crossing counts.")]
    public float lineLeaveDistance = 25f;

    [Tooltip("How far sideways from the line still counts as crossing it.")]
    public float lineHalfWidth = 25f;

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

    private CanvasGroup _scoreUpdateCanvasGroup;
    private RectTransform _scoreUpdateRect;
    private float _fadeTimer = 0f;
    private bool _isFadingOut = false;
    private Vector3 _baseScale;

    private Color _baseTextColor;
    private float _shakeTimer = 0f;
    private bool _isShaking = false;
    private Vector3 _popupBasePosition;

    public override void OnStartServer()
    {
        _calc = new DriftScoreCalculator
        {
            DriftSlipThreshold = driftSlipThreshold,
            DriftTimeoutDelay = driftTimeoutDelay,
            PointsRate = pointsRate,
            MultiplierStepTime = multiplierStepTime,
            MaxMultiplier = maxMultiplier,
        };
        _calc.Banked += OnCalcBanked;
        _calc.Lost += OnCalcLost;

        _lapTracker = new LapTracker
        {
            MaxLaps = maxLaps,
            LeaveDistance = lineLeaveDistance,
            LineHalfWidth = lineHalfWidth,
        };
        _lapTracker.LapCompleted += OnLapCompleted;
        _lapTracker.RaceFinished += OnRaceFinished;

        FinishLine line = FindAnyObjectByType<FinishLine>();
        if (line != null)
        {
            _lapTracker.SetLine(line.transform.position, line.transform.forward);
        }
        else
        {
            Debug.LogWarning("PlayerScore: no FinishLine in the scene, using the car's spawn as the line.");
            _lapTracker.SetLine(transform.position, transform.forward);
        }
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            _localInstance = this;
            _scoreText = Game_UI_Manager.Instance.score;
            _scoreUpdateText = Game_UI_Manager.Instance.updScore;
            _multiplierText = Game_UI_Manager.Instance.multiplier;
            _lapText = Game_UI_Manager.Instance.lap;
            _scoreText.gameObject.SetActive(true);
            _scoreUpdateText.gameObject.SetActive(true);
            _scoreText.text = "Score: " + 0;
            _scoreUpdateText.text = " " + 0;
            _carController = GetComponent<RCCP_CarController>();

            audioSource = GetComponent<AudioSource>();
            if (!audioSource)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;

            _scoreUpdateCanvasGroup = _scoreUpdateText.GetComponent<CanvasGroup>();
            if (!_scoreUpdateCanvasGroup)
                _scoreUpdateCanvasGroup = _scoreUpdateText.gameObject.AddComponent<CanvasGroup>();

            _scoreUpdateRect = _scoreUpdateText.GetComponent<RectTransform>();
            _baseScale = _scoreUpdateRect.localScale;
            _popupBasePosition = _scoreUpdateRect.anchoredPosition;
            _baseTextColor = _scoreUpdateText.color;
        }
    }

    private void Update()
    {
        if (isServer && _lapTracker != null && !_lapTracker.Finished)
            _lapTracker.Tick(transform.position, Time.deltaTime);

        if (!NetworkClient.ready)
        {
            return;
        }

        if (NetworkCamera.LocalInstance != null && NetworkCamera.LocalInstance.IsPlayerActive && isLocalPlayer)
        {
            if (!_carController) return;
            if (_carController.PoweredAxles.Count <= 0) return;

            float sidewaysSlip = (float)Math.Abs(_carController.PoweredAxles[0].leftWheelCollider.SidewaysSlip);
            float speed = _carController.speed;
            float deltaTime = Time.deltaTime;
            CmdUpdateDrift(sidewaysSlip, speed, deltaTime);
            UpdateScoreUI(Score_Manager.Instance.ScoreData[GetComponent<PlayerInfos>().SteamId]);
        }
        else if (NetworkCamera.LocalInstance != null && NetworkCamera.LocalInstance.ActiveCar != null && NetworkCamera.LocalInstance.ActiveCar.gameObject == gameObject)
        {
            UpdateScoreUI(Score_Manager.Instance.ScoreData[GetComponent<PlayerInfos>().SteamId]);
        }

        if (isLocalPlayer)
        {
            UpdatePopupAnimation();
            UpdateShakeEffect();
        }
    }

    private void UpdateScoreUI(float scoreValue)
    {
        if (_localInstance == null)
            return;

        TMP_Text scoreText = _localInstance._scoreText;
        TMP_Text scoreUpdateText = _localInstance._scoreUpdateText;
        TMP_Text multiplierText = _localInstance._multiplierText;
        CanvasGroup canvasGroup = _localInstance._scoreUpdateCanvasGroup;
        RectTransform rect = _localInstance._scoreUpdateRect;
        Vector3 baseScale = _localInstance._baseScale;

        scoreText.text = "Score: " + scoreValue.ToString("N0");

        if (_localInstance._lapText != null)
            _localInstance._lapText.text = "LAP " + Mathf.Min(_syncCurrentLap, maxLaps) + "/" + maxLaps;

        if (_localInstance._isShaking)
            return;

        bool isCurrentlyDrifting = _syncScoreUpdate > 0;

        if (isCurrentlyDrifting && !_localInstance._isFadingOut)
        {
            scoreUpdateText.gameObject.SetActive(true);
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
            if (rect != null)
                rect.localScale = baseScale;
        }

        if (!isCurrentlyDrifting)
        {
            if (!_localInstance._isFadingOut)
            {
                scoreUpdateText.gameObject.SetActive(false);
                if (multiplierText != null)
                    multiplierText.gameObject.SetActive(false);
            }

            return;
        }

        scoreUpdateText.text = $"+{((int)_syncScoreUpdate).ToString("N0")}";

        if (multiplierText != null)
        {
            if (_syncScoreMultiplier > 1)
            {
                multiplierText.gameObject.SetActive(true);
                multiplierText.text = "x" + _syncScoreMultiplier;
            }
            else
            {
                multiplierText.gameObject.SetActive(false);
            }
        }
    }

    [Command]
    private void CmdUpdateDrift(float sidewaysSlip, float speed, float dt)
    {
        if (_calc == null || _calc.Ended)
            return;

        if (speed < 0)
            speed = 0;

        _calc.Tick(sidewaysSlip, speed, dt);

        _syncScoreUpdate = _calc.PendingPoints;
        _syncScoreMultiplier = _calc.Multiplier;
    }

    [Server]
    private void PushScoreDelta()
    {
        float delta = _calc.Score - _pushedScore;
        if (delta > 0f)
        {
            Score_Manager.Instance.CmdAddScoreForCon(GetComponent<PlayerInfos>().SteamId, delta);
            _pushedScore = _calc.Score;
        }
    }

    [Server]
    private void OnCalcBanked()
    {
        PushScoreDelta();
        _syncScoreUpdate = 0f;
        _syncScoreMultiplier = 1;
        RpcOnScoreBanked();
    }

    [Server]
    private void OnCalcLost()
    {
        _syncScoreUpdate = 0f;
        _syncScoreMultiplier = 1;
        RpcOnScoreLost();
    }

    [Server]
    private void OnLapCompleted()
    {
        _syncCurrentLap = _lapTracker.CurrentLap;
    }

    [Server]
    private void OnRaceFinished()
    {
        _syncCurrentLap = _lapTracker.CurrentLap;
        EndScoring();
    }

    [Server]
    private void EndScoring()
    {
        if (_calc == null || _calc.Ended)
            return;

        _calc.Finalize();
        PushScoreDelta();
        _syncScoreUpdate = 0f;
        _syncScoreMultiplier = 1;
    }


    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (_calc != null && IsInLayerMask(collision.gameObject.layer, obstacleLayer))
            _calc.RegisterCollision();
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }


    [ClientRpc]
    private void RpcOnScoreLost()
    {
        if (!isLocalPlayer)
            return;

        StartScoreLostEffect();
    }

    private void OnScoreUpdateChanged(float _, float newUpdate)
    {
    }

    [ClientRpc]
    private void RpcOnScoreBanked()
    {
        if (!isLocalPlayer)
            return;

        PlayScoreBankSound();
        StartPopupFadeOut();
    }


    private void PlayScoreBankSound()
    {
        if (scoreBankSound && audioSource)
            audioSource.PlayOneShot(scoreBankSound, scoreBankSoundVolume);
    }

    private void StartPopupFadeOut()
    {
        _fadeTimer = 0f;
        _isFadingOut = true;

        if (_scoreUpdateCanvasGroup != null)
            _scoreUpdateCanvasGroup.alpha = 1f;

        if (_multiplierText != null)
            _multiplierText.gameObject.SetActive(false); 
    }

    private void UpdatePopupAnimation()
    {
        if (!_isFadingOut)
            return;

        _fadeTimer += Time.deltaTime;
        float t = _fadeTimer / popupFadeOutDuration;

        _scoreUpdateCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
        _scoreUpdateRect.localScale = Vector3.Lerp(_baseScale * popupScalePunch, _baseScale, t);

        if (t >= 1f)
        {
            _isFadingOut = false;
            _scoreUpdateText.gameObject.SetActive(false);
            _scoreUpdateRect.localScale = _baseScale;
        }
    }

    private void StartScoreLostEffect()
    {
        _scoreUpdateText.gameObject.SetActive(true);

        if (_scoreUpdateCanvasGroup != null)
            _scoreUpdateCanvasGroup.alpha = 1f;

        _scoreUpdateText.color = scoreLostColor;

        if (_multiplierText != null)
            _multiplierText.gameObject.SetActive(false);

        _shakeTimer = 0f;
        _isShaking = true;

        PlayScoreLostSound();
    }

    private void PlayScoreLostSound()
    {
        if (scoreLostSound && audioSource)
            audioSource.PlayOneShot(scoreLostSound, scoreLostSoundVolume);
    }

    private void UpdateShakeEffect()
    {
        if (!_isShaking)
            return;

        _shakeTimer += Time.deltaTime;
        float t = _shakeTimer / scoreLostShakeDuration;

        if (t >= 1f)
        {
            _isShaking = false;
            _scoreUpdateRect.anchoredPosition = _popupBasePosition;
            _scoreUpdateText.color = _baseTextColor;
            HidePopupImmediately();
            return;
        }

        float damper = 1f - t;
        float offsetX = Mathf.Sin(t * 40f) * scoreLostShakeStrength * damper;
        _scoreUpdateRect.anchoredPosition = _popupBasePosition + new Vector3(offsetX, 0f, 0f);
    }

    private void HidePopupImmediately()
    {
        _scoreUpdateText.gameObject.SetActive(false);

        if (_multiplierText != null)
            _multiplierText.gameObject.SetActive(false);

        _scoreUpdateRect.anchoredPosition = _popupBasePosition;
        _scoreUpdateText.color = _baseTextColor;
    }
}
