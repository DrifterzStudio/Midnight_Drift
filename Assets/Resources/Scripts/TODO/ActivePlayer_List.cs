using JetBrains.Annotations;
using Mirror;
using Mirror.FizzySteam;
using System.Collections.Generic;
using System.Reflection;
using Steamworks;
using UnityEngine;
using UnityEngine.Serialization;

public class ActivePlayer_List : Singleton_Obj_Net<ActivePlayer_List>
{ 

    public readonly SyncList<ulong> PlayerSteamId = new SyncList<ulong>();

    private List<NetworkConnectionToClient> _readyClients = new();
    //private readonly List<ulong> PlayerSteamId = new List<ulong>();
    public bool Ready = false;
    private int targetCount = 0;
    public override void OnStartServer()
    {
        base.OnStartServer();
        PlayerSteamId.OnChange += OnListChangedServer;
    }

    [Command(requiresAuthority = false)]
    public void CmdAddId(ulong steamId)
    {
        // Sécurité serveur : vérifier que steamId correspond bien à l'appelant
        if (!PlayerSteamId.Contains(steamId))
            PlayerSteamId.Add(steamId);
    }

    [Server]
    private void OnListChangedServer(SyncList<ulong>.Operation op, int index, ulong newItem)
    {
        Ready = false;
        _readyClients.Clear();
        RpcCheckSync(Count);

    }
    [ClientRpc]
    private void RpcCheckSync(int count)
    {
        targetCount = count;
        // Le client vérifie qu'il a bien reçu la SyncList
        if (PlayerSteamId.Count == targetCount)
            CmdConfirmSync();
        else
            // Pas encore reçu → attend le prochain OnChange
            PlayerSteamId.OnChange += WaitForSync;
    }
    [Command(requiresAuthority = false)]
    public void CmdConfirmSync(NetworkConnectionToClient sender = null)
    {
        _readyClients.Add(sender);

        if (_readyClients.Count >= NetworkServer.connections.Count)
        {
            _readyClients.Clear();
            Ready = true;
        }
    }

    private void WaitForSync(SyncList<ulong>.Operation op, int index, ulong newItem)
    {
        if (PlayerSteamId.Count == targetCount)
        {
            Instance.PlayerSteamId.OnChange -= WaitForSync;
            CmdConfirmSync();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdRemove(ulong steamId)
    {
        Instance.PlayerSteamId.Remove(steamId);
    }

    // Plus besoin de CmdContain/CmdCount : lecture directe côté client
    public bool Contains(ulong steamId) => PlayerSteamId.Contains(steamId);
    public int Count => PlayerSteamId.Count;
}
    