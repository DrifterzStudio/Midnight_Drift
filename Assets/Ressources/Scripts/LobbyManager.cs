//using Mirror;
//using Steamworks;
//using UnityEngine;

//public class LobbyManager : MonoBehaviour
//{
//    private Callback<LobbyChatUpdate_t> lobbyChatUpdate;

//    private void Start()
//    {
//        lobbyChatUpdate =
//            Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
//    }

//    private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
//    {
//        SteamLobby lobbyList =
//            FindAnyObjectByType<SteamLobby>();

//        if (lobbyList != null)
//        {
//            lobbyList.RefreshLobby();
//        }

//        EChatMemberStateChange state =
//            (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;

//        if (state == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
//        {
//            Debug.Log("New Player Joined");
//        }

//        if (state == EChatMemberStateChange.k_EChatMemberStateChangeLeft)
//        {
//            Debug.Log("Player Left");
//        }
//    }
//}
using Steamworks;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public bool IsPlaying = true;

    // CanPlayerPlay() supprimée → logique déplacée dans NetworkManager
    // Si plus tard tu veux un système de vote/sélection depuis l'UI du lobby,
    // tu pourras repasser les IDs ici et les lire dans NetworkManager.

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