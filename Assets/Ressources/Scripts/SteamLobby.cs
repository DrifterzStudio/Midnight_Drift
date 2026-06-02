using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    //calllback
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    //variables
    public ulong currentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private NetWorkManager manager;
    public static SteamLobby Instance;

    //GameObject
    public GameObject HostButton;
    public TMP_Text LobbyNameText;


    private void Start()
    {
        Instance = this;

        Debug.Log("Start");
        if (!SteamManager.Initialized)
            return;

        manager = GetComponent<NetWorkManager>();

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
       Debug.Log("Button appuyer");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
    }


    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log("Lobby creation failed");
            return;
        }
        manager.StartHost();
        currentLobbyID = callback.m_ulSteamIDLobby;
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
        LobbyNameText.text = "Lobby Name: " + SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Join request received");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //everione
        HostButton.SetActive(false);
        currentLobbyID = callback.m_ulSteamIDLobby;
        LobbyNameText.gameObject.SetActive(true);
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        //client
        if (NetworkServer.active)
        {
            return;
        }
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();
    }
    public void RefreshLobby()
    {
        if (SteamLobby.Instance == null)
        {
            Debug.Log("Pas de lobby instance");
            return;
        }

        CSteamID lobbyID = new CSteamID(SteamLobby.Instance.currentLobbyID);

        int count = SteamMatchmaking.GetNumLobbyMembers(lobbyID);

        Debug.Log("Joueurs dans le lobby : " + count);

        for (int i = 0; i < count; i++)
        {
            CSteamID member = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);

            string name = SteamFriends.GetFriendPersonaName(member);

            Debug.Log(name);
        }
    }
}