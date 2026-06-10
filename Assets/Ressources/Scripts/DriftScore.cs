using Mirror;
using UnityEngine;


public class DriftScore : NetworkBehaviour
{
    private PlayerName playerNetwork;

    public override void OnStartLocalPlayer()
    {
        playerNetwork = GetComponent<PlayerName>();
    }
    private int currentScore;
    private int lastSentScore;

    void Update()
    {
        if (!isLocalPlayer)
            return;

        // Ton calcul de drift ici
        currentScore += 2;

        if (currentScore - lastSentScore >= 100)
        {
            lastSentScore = currentScore;
            playerNetwork.CmdSetScore(currentScore);
        }
    }
}
