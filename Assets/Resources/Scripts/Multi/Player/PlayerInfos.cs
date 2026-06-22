using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerInfos : NetworkBehaviour
{
    public bool Isplaying = false;
    [SyncVar]
    public ulong SteamId = 0;

    [SyncVar]
    public string SteamName = "";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnStartClient()
    {
        if (!isLocalPlayer)
            return;
        ulong myId = SteamUser.GetSteamID().m_SteamID;
        string MyName = SteamFriends.GetPersonaName();
        SendInfosPlayer(myId, MyName);
    }

    [Command]
    void SendInfosPlayer(ulong Id, string Name)
    {
        SteamId = Id;
        SteamName = Name;
    }
}
