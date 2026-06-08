using System;
using Mirror;
using static RCCP;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : NetworkBehaviour
{
    private TMP_Text scoreText;
    private TMP_Text scoreUpdateText;
 
    [SyncVar]
    private float score = 0;

    [SyncVar]
    private float scoreUpdate = 0;

    private RCCP_CarController carController;

    private float metters = 0;
    private float distDrift = 0;

    private float multiplierModifier = 1;
    private float multiplier = 1;
    private float challengeMultiplier = 1;

    private bool[] scoreAchievements = { false, false, false };
    private bool[] distAchievements = { false, false, false };
    private bool[] parkingChallenge = { false, false };

    private bool isChallengeMultAply = false;
    private bool isEnd = false;

    public override void OnStartServer()
    {
        base.OnStartServer();

        carController = GetComponent<RCCP_CarController>();

        if (carController == null)
            carController = GetComponentInParent<RCCP_CarController>();
    }

    private void Start()
    {
        scoreText = UI_Manager.Instance.score;
        scoreText.gameObject.SetActive(true);
        scoreUpdateText = UI_Manager.Instance.Upd_score;
        scoreUpdateText.gameObject.SetActive(true);
    }

    [ServerCallback]
    private void Update()
    {
        if (carController == null )
            return;

        if (!isEnd)
        {
            if (Mathf.Abs(carController.PoweredAxles[0].leftWheelCollider.SidewaysSlip) >= 0.25f)
            {
                metters += (Mathf.Abs(carController.speed) / 3.6f) * Time.deltaTime;

                // Score achievements
                if (score >= 10000 && !scoreAchievements[0])
                {
                    multiplier += 2;
                    scoreAchievements[0] = true;
                }

                if (score >= 5000 && !scoreAchievements[1])
                {
                    multiplier += 1.5f;
                    scoreAchievements[1] = true;
                }

                if (score >= 1000 && !scoreAchievements[2])
                {
                    multiplier += 1;
                    scoreAchievements[2] = true;
                }

                // Distance achievements
                if (metters >= 200 && !distAchievements[0])
                {
                    multiplier += 1.8f;
                    distAchievements[0] = true;
                }

                if (metters >= 100 && !distAchievements[1])
                {
                    multiplier += 1.2f;
                    distAchievements[1] = true;
                }

                if (metters >= 50 && !distAchievements[2])
                {
                    multiplier += 1;
                    distAchievements[2] = true;
                }

                // Challenge multiplier
                if (metters >= 30 && !parkingChallenge[0])
                {
                    challengeMultiplier *= 1.5f;
                    parkingChallenge[0] = true;
                }

                if (score >= 5000 && !parkingChallenge[1])
                {
                    challengeMultiplier *= 2;
                    parkingChallenge[1] = true;
                }

                distDrift += metters;

                if (multiplierModifier < distDrift)
                {
                    if (multiplierModifier + 100 <= distDrift)
                    {
                        multiplierModifier += 100;
                        multiplier += multiplierModifier / 500f;
                    }
                }

                scoreUpdate = (int)distDrift;
                metters = 0;
            }
            else
            {
                score += (int)distDrift;
                scoreUpdate = 0;

                distDrift = 0;
                multiplierModifier = 0;
            }
        }
        else if (!isChallengeMultAply)
        {
            if (distDrift != 0)
                score += (int)distDrift;

            score *= multiplier;
            score *= challengeMultiplier;

            isChallengeMultAply = true;
        }

        RpcUpdateUI(score, scoreUpdate);
    }

    [ClientRpc]
    private void RpcUpdateUI(float currentScore, float currentScoreUpdate)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore:F0}";

        if (scoreUpdateText != null)
            scoreUpdateText.text = currentScoreUpdate > 0
                ? $"+{currentScoreUpdate:F0}"
                : "";
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
            isEnd = true;
    }

    [ServerCallback]
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Finish"))
            isEnd = true;
    }
}