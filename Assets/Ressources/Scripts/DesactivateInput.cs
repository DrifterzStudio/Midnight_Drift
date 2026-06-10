using Mirror;
using Steamworks;
using UnityEngine;

public class CarPlayState : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnCanPlayChanged))]
    public bool canPlay = false;

    private RCCP_CarController _car;
    private RCCP_Engine _engine;

    private void Awake()
    {
        _car = GetComponent<RCCP_CarController>();
        _engine = GetComponentInChildren<RCCP_Engine>();
    }

    // OnStartClient → tous les clients, applique l'état initial reçu du serveur
    public override void OnStartClient()
    {
        base.OnStartClient();
        ApplyState(canPlay);
    }

    // OnStartLocalPlayer → uniquement le propriétaire du prefab
    // Envoie le Steam ID au serveur pour qu'il détermine canPlay
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        ulong myId = SteamUser.GetSteamID().m_SteamID;
        Debug.Log($"[CarPlayState] Je m'enregistre avec le Steam ID : {myId}");
        CmdRegisterSteamID(myId);
    }

    // =============================================
    // COMMANDES
    // =============================================

    [Command]
    private void CmdRegisterSteamID(ulong steamId)
    {
        bool allowed = LobbyManager.CanPlayerPlay(steamId);
        SetCanPlay(allowed);
        Debug.Log($"[Server] SteamID {steamId} → canPlay: {allowed}");

        if (!allowed && LobbyManager.ActivePlayerSteamIds.Count == 0)
            Debug.LogWarning("[Server] ⚠️ ActivePlayerSteamIds est vide ! As-tu sélectionné des joueurs dans le lobby ?");
    }

    [Server]
    public void SetCanPlay(bool value) => canPlay = value;

    // =============================================
    // ÉTAT
    // =============================================

    // Hook Mirror : déclenché sur tous les clients quand canPlay change
    private void OnCanPlayChanged(bool _, bool next) => ApplyState(next);

    private void ApplyState(bool active)
    {
        Debug.Log($"[CarPlayState] ApplyState — active: {active} | isLocalPlayer: {isLocalPlayer}");

        // Contrôle input → seulement le propriétaire de CE prefab
        if (isLocalPlayer && _car != null)
            _car.canControl = active;

        // Moteur → visible/audible pour tout le monde
        if (_engine != null)
        {
            if (active) _engine.StartEngine();
            else _engine.StopEngine();
        }
    }
}