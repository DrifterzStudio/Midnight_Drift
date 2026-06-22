using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Scene_Controller;




public class NetWorkManager : NetworkManager
{
    PlayerInfos playerInfos = Object.FindAnyObjectByType<PlayerInfos>();
    [Header("Scenes")]
    [Scene] public string LobbyScene;
    [Scene] public string GameScene;
    private Dictionary<string, GameObject> _prefabForScene  = new Dictionary<string, GameObject>();
    private string _nextSlot = " ";

    private string _activeScene = " ";


    private bool shouldSpawn = false;

    private void Start()
    {
        base.Start();
        _prefabForScene[GameScene] = playerPrefab;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // volontairement vide
    }

    public override void ServerChangeScene(string newSceneName)
    {
        // volontairement vide
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("[Client] Connecté au serveur !");
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"[Server] Client connecté — conn: {conn.connectionId}");
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        
        base.OnServerReady(conn);
       
        Debug.Log($"[Server] Client ready : {conn.connectionId}");

        if (_prefabForScene.TryGetValue(_activeScene, out GameObject prefab))
        {
            Debug.Log(playerInfos.Isplaying + " haaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa2");

            if (prefab != null) 
                SpawnPlayer(conn, playerPrefab);
        }
    }

    [Server]
    public void SpawnPlayer(NetworkConnectionToClient conn, GameObject prefab) // faire la condition for spawn only 2 player and the 2 oother only have access to the cameras of the players
    {
        if (conn.identity != null) return;
        Debug.Log(playerInfos.Isplaying + " haaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa1");
        Vector3 spawnPos = GetStartPosition()?.position ?? Vector3.zero;
        Quaternion spawnRot = GetStartPosition()?.rotation ?? Quaternion.identity;

        GameObject player = Instantiate(prefab, spawnPos, spawnRot);
        NetworkServer.AddPlayerForConnection(conn, player);

        Debug.Log($"[Server] Joueur spawné : {conn.connectionId}"); 
    }


    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        customHandling = true;

        SceneLoadOperation op = new SceneLoadOperation();
        op.OnOpCreated = (asyncOp) => loadingSceneAsync = asyncOp;

        Scene_Controller.Instance.NewTransition()
            .Load(_nextSlot, newSceneName, op, true)
            .EnableOverlay(true)
            .Execute();
    }


    [Server]
    public void StartGame()
    {
        shouldSpawn = true;
        Debug.Log("[Server] Changement vers la scène Game...");
        NetworkServer.SetAllClientsNotReady();
        networkSceneName = GameScene;
        _nextSlot = "Game";
        _activeScene = GameScene;
        OnServerChangeScene(GameScene);

        if (NetworkServer.active)
        {
            NetworkServer.SendToAll(new SceneMessage { sceneName = GameScene });
        }

        LobbyManager lobbymana = Object.FindAnyObjectByType<LobbyManager>();
        
        startPositionIndex = 0;
        startPositions.Clear();
    }
}