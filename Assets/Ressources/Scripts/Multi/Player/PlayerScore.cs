using System;
using Mirror;
using Telepathy;
using TMPro;
using UnityEngine;


    public class Score : NetworkBehaviour
    {

       
        // variable syncro
        [SyncVar(hook = nameof(OnScoreChanged))]
        private float _syncScore = 0f;
        [SyncVar(hook = nameof(OnScoreUpdateChanged))]
        private float _syncScoreUpdate = 0f;
        private float _syncScoreMultiplier = 1f;

        // private variable 
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

        private bool[] _scoreAchievements = { false, false, false };
        private bool[] _distAchievements = { false, false, false };
        private bool[] _parkingChallenge = { false, false };

        // ui local 
        private TMP_Text _scoreText;
        private TMP_Text _scoreUpdateText;
        // voiture du player 
        private RCCP_CarController _carController;


        private void Start()
        {
           // set la ui et la voiture si c'est le player local 
            if (isLocalPlayer)
            {
                _scoreText = Game_UI_Manager.Instance.score;
                _scoreUpdateText = Game_UI_Manager.Instance.updScore;
                _scoreText.gameObject.SetActive(true);
                _scoreUpdateText.gameObject.SetActive(true);
                _carController = GetComponent<RCCP_CarController>();
            }
        }

        private void Update()
        {
            //seul le client local envoie ses données 
            if (!isLocalPlayer) return;
            if (!_carController) return;

            float sidewaysSlip = (float)Math.Abs(_carController.PoweredAxles[0].leftWheelCollider.SidewaysSlip);
            float speed = _carController.speed;
            float deltaTime = Time.deltaTime;

            // command envoyer au server
            CmdUpdateDrift(sidewaysSlip, speed, deltaTime);
        }

        [Command]
        private void CmdUpdateDrift(float sidewaysSlip, float speed, float dt)
        {
            // eviter le drift en arriere
            if(speed < 0 )
                speed  = 0;

            // verifier que le drift est en cour 
            if (_isEnd)
            {
                if (!_isChallengeMultiApplied)
                {
                    _score *= _multiplier;
                    _score *= _challengeMultiplier;
                    _isChallengeMultiApplied = true;
                    _syncScore = _score;
                }

                return;
            }
            // update logique 
            if (sidewaysSlip >= 0.25f)
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
                if (_timer >= 2f)
                {
                    _score += (int)_distDrift * _scoreMultiplier;
                    _syncScore = _score;
                    _syncScoreUpdate = 0f;

                    _distDrift = 0f;
                    _distMultiplierModifier = 0f;
                    _scoreMultiplierModifier = 0f;
                    _scoreMultiplier = 1f;
                   _syncScoreMultiplier = 1f;
                    _timer = 0f;
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

       
        // callback pour la syncro 
        private void OnScoreChanged(float _, float newScore)
        {
            if (!isLocalPlayer) return;
            _scoreText.text = "Score: " + (int)newScore;
        }

        private void OnScoreUpdateChanged(float _, float newUpdate)
        {
            if (!isLocalPlayer) return;
            _scoreUpdateText.text = newUpdate > 0
                ? $" {(int)newUpdate} * {_syncScoreMultiplier:F1}"
                : $" {(int)newUpdate}";
        }

    }
