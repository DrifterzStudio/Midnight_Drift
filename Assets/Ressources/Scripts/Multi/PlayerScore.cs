using System;
using Mirror;
using Telepathy;
using TMPro;
using UnityEngine;

public class Score : NetworkBehaviour
{
    private RCCP_CarController carController;

    [SyncVar(hook = nameof(OnScoreChanged))]
    private float syncScore = 0f;
    [SyncVar(hook = nameof(OnScoreUpdateChanged))]
    private float syncScoreUpdate = 0f;
    private float syncScoreMultiplier = 1f;

    // --- État côté serveur uniquement ---
    private float meters = 0f;
    private float distDrift = 0f;
    private float timer = 0f;
    private float score = 0f;
    private float multiplier = 1f;
    private float challengeMultiplier = 1f;
    private float scoreMultiplier = 1f;
    private float distMultiplierModifier = 0f;
    private float scoreMultiplierModifier = 0f;
    private bool isEnd = false;
    private bool isChallengeMultApplied = false;

    private bool[] scoreAchievements = { false, false, false };
    private bool[] distAchievements = { false, false, false };
    private bool[] parkingChallenge = { false, false };

   
    private TMP_Text scoreText;
    private TMP_Text scoreUpdateText;

    private void Start()
    {
       
        scoreText = UI_Manager.Instance.score;
        scoreUpdateText = UI_Manager.Instance.Upd_score;

        if (isLocalPlayer)
        {
            scoreText.gameObject.SetActive(true);
            scoreUpdateText.gameObject.SetActive(true);
            scoreText.text = "Score: " + 0;
            scoreUpdateText.text = " " + 0;
        }

        carController = GetComponent<RCCP_CarController>();
    }

    private void Update()
    {
       
        if (!isLocalPlayer) return;
        if (!carController) return;

        float sidewaysSlip = (float)Math.Abs(
            carController.PoweredAxles[0].leftWheelCollider.SidewaysSlip);
        float speed = carController.speed ;
        float deltaTime = Time.deltaTime;
        CmdUpdateDrift(sidewaysSlip, speed ,deltaTime);
    }

    
    [Command]
    private void CmdUpdateDrift(float sidewaysSlip, float speed,float dt)
    {
        if(speed < 0) speed = 0;
        if (isEnd)
        {
            if (!isChallengeMultApplied)
            {
                score *= multiplier;
                score *= challengeMultiplier;
                isChallengeMultApplied = true;
                syncScore = score;
            }
            return;
        }

        if (sidewaysSlip >= 0.25f)
        {
            meters += (speed / 3.6f) *dt;
            timer = 0f;
            distDrift += meters;

            syncScoreUpdate = (int)distDrift * scoreMultiplier;
            meters = 0f;
        }
        else
        {
            timer += dt;
            if (timer >= 2f)
            {
                score += (int)distDrift * scoreMultiplier;
                syncScore = score;
                syncScoreUpdate = 0f;

                distDrift = 0f;
                distMultiplierModifier = 0f;
                scoreMultiplierModifier = 0f;
                scoreMultiplier = 1f;
                syncScoreMultiplier = 1f;
                timer = 0f;
            }
        }

        UpdateMultiplier();
    }

    
    [ServerCallback]
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Finish"))
            isEnd = true;
    }

    [ServerCallback]
    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Finish"))
            isEnd = true;
    }

    
    private void UpdateMultiplier()
    {
       
        if (score >= 10000 && !scoreAchievements[0]) { multiplier += 3f; scoreAchievements[0] = true; }
        if (score >= 5000 && !scoreAchievements[1]) { multiplier += 2.5f; scoreAchievements[1] = true; }
        if (score >= 1000 && !scoreAchievements[2]) { multiplier += 2f; scoreAchievements[2] = true; }

       
        if (distDrift >= 200 && !distAchievements[0]) { multiplier += 1.8f; distAchievements[0] = true; }
        if (distDrift >= 100 && !distAchievements[1]) { multiplier += 1.2f; distAchievements[1] = true; }
        if (distDrift >= 50 && !distAchievements[2]) { multiplier += 1f; distAchievements[2] = true; }

      
        if (distDrift >= 30 && !parkingChallenge[0]) { challengeMultiplier *= 1.5f; parkingChallenge[0] = true; }
        if (score >= 5000 && !parkingChallenge[1]) { challengeMultiplier *= 2f; parkingChallenge[1] = true; }

        
        if (distMultiplierModifier + 100 <= distDrift)
        {
            distMultiplierModifier += 100;
            multiplier += distMultiplierModifier / 200f;
        }

        
        if (scoreMultiplierModifier + 150 <= distDrift)
        {
            scoreMultiplierModifier += 150;
            scoreMultiplier = 1f + scoreMultiplierModifier / 300f;
            syncScoreMultiplier = scoreMultiplier;
        }
    }

   

    private void OnScoreChanged(float _, float newScore)
    {
        if (!isLocalPlayer) return;
        scoreText.text = "Score: " + (int)newScore;
    }

    private void OnScoreUpdateChanged(float _, float newUpdate)
    {
        if (!isLocalPlayer) return;
        scoreUpdateText.text = newUpdate > 0
            ? $" {(int)newUpdate} * {syncScoreMultiplier:F1}"
            : $" {(int)newUpdate}";
    }

}