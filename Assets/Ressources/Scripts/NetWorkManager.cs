using UnityEngine;
using Mirror;

public class NetWorkManager : NetworkManager
{
    [Header("Scenes")]
    [Scene] public string lobbyScene;
    [Scene] public string gameScene;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log($"[Server] OnServerAddPlayer — conn: {conn.connectionId}");
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

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName == gameScene)
        {
            Debug.Log("[Server] Scène Game chargée, spawn des joueurs...");
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                SpawnPlayer(conn);
            }
        }
    }

    [Server]
    public void SpawnPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log($"[Server] SpawnPlayer appelé pour conn: {conn.connectionId}");

        if (conn.identity != null)
        {
            Debug.LogWarning($"[Server] Déjà spawné !");
            return;
        }

        Vector3 spawnPos = GetStartPosition()?.position ?? Vector3.zero;
        Quaternion spawnRot = GetStartPosition()?.rotation ?? Quaternion.identity;

        GameObject player = Instantiate(playerPrefab, spawnPos, spawnRot);
        NetworkServer.AddPlayerForConnection(conn, player);

        Debug.Log($"[Server] Joueur spawné !");
    }

    [Server]
    public void StartGame()
    {
        Debug.Log("[Server] Changement vers la scène Game...");
        ServerChangeScene(gameScene);
    }
}