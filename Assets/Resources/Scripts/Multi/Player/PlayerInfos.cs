using Mirror;
using Steamworks;
using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public class PlayerInfos : NetworkBehaviour
{
    public bool Isplaying = false;
    [SyncVar(hook = nameof(OnSteamIdReceived))]
    public ulong SteamId = 0;

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnStartClient()
    {
        if (!isLocalPlayer)
            return;
        ulong myId = SteamUser.GetSteamID().m_SteamID;
        string MyName = SteamFriends.GetPersonaName();
        GetComponent<Connect_callBack>().OnDisconnect = Disconnect;
        SendInfosPlayer(myId, MyName);
    }

    [Command]
    void SendInfosPlayer(ulong Id, string Name)
    {
        SteamId = Id;
    }
    private void OnSteamIdReceived(ulong oldId, ulong newId)
    {
        if (this == null || gameObject == null)
            return;
        Connect(newId);
        GetComponent<NetworkCamera>()?.OnSteamIdReady(newId);
    }

    void Connect(ulong id)
    {
        if (!isLocalPlayer)
            return;

        if (Score_Manager.Instance == null)
        {
            Debug.LogError("Score_Manager.Instance == null");
            return;
        }

        if (Score_Manager.Instance.ScoreData == null)
        {
            Debug.LogError("ScoreData == null");
            return;
        }

        if (!Score_Manager.Instance.ScoreData.ContainsKey(id))
        {
            Score_Manager.Instance.CmdAddPlayer(id);
        }
    }
    void Disconnect()
    {
        if (!NetworkServer.active)
            return;
        if (!NetworkClient.ready) return;
        if (!isLocalPlayer) return;


        if (ActivePlayer_List.Instance.Contains(SteamId))
            ActivePlayer_List.Instance.CmdRemove(SteamId);

        if (Score_Manager.Instance.ScoreData.ContainsKey(SteamId))
            Score_Manager.Instance.CmdRemovePlayer(SteamId);

        Debug.Log($"disconect : {SteamId}");
    }
}
