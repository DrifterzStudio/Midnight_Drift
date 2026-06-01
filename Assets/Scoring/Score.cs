using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class Score : RCCP_GenericComponent  {
    private RCCP_CarController carController;

    private int metters = 0;
    private int score = 0;
    private int multiplier = 1;
    private int multiplierModifier = 0;
    private int timer = 0;
    private bool[] scoreAchievements = { false, false, false }; // 10000 / 5000 / 1000
    private bool[] distAchievements = { false, false, false }; // 200 / 100 / 50
    public bool isEnd = false;
   
    [Tooltip("Text showing the current score.")]
    [Space()]
    public Text scoreText;

    private void Update() {
        
        // Getting active player car controller on the scene.
        carController = RCCPSceneManager.activePlayerVehicle;
         
        // If no active player car found, or the circuit finish, return.
        if (!carController || isEnd)
            return;

        // Calculate the score when drifting.
        if (carController.handbrakeInput_V == 1f && carController.speed >= 0 && carController.steerInput_V != 0) {
            timer++;
            metters += (int)Math.Abs(carController.speed) * 10 / 36; // km/h to m/s
            metters /= timer;

            // Calculate the score achievements multiplier.
            if (score >= 10000 && !scoreAchievements[0]) {
                multiplier += 7;
                scoreAchievements[0] = true;
            }
            if (score >= 5000 && !scoreAchievements[1]) {
                multiplier += 5;
                scoreAchievements[1] = true;
            }
            if (score >= 1000 && !scoreAchievements[2]) {
                multiplier += 3;
                scoreAchievements[2] = true;
            }

            // Calculate the distance achievements multiplier.
            if (metters >= 200 && !distAchievements[0]) {
                multiplier += 5;
                distAchievements[0] = true;
            }
            if (metters >= 100 && !distAchievements[1]) {
                multiplier += 3;
                distAchievements[1] = true;
            }
            if (metters >= 50 && !distAchievements[2]) {
                multiplier += 2;
                distAchievements[2] = true;
            }

            // Calculate the score multiplier.
            if (multiplierModifier < metters) {
                if (multiplierModifier + 100 <= metters) {
                    multiplierModifier += 100;
                    multiplier += multiplierModifier / 100;
                }
            }
        }
        else {
            timer = 0;
            metters = 0;
            multiplierModifier = 0;
        }

        score += metters * multiplier;
        scoreText.text = "Score: " + score;
    }
    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Finish")) {
            isEnd = true;
        }
    }
}
