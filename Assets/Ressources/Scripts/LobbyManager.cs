
using Steamworks;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public bool IsPlaying = true;

 
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private TMP_Text playerEntryPrefab;

    private Callback<LobbyChatUpdate_t> lobbyChatUpdate;

    private void Start()
    {
        lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
    {
        EChatMemberStateChange state = (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;

        if (state == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
            Debug.Log("New Player Joined");
        if (state == EChatMemberStateChange.k_EChatMemberStateChangeLeft)
            Debug.Log("Player Left");

        RefreshPlayerListUI();
    }

    public void RefreshPlayerListUI()
    {
        if (SteamLobby.Instance == null) return;

        CSteamID lobbyID = new CSteamID(SteamLobby.Instance.currentLobbyID);
        int count = SteamMatchmaking.GetNumLobbyMembers(lobbyID);

        foreach (Transform child in playerListContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < count; i++)
        {
            CSteamID member = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);
            string playerName = SteamFriends.GetFriendPersonaName(member);

            TMP_Text entry = Instantiate(playerEntryPrefab, playerListContainer);
            // Indique visuellement qui peut jouer
            entry.text = i < ((NetWorkManager)NetWorkManager.singleton).MaxActivePlayers
                ? $"🎮 {playerName}"
                : $"👁 {playerName} (spectateur)";
        }
    }
}