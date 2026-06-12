using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static Scene_Controller;


public class CustomSceneData
{
    public string Slot = " ";
    public string Scene= " ";
}
public class Mirror_Manager :Singleton_Obj_MirrorManager<Mirror_Manager>
{
    public CustomSceneData SceneData { get; private set; } = new CustomSceneData();
    private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

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

        if (_prefabs.TryGetValue(SceneData.Scene, out GameObject prefab))
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

        Debug.Log($"Scene : {SceneData.Scene}");
        Debug.Log($"Prefab : {prefab.name}");

        Vector3 spawnPos = GetStartPosition()?.position ?? Vector3.zero;
        Quaternion spawnRot = GetStartPosition()?.rotation ?? Quaternion.identity;

        GameObject player = Instantiate(prefab, spawnPos, spawnRot);
        NetworkServer.AddPlayerForConnection(conn, player);

        Debug.Log($"[Server] Player spawn : {conn.connectionId}");
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        CustomSceneData _sceneData = Mirror_Manager.Instance.SceneData;
        customHandling = true;
        if(newSceneName != _sceneData.Scene)
            Debug.LogWarning($"scene not valid");

        SceneLoadOperation op = new SceneLoadOperation();
        op.OnOpCreated = (asyncOp) => loadingSceneAsync = asyncOp;

        Scene_Controller.Instance.NewTransition()
            .Load(_sceneData.Slot , newSceneName, op, true)
            .EnableOverlay(true)
            .Execute();
    }

    [Server]
    public void ChangeScene(string slot, string scene)
    {
        NetworkServer.SetAllClientsNotReady();
        networkSceneName = scene;
        SceneData.Slot = slot;
        SceneData.Scene = scene;
        OnServerChangeScene(scene);
        if (NetworkServer.active)
        {
            NetworkServer.SendToAll(new SceneMessage { sceneName = scene });
        }

        startPositionIndex = 0;
        startPositions.Clear();
    }

}
