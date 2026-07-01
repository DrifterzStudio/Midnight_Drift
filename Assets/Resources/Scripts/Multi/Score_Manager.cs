using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Score_Manager : Singleton_Obj_Net<Score_Manager>
{

    public readonly SyncDictionary<ulong, float> ScoreData = new();

    private void Update()
    {
        if (isServer)
            return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            foreach (var item in ScoreData)
            {
                string playerName = SteamFriends.GetFriendPersonaName(new CSteamID(item.Key));
                Debug.Log($"Score {playerName} ({item.Key}) = {item.Value}");
            }
        }
    }

    [Command(requiresAuthority = false)]

    public void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            foreach (var item in ScoreData)
            {
                string playerName = SteamFriends.GetFriendPersonaName(new CSteamID(item.Key));
                Debug.Log($"Score {playerName} ({item.Key}) = {item.Value}");
            }
        }
    }
    public void CmdAddPlayer(ulong steamId)
    {
        if(ScoreData.ContainsKey(steamId))
            Debug.LogError("con already exist");
        ScoreData.Add(steamId,0);
        Debug.Log($"Score conn add : {steamId}");
    }
    [Command(requiresAuthority = false)]
    public void CmdRemovePlayer(ulong steamId)
    {
        if (!ScoreData.ContainsKey(steamId))
            Debug.LogError("con don't exist");
        ScoreData.Remove(steamId);
        Debug.Log($"Score conn remove : {steamId}");
    }

    [Command(requiresAuthority = false)]
    public void CmdAddScoreForCon(ulong steamId, float scoreAdd)
    {
        if (!ScoreData.ContainsKey(steamId))
            Debug.LogError("con don't exist");
        ScoreData[steamId] += scoreAdd;
    }
    [Command(requiresAuthority = false)]
    public void CmdSetScoreForCon(ulong steamId, float scoreAdd)
    {
        if (!ScoreData.ContainsKey(steamId))
            Debug.LogError("con don't exist");
        ScoreData[steamId] = scoreAdd;
    }
    [Command(requiresAuthority = false)]
    public void CmdResetScoreForCon(ulong steamId)
    {
        if (!ScoreData.ContainsKey(steamId))
            Debug.LogError("con don't exist");
        ScoreData[steamId] = 0;
    }
}
