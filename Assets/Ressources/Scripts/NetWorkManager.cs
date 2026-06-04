using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Scene_Controller;

public class NetWorkManager : NetworkManager
{
    [Header("Scenes")]
    [Scene] public string LobbyScene;
    [Scene] public string GameScene;

    private bool shouldSpawn = false;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // volontairement vide
    }
    public override void ServerChangeScene(string newSceneName)
    {
        // Tu notifies Mirror que la scène change SANS qu'il la charge lui-même
        Debug.Log("CHANGEEEEEE");
        if (NetworkServer.active)
        {
            // notify all clients about the new scene
            NetworkServer.SendToAll(new SceneMessage
            {
                sceneName = GameScene
            });
        }

        startPositionIndex = 0;
        startPositions.Clear();
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
        {
            SpawnPlayer(conn);
        }
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

    [Server]
    public void StartGame()
    {
        shouldSpawn = true;
        Debug.Log("[Server] Changement vers la scène Game...");
        NetworkServer.SetAllClientsNotReady();
        networkSceneName = GameScene;

        // Let server prepare for scene change
        OnServerChangeScene(GameScene);

        // set server flag to stop processing messages while changing scenes
        // it will be re-enabled in FinishLoadScene.
        NetworkServer.isLoadingScene = true;
        SceneLoadOperation op = new SceneLoadOperation();
        op.OnOpCreated = (asyncOp) => loadingSceneAsync = asyncOp;
        //loadingSceneAsync = SceneManager.LoadSceneAsync(GameScene);
        Scene_Controller.Instance.NewTransition()
            .Load("Game", GameScene, op, true)
            .EnableOverlay(true)
            .Execute();
        // ServerChangeScene can be called when stopping the server
        // when this happens the server is not active so does not need to tell clients about the change
       
    }
}