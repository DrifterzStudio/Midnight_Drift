using TMPro;
using UnityEngine;

// holds the drift-score UI texts in the scene. the Score component rides on the spawned car prefab,
// so it can't keep a direct ref to these scene objects - it finds this at runtime instead.
public class ScoreHUD : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text scoreUpdateText;
    public TMP_Text multiplierText;
}
