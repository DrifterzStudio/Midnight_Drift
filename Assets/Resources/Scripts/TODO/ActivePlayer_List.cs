using JetBrains.Annotations;
using Mirror;
using Mirror.FizzySteam;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ActivePlayer_List : Singleton_Obj_Net<ActivePlayer_List>
{ 

    private readonly SyncList<ulong> PlayerSteamId = new SyncList<ulong>();
    //private readonly List<ulong> PlayerSteamId = new List<ulong>();

    [Command(requiresAuthority = false)]
    public void CmdAddId(ulong steamId)
    {
        // Sécurité serveur : vérifier que steamId correspond bien à l'appelant
        if (!PlayerSteamId.Contains(steamId))
            PlayerSteamId.Add(steamId);
    }

    [Command(requiresAuthority = false)]
    public void CmdRemove(ulong steamId)
    {
        PlayerSteamId.Remove(steamId);
    }

    // Plus besoin de CmdContain/CmdCount : lecture directe côté client
    public bool Contains(ulong steamId) => PlayerSteamId.Contains(steamId);
    public int Count => PlayerSteamId.Count;
}
    