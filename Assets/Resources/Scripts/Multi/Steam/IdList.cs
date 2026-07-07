using System.Collections.Generic;
using UnityEngine;

public class IdList : MonoBehaviour
{
    private List<string> PlayerIdSteam = new List<string>();
    ZoneChangementScene instance;

    public void GetPlayerIds(List<string> SteamId)
    {
        Debug.Log("Tentative de récupération des ID de joueurs...");
        if (SteamId == null || SteamId.Count == 0)
        {
            Debug.LogWarning("Aucun ID de joueur reçu.");
            return;
        }
        PlayerIdSteam = SteamId;
        if (PlayerIdSteam.Count > 0 )
            Debug.Log("Nombre d'ID de joueurs reçus : " + PlayerIdSteam.Count);
        for (int i = 0; i < PlayerIdSteam.Count; i++)
        {
            string id = PlayerIdSteam[i].ToString();
            Debug.Log("ID du joueur : " + id);
        }
    }
}
