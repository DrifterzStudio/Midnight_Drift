using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using System;

public class Score : RCCP_GenericComponent {
    private RCCP_CarController carController;

    int metters = 0;
    int score = 0;
    int multiplier = 1;
    float dist;


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
        if (carController.handbrakeInput_V == 1f && carController.absoluteSpeed != 0 && carController.steerInput_V != 0) { 
            metters += (int)carController.absoluteSpeed / 36;
            if (metters >= 100) {
                multiplier = metters % 100 + 1;
            }
            score = metters * multiplier;
        } 


        scoreText.text = "Score: " + score;
    }

}
