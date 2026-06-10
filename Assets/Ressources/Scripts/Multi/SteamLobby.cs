using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SteamLobby : MonoBehaviour
{
    // callbacks
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyChatUpdate_t> lobbyChatUpdate;

    // variables
    public ulong currentLobbyID;
    private const string HostAddressKey = "HostAddress";
    [SerializeField] private NetWorkManager manager;
    [SerializeField] private int maxSize = 4;
    public static SteamLobby Instance;
    bool isInit = false;

    // GameObjects
    public GameObject HostButton;
    public TMP_Text LobbyNameText;
    public GameObject StartGameButton;

    private void Start()
    {
        Instance = this;
        HostButton.SetActive(false);
        TryInit();
    }

    private void TryInit()
    {
        if (!SteamManager.Initialized || isInit)
            return;

        isInit = true;
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);

        // Steam prêt → on affiche le bouton Host
        HostButton.SetActive(true);
        Debug.Log("Steam initialisé !");
    }

    private void Update()
    {
        if (!isInit)
            TryInit();

        if (NetworkServer.active && currentLobbyID != 0 && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartGame();
        }
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, maxSize);
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
        // tout le monde
        HostButton.SetActive(false);
        currentLobbyID = callback.m_ulSteamIDLobby;
        LobbyNameText.gameObject.SetActive(true);
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        if (NetworkServer.active)
        {
            // host uniquement
            if (StartGameButton != null)
                StartGameButton.SetActive(true);
            return;
        }

        // client uniquement
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();
    }

    public void StartGame()
    {
        if (!NetworkServer.active) return;

        // On passe par le NetworkManager qui gère le changement de scène
        // Le spawn se fait automatiquement dans OnServerSceneChanged()
        manager.StartGame();

        Debug.Log("[Server] Lancement de la partie !");
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
    {
        RefreshLobby();

        EChatMemberStateChange state = (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;

        if (state == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
            Debug.Log("New Player");

        if (state == EChatMemberStateChange.k_EChatMemberStateChangeLeft)
            Debug.Log("A Player Left");
    }

    public void RefreshLobby()
    {
        if (SteamLobby.Instance == null)
        {
            Debug.Log("No Lobby");
            return;
        }

        CSteamID lobbyID = new CSteamID(SteamLobby.Instance.currentLobbyID);
        int count = SteamMatchmaking.GetNumLobbyMembers(lobbyID);

        Debug.Log("Lobby Players : " + count);

        for (int i = 0; i < count; i++)
        {
            CSteamID member = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);
            string name = SteamFriends.GetFriendPersonaName(member);
            Debug.Log(name);
        }
    }
}