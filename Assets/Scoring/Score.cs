using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using System;

public class Score : RCCP_GenericComponent  {
    private RCCP_CarController carController;

    int metters = 0;
    int score = 0;
    int multiplier = 1;
    int multiplierModifier = 0;
    int timer = 0;
    int totalMetters = 0;
    bool[] scoreAchievements = { false, false, false }; // 10000 / 5000 / 1000
    bool[] distAchievements = { false, false, false }; // 200 / 100 / 50


    [Tooltip("Text showing the current score.")]
    [Space()]
    public Text scoreText;

    
    private void Update() {
        //  Getting active player car controller on the scene.
        carController = RCCPSceneManager.activePlayerVehicle;

        //  If no active player car found, return.
        if (!carController)
            return;

        /// <summary>
        /// Calculate the score when drifting.
        /// </summary>
        if (carController.handbrakeInput_V == 1f && carController.speed >= 0 && carController.steerInput_V != 0) {
            timer++;
            metters += (int)Math.Abs(carController.speed) * 10 / 36;
            metters /= timer;

            /// <summary>
            /// Calculate the score achievements multiplier.
            /// </summary>
            if (score >= 10000 && !scoreAchievements[0]) {
                multiplier += 5;
                scoreAchievements[0] = true;
            }
            if (score >= 5000 && !scoreAchievements[1]) {
                multiplier += 3;
                scoreAchievements[1] = true;
            }
            if (score >= 1000 && !scoreAchievements[2]) {
                multiplier += 2;
                scoreAchievements[2] = true;
            }

            /// <summary>
            /// Calculate the distance achievements multiplier.
            /// </summary>
            if (totalMetters >= 200 && !distAchievements[0]) {
                multiplier += 3;
                distAchievements[0] = true;
            }
            if (totalMetters >= 100 && !distAchievements[1]) {
                multiplier += 2;
                distAchievements[1] = true;
            }
            if (totalMetters >= 50 && !distAchievements[2]) {
                 multiplier += 2;
                distAchievements[2] = true;
            }

            /// <summary>
            /// Calculate the score multiplier.
            /// </summary>
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
        }

        totalMetters += metters;
        score += metters * multiplier;
        scoreText.text = "Score: " + score;
    }

}
