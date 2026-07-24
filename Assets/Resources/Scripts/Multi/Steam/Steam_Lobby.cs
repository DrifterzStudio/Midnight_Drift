using Edgegap;
using Mirror;
using Steamworks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum LobbyChatStateUpdate
{
    Enter,
    Exit
}



public class Steam_Lobby : Singleton_Obj<Steam_Lobby>
{
    // important callBack
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> LobbyEntered;
    protected Callback<LobbyChatUpdate_t> LobbyChatUpdate;
    // variable 
    public CSteamID LobbyID { get; private set; }

    private const string HostAddressKey = "HostAddress"; 
    public int MaxPlayer { get; private set; } = 4;
    private bool _isLocked = false;
    public Action<ulong, LobbyChatStateUpdate> lobbyUpdateCallBack = null;
    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogWarning("ERRORR");
            return;
        }


        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
    }
    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, MaxPlayer);
    }
    
    public void LockLobby()
    {
        if(_isLocked  || !LobbyID.IsValid())
            return;

        SteamMatchmaking.SetLobbyJoinable(LobbyID, true);
        _isLocked = true;
    }

    public void UnlockLobby()
    {
        if (!_isLocked || !LobbyID.IsValid())
            return;

        SteamMatchmaking.SetLobbyJoinable(LobbyID, false);
        _isLocked = false;
    }
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log("Lobby creation failed");
            return;
        }
        Mirror_Manager.Instance.StartHost();
        LobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
        
    }

    // Todo
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Join request received");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    // lobby Enter
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        LobbyID = new CSteamID(callback.m_ulSteamIDLobby);


        if (NetworkServer.active)
            return;

        Mirror_Manager.Instance.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        Mirror_Manager.Instance.StartClient();
    }

    // here the lobbyData
    private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
    {
        RefreshLobby();
        Debug.Log(callback.m_ulSteamIDLobby);
        EChatMemberStateChange state = (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;

        if (state == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
        {
            Debug.Log("New Player");
            if (lobbyUpdateCallBack != null)
                lobbyUpdateCallBack(callback.m_ulSteamIDUserChanged, LobbyChatStateUpdate.Enter);
        }

        if (state == EChatMemberStateChange.k_EChatMemberStateChangeLeft)
        {
            Debug.Log("A Player Left");
            if (lobbyUpdateCallBack != null)
                lobbyUpdateCallBack(callback.m_ulSteamIDUserChanged, LobbyChatStateUpdate.Exit);
        }

       
    }

    public void LeaveLobby()
    {
        if (!LobbyID.IsValid())
            return;

        SteamMatchmaking.LeaveLobby(LobbyID);
        LobbyID = CSteamID.Nil;
        _isLocked = false;
    }
    public void RefreshLobby()
    {
        //TODO if needed to lobby ui
    }

    public List<ulong> GetLobbyMembers()
    {
        List<ulong> members = new List<ulong>();

        if (!LobbyID.IsValid())
            return members;

        int count = SteamMatchmaking.GetNumLobbyMembers(LobbyID);

        for (int i = 0; i < count; i++)
        {
            members.Add(SteamMatchmaking.GetLobbyMemberByIndex(LobbyID, i).m_SteamID);
        }

        return members;
    }
}
