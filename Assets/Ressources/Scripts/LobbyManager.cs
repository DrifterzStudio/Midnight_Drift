using UnityEngine;
using Steamworks;

public class LobbyManager : MonoBehaviour
{
    private Callback<LobbyChatUpdate_t> lobbyChatUpdate;

    private void Start()
    {
        lobbyChatUpdate =
            Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
    {
        Debug.Log("Lobby mis à jour");

        SteamLobby lobbyList =
            FindAnyObjectByType<SteamLobby>();

        if (lobbyList != null)
        {
            lobbyList.RefreshLobby();
        }

        EChatMemberStateChange state =
            (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;

        if (state == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
        {
            Debug.Log("Un joueur a rejoint");
        }

        if (state == EChatMemberStateChange.k_EChatMemberStateChangeLeft)
        {
            Debug.Log("Un joueur a quitté");
        }
    }
}