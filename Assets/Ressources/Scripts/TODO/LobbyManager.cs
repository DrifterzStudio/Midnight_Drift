using Mirror;
using Steamworks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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
        if (playerRowPrefab == null || SteamLobby.Instance == null) return;

        // Trouve le canvas (ton approche qui marchait)
        GameObject canvasObj = GameObject.Find("RCCP_Canvas");
        if (canvasObj == null) { Debug.LogError("[Lobby] RCCP_Canvas introuvable !"); return; }

        // Trouve ou crée le container de liste à l'intérieur du canvas
        Transform container = canvasObj.transform.Find("PlayerList");
        if (container == null)
        {
            GameObject listObj = new GameObject("PlayerList");
            listObj.transform.SetParent(canvasObj.transform, false);
            RectTransform rt = listObj.AddComponent<RectTransform>();
            // Position en haut à gauche — ajuste ces valeurs selon ton layout
            rt.anchorMin = new Vector2(0f, 0.6f);
            rt.anchorMax = new Vector2(0.35f, 0.95f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            container = listObj.transform;
        }

        // VerticalLayoutGroup — childControlHeight = true pour qu'il positionne vraiment les lignes
        VerticalLayoutGroup vGroup = container.GetComponent<VerticalLayoutGroup>();
        if (vGroup == null) vGroup = container.gameObject.AddComponent<VerticalLayoutGroup>();
        vGroup.spacing = 6;
        vGroup.padding = new RectOffset(6, 6, 6, 6);
        vGroup.childControlWidth = true;
        vGroup.childForceExpandWidth = true;
        vGroup.childControlHeight = true;  // ← VLG gère la hauteur via LayoutElement
        vGroup.childForceExpandHeight = false;

        ContentSizeFitter csf = container.GetComponent<ContentSizeFitter>();
        if (csf == null) csf = container.gameObject.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Nettoie
        foreach (Transform child in container)
            Destroy(child.gameObject);

        // Spawn des lignes
        CSteamID lobbyID = GetLobbyID();
        int count = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
        bool isHost = NetworkServer.active;

        for (int i = 0; i < count; i++)
        {
            CSteamID member = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);
            ulong memberId = member.m_SteamID;
            string name = SteamFriends.GetFriendPersonaName(member);
            bool isActive = ActivePlayerSteamIds.Contains(memberId);

            var row = Instantiate(playerRowPrefab, container);

            // Force une hauteur si le prefab n'a pas de LayoutElement
            LayoutElement le = row.GetComponent<LayoutElement>();
            if (le == null) le = row.gameObject.AddComponent<LayoutElement>();
            if (le.preferredHeight <= 0) le.preferredHeight = 60f;

            row.Setup(name, memberId, isActive, isHost, TogglePlayer);
        }

        // Force Unity à recalculer le layout immédiatement
        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());
    }
}