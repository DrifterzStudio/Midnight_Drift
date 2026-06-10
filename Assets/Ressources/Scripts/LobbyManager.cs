using Mirror;
using Steamworks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas rccp_canvas;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private LobbyPlayerRowUI playerRowPrefab; // ← Ton nouveau Prefab ici !

    [Header("Rules")]
    [Tooltip("Si personne n'est sélectionné manuellement, les N premiers joueurs peuvent jouer")]
    //[SerializeField] private int defaultMaxPlayers = 2;

    public bool IsPlaying = true;

    public static readonly HashSet<ulong> ActivePlayerSteamIds = new HashSet<ulong>();
    private const string LOBBY_KEY = "activePlayers";

    private Callback<LobbyChatUpdate_t> lobbyChatUpdate;
    private Callback<LobbyDataUpdate_t> lobbyDataUpdate;

    private void Start()
    {
        lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
        RefreshPlayerListUI();
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t cb)
    {
        EChatMemberStateChange state = (EChatMemberStateChange)cb.m_rgfChatMemberStateChange;
        if (state == EChatMemberStateChange.k_EChatMemberStateChangeEntered) Debug.Log("Player Joined");
        if (state == EChatMemberStateChange.k_EChatMemberStateChangeLeft) Debug.Log("Player Left");
        RefreshPlayerListUI();
    }

    private void OnLobbyDataUpdate(LobbyDataUpdate_t cb)
    {
        ReadActivePlayersFromLobbyData();
        RefreshPlayerListUI();
    }

    private CSteamID GetLobbyID() => new CSteamID(SteamLobby.Instance.currentLobbyID);

    private void ReadActivePlayersFromLobbyData()
    {
        if (SteamLobby.Instance == null) return;
        string data = SteamMatchmaking.GetLobbyData(GetLobbyID(), LOBBY_KEY);

        ActivePlayerSteamIds.Clear();
        if (string.IsNullOrEmpty(data)) return;

        foreach (string idStr in data.Split(','))
            if (ulong.TryParse(idStr, out ulong id))
                ActivePlayerSteamIds.Add(id);
    }

    private void WriteActivePlayersToLobbyData()
    {
        if (SteamLobby.Instance == null) return;
        SteamMatchmaking.SetLobbyData(GetLobbyID(), LOBBY_KEY, string.Join(",", ActivePlayerSteamIds));
    }

    public void TogglePlayer(ulong steamId)
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Lobby] Seul le host peut changer les rôles.");
            return;
        }

        if (ActivePlayerSteamIds.Contains(steamId))
            ActivePlayerSteamIds.Remove(steamId);
        else
            ActivePlayerSteamIds.Add(steamId);

        WriteActivePlayersToLobbyData();
        RefreshPlayerListUI();
    }

    public static bool CanPlayerPlay(ulong steamId)
    {
        if (ActivePlayerSteamIds.Count == 0) return true;
        return ActivePlayerSteamIds.Contains(steamId);
    }

    public void RefreshPlayerListUI()
    {
        GameObject canvasObj = GameObject.Find("RCCP_Canvas");
        if (canvasObj == null)
        {
            Debug.LogError("RCCP_Canvas introuvable !");
            return;
        }

        Transform canvas = canvasObj.transform;

        // 💡 CLEAN UNIQUEMENT les rows du lobby (PAS tout le canvas)
        foreach (Transform child in canvas)
        {
            if (child.GetComponent<LobbyPlayerRowUI>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        if (playerRowPrefab == null || SteamLobby.Instance == null)
            return;

        CSteamID lobbyID = GetLobbyID();
        int count = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
        bool isHost = NetworkServer.active;

        for (int i = 0; i < count; i++)
        {
            CSteamID member = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);

            ulong memberId = member.m_SteamID;
            string name = SteamFriends.GetFriendPersonaName(member);
            bool isActive = ActivePlayerSteamIds.Contains(memberId);

            // Spawn
            var row = Instantiate(playerRowPrefab);
            row.transform.SetParent(canvas, false);

            row.Setup(name, memberId, isActive, isHost, TogglePlayer);
        }
    }
}