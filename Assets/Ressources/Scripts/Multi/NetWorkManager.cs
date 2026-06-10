using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Scene_Controller;

public class NetWorkManager : NetworkManager
{
    [Header("Scenes")]
    [Scene] public string LobbyScene;
    [Scene] public string GameScene;

    [Header("Game Rules")]
    [Tooltip("Combien de joueurs peuvent réellement contrôler leur voiture")]
    public int MaxActivePlayers = 2;

    private bool shouldSpawn = false;
    private int _spawnedCount = 0; // ← compteur de spawns, reset à chaque partie

    public override void OnServerAddPlayer(NetworkConnectionToClient conn) { }
    public override void ServerChangeScene(string newSceneName) { }

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

        // ← canPlay déclaré ICI, avant le if, pour être accessible partout
        _spawnedCount++;
        bool canPlay = _spawnedCount <= MaxActivePlayers;

        CarPlayState carState = player.GetComponent<CarPlayState>();
        if (carState != null)
        {
            carState.SetCanPlay(canPlay);
        }
        else
        {
            Debug.LogWarning("[Server] CarPlayState manquant sur le prefab !");
        }

        Debug.Log($"[Server] Joueur spawné : {conn.connectionId} | canPlay: {canPlay} ({_spawnedCount}/{MaxActivePlayers})");
    }

    [Server]
    public void StartGame()
    {
        shouldSpawn = true;
        _spawnedCount = 0; // ← reset pour une nouvelle partie

        Debug.Log("[Server] Changement vers la scène Game...");
        NetworkServer.SetAllClientsNotReady();
        networkSceneName = GameScene;

        OnServerChangeScene(GameScene);
        NetworkServer.isLoadingScene = true;

        SceneLoadOperation op = new SceneLoadOperation();
        op.OnOpCreated = (asyncOp) => loadingSceneAsync = asyncOp;

        Scene_Controller.Instance.NewTransition()
            .Load("Game", GameScene, op, true)
            .EnableOverlay(true)
            .Execute();

        if (NetworkServer.active)
        {
            NetworkServer.SendToAll(new SceneMessage { sceneName = GameScene });
        }

        LobbyManager lobbymana = Object.FindAnyObjectByType<LobbyManager>();
        if (lobbymana != null && lobbymana.IsPlaying)
        {
            startPositionIndex = 0;
            startPositions.Clear();
        }
    }
}