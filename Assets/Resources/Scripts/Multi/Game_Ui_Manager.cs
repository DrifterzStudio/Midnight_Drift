using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Game_UI_Manager : Singleton_Obj<Game_UI_Manager>
{
    [SerializeField]
    public TMP_Text score;
    [SerializeField]
    public TMP_Text updScore;
}