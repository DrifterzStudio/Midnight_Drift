using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static Scene_Controller;


public struct custom_change_scene : NetworkMessage
{
    public string Slot;
    public string Scene;
}

public class Mirror_Manager :Singleton_Obj_MirrorManager<Mirror_Manager>
{

    private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
    [SerializeField] private Server_Data dataObj;
    public void RegisterPrefab(string scene, GameObject prefabObject)
    {
        if (!NetworkClient.prefabs.ContainsValue(prefabObject))
            NetworkClient.RegisterPrefab(prefabObject);
        AddPrefabs(scene, prefabObject);
    }

    private void AddPrefabs(string SceneName, GameObject prefab)
    {
        if (_prefabs.ContainsKey(SceneName))
        {
            _prefabs[SceneName] = prefab;
            return;
        }
        _prefabs.Add(SceneName, prefab);
    }

    public void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            PlayerInfos info = conn.identity.GetComponent<PlayerInfos>();
            if (info != null && ActivePlayer_List.Instance.Contains(info.SteamId))
                ActivePlayer_List.Instance.CmdRemove(info.SteamId);
        }

        base.OnServerDisconnect(conn);
    }
    public override void Start()
    {
        base.Start();
        Debug.Log("Mirror_Manager start success");
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // volontairement vide
    }

    public override void ServerChangeScene(string newSceneName)
    {
        // volontairement vide
    }
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        Debug.Log($"[Server] Client ready : {conn.connectionId}");

        if (_prefabs.TryGetValue(dataObj.GetSceneName(), out GameObject prefab))
        {
            if (prefab != null)
                SpawnPlayer(conn, prefab);
        }
    }

    [Server]
    public void SpawnPlayer(NetworkConnectionToClient conn, GameObject prefab)
    {

        if (conn.identity != null)
        {
            Debug.Log("Already Spawn");
            return;
        }
        if (prefab == null)
        {
            Debug.LogWarning($"[Server] No prefab for player : {conn.connectionId}");
            return;
        }

        Debug.Log($"Scene : {dataObj.GetSceneName()}");
        Debug.Log($"Prefab : {prefab.name}");

        Vector3 spawnPos = GetStartPosition()?.position ?? Vector3.zero;
        Quaternion spawnRot = GetStartPosition()?.rotation ?? Quaternion.identity;

        GameObject player = Instantiate(prefab, spawnPos, spawnRot);
        NetworkServer.AddPlayerForConnection(conn, player);

        Debug.Log($"[Server] Player spawn : {conn.connectionId}");
    }
    
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        customHandling = true;
    }

   

    [Server]
    public void ChangeScene(string slot, string scene)
    {
      
        NetworkServer.SetAllClientsNotReady();
        networkSceneName = scene;
        NetworkServer.isLoadingScene = true;

        dataObj.SetSceneData(slot,scene);
        OnServerChangeScene(scene);

        if (NetworkServer.active)
        {
            NetworkServer.SendToAll(new custom_change_scene { Slot = slot,Scene  = scene});
           
        }
       
        startPositionIndex = 0;
        startPositions.Clear();
    }

}
