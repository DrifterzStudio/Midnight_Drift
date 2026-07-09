using System;
using Mirror;
using Telepathy;
using TMPro;
using UnityEngine;


public class Score : NetworkBehaviour
{
    static private Score _localInstance = null;

    //[SyncVar(hook = nameof(OnScoreChanged))]
    //private float _syncScore = 0f;
    [SyncVar(hook = nameof(OnScoreUpdateChanged))]
    private float _syncScoreUpdate = 0f;
    [SyncVar]
    private float _syncScoreMultiplier = 1f;

    private float _meters = 0f;
    private float _distDrift = 0f;
    private float _timer = 0f;
    private float _score = 0f;
    private float _multiplier = 1f;
    private float _challengeMultiplier = 1f;
    private float _scoreMultiplier = 1f;
    private float _distMultiplierModifier = 0f;
    private float _scoreMultiplierModifier = 0f;
    private bool _isEnd = false;
    private bool _isChallengeMultiApplied = false;

    private bool[] _scoreAchievements = { false, false, false }; // 10000 / 5000 / 1000
    private bool[] _distAchievements = { false, false, false }; // 200 / 100 / 50
    private bool[] _parkingChallenge = { false, false }; // dist 30m / 5000 pts

    private TMP_Text _scoreText;
    private TMP_Text _scoreUpdateText;

    private RCCP_CarController _carController;


    [Tooltip("Minimum sideways slip required to count as drifting.")]
    public float driftSlipThreshold = 0.25f;

    [Tooltip("Time without drifting before the current run banks into total score.")]
    public float driftTimeoutDelay = 2f;

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

    private void Start()
    {
        if (isLocalPlayer)
        {
            _localInstance = this;
            _scoreText = Game_UI_Manager.Instance.score;
            _scoreUpdateText = Game_UI_Manager.Instance.updScore;
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
        if (!NetworkClient.ready)
        {
            return;
        }

        NetworkCamera camera = GetComponent<NetworkCamera>();
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
        CanvasGroup canvasGroup = _localInstance._scoreUpdateCanvasGroup;
        RectTransform rect = _localInstance._scoreUpdateRect;
        Vector3 baseScale = _localInstance._baseScale;

        scoreText.text = "Score: " + scoreValue.ToString("N0");

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

        if (!isCurrentlyDrifting && !_localInstance._isFadingOut)
            return;

    
        if (_syncScoreMultiplier > 1)
        {
            scoreUpdateText.text = $"+{((int)_syncScoreUpdate).ToString("N0")} x{_syncScoreMultiplier:0.0}";
        }
        else
        {
            scoreUpdateText.text = $"+{((int)_syncScoreUpdate).ToString("N0")}";
        }
    }

    [Command]
    private void CmdUpdateDrift(float sidewaysSlip, float speed, float dt)
    {
        if (speed < 0)
            speed = 0;

        if (_isEnd)
        {
            if (!_isChallengeMultiApplied)
            {
                _score *= _multiplier;
                _score *= _challengeMultiplier;
                _isChallengeMultiApplied = true;
                Score_Manager.Instance.CmdAddScoreForCon(GetComponent<PlayerInfos>().SteamId, _score);
                //_syncScore += _score;
                _score = 0;
            }

            return;
        }
        if (sidewaysSlip >= driftSlipThreshold)
        {
            _meters += (speed / 3.6f) * dt;
            _timer = 0f;
            _distDrift += _meters;

            _syncScoreUpdate = (int)_distDrift * _scoreMultiplier;
            _meters = 0f;
        }
        else
        {
            _timer += dt;
            if (_timer >= driftTimeoutDelay)
            {
                bool hasPointsToBank = (int)_distDrift > 0;

                _score += (int)_distDrift * _scoreMultiplier;
                Score_Manager.Instance.CmdAddScoreForCon(GetComponent<PlayerInfos>().SteamId, _score);
                _score = 0;
                _syncScoreUpdate = 0f;

                _distDrift = 0f;
                _distMultiplierModifier = 0f;
                _scoreMultiplierModifier = 0f;
                _scoreMultiplier = 1f;
                _syncScoreMultiplier = 1f;
                _timer = 0f;

         
                if (hasPointsToBank)
                    RpcOnScoreBanked();
            }
        }
        // appel server uniquement
        UpdateMultiplier();
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Finish"))
            _isEnd = true;
    }

    [ServerCallback]
    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Finish"))
            _isEnd = true;
    }

  
    [ServerCallback]
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
        bool hadPointsToLose = (int)_distDrift > 0;

        _distDrift = 0f;
        _meters = 0f;
        _distMultiplierModifier = 0f;
        _scoreMultiplierModifier = 0f;
        _scoreMultiplier = 1f;
        _syncScoreMultiplier = 1f;
        _timer = 0f;
        _syncScoreUpdate = 0f;


        if (hadPointsToLose)
            RpcOnScoreLost();
    }


    [ClientRpc]
    private void RpcOnScoreLost()
    {
        if (!isLocalPlayer)
            return;

        StartScoreLostEffect();
    }

    [Server]
    private void UpdateMultiplier()
    {
        // Score achievements
        if (_score >= 10000 && !_scoreAchievements[0])
        {
            _multiplier += 3f;
            _scoreAchievements[0] = true;
        }

        if (_score >= 5000 && !_scoreAchievements[1])
        {
            _multiplier += 2.5f;
            _scoreAchievements[1] = true;
        }

        if (_score >= 1000 && !_scoreAchievements[2])
        {
            _multiplier += 2f;
            _scoreAchievements[2] = true;
        }


        if (_distDrift >= 200 && !_distAchievements[0])
        {
            _multiplier += 1.8f;
            _distAchievements[0] = true;
        }

        if (_distDrift >= 100 && !_distAchievements[1])
        {
            _multiplier += 1.2f;
            _distAchievements[1] = true;
        }

        if (_distDrift >= 50 && !_distAchievements[2])
        {
            _multiplier += 1f;
            _distAchievements[2] = true;
        }


        if (_distDrift >= 30 && !_parkingChallenge[0])
        {
            _challengeMultiplier *= 1.5f;
            _parkingChallenge[0] = true;
        }

        if (_score >= 5000 && !_parkingChallenge[1])
        {
            _challengeMultiplier *= 2f;
            _parkingChallenge[1] = true;
        }


        if (_distMultiplierModifier + 100 <= _distDrift)
        {
            _distMultiplierModifier += 100;
            _multiplier += _distMultiplierModifier / 200f;
        }


        if (_scoreMultiplierModifier + 150 <= _distDrift)
        {
            _scoreMultiplierModifier += 150;
            _scoreMultiplier = 1f + _scoreMultiplierModifier / 300f;
            _syncScoreMultiplier = _scoreMultiplier;
        }
    }

    private void OnScoreChanged(float _, float newScore)
    {
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
        _scoreUpdateRect.anchoredPosition = _popupBasePosition;
        _scoreUpdateText.color = _baseTextColor;
    }
}