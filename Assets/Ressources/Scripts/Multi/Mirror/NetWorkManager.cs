using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Scene_Controller;




public class NetWorkManager : NetworkManager
{
    [Header("Scenes")]
    [Scene] public string LobbyScene;
    [Scene] public string GameScene;

    private string _nextSlot = "";

    // _spawnedCount et MaxActivePlayers supprimés
    // → la logique canPlay est maintenant dans CarPlayState.CmdRegisterSteamID

    private bool shouldSpawn = false;

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

        if (shouldSpawn)
            SpawnPlayer(conn);
    }

    [Server]
    public void SpawnPlayer(NetworkConnectionToClient conn)
    {
        if (conn.identity != null) return;

        Vector3 spawnPos = GetStartPosition()?.position ?? Vector3.zero;
        Quaternion spawnRot = GetStartPosition()?.rotation ?? Quaternion.identity;

        GameObject player = Instantiate(playerPrefab, spawnPos, spawnRot);
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
        OnServerChangeScene(GameScene);

        //NetworkServer.isLoadingScene = true;
        //SceneLoadOperation op = new SceneLoadOperation();
        //op.OnOpCreated = (asyncOp) => loadingSceneAsync = asyncOp;

        //Scene_Controller.Instance.NewTransition()
        //    .Load("Game", GameScene, op, true)
        //    .EnableOverlay(true)
        //    .Execute();

        if (NetworkServer.active)
        {
            NetworkServer.SendToAll(new SceneMessage { sceneName = GameScene });
        }

        LobbyManager lobbymana = Object.FindAnyObjectByType<LobbyManager>();
        
        startPositionIndex = 0;
        startPositions.Clear();
    }
}