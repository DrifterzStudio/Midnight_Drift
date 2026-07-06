    using Mirror;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
using UnityEngine.SceneManagement;
    using static Scene_Controller;



  

    public class Mirror_Manager :Singleton_Obj_MirrorManager<Mirror_Manager>
    {
        public struct custom_change_scene : NetworkMessage
        {
            public string Slot;
            public string Scene;
        }
    private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
       // private Server_Data dataObj;
        public void RegisterPrefab(string scene, GameObject prefabObject)
        {
            if (!NetworkClient.prefabs.ContainsValue(prefabObject))
                NetworkClient.RegisterPrefab(prefabObject);
            AddPrefabs(scene, prefabObject);
        }

        public void SetData(Server_Data data)
        {
            //dataObj = data;
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

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            if (conn.identity != null)
            {
                conn.identity.GetComponent<Connect_callBack>()?.OnDisconnect?.Invoke();
                PlayerInfos info = conn.identity.GetComponent<PlayerInfos>();
                if (info != null && ActivePlayer_List.Instance.Contains(info.SteamId))
                    ActivePlayer_List.Instance.CmdRemove(info.SteamId);
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if (conn.identity != null)
            {
                conn.identity.GetComponent<Connect_callBack>()?.OnConnect?.Invoke();
            }
        }

        public override void Start()
        {
            base.Start();
            //NetworkClient.RegisterHandler<custom_change_scene>(OnSceneChangeMessage);
            Debug.Log("Mirror_Manager start success");
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            //dataObj = FindFirstObjectByType<Server_Data>();
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();
            NetworkClient.ReplaceHandler<custom_change_scene>(OnSceneChangeMessage);
        }
    private void OnSceneChangeMessage(custom_change_scene msg)
        {
            Debug.LogWarning($"Message reçu slot:{msg.Slot} scene:{msg.Scene}");

            Scene_Controller.SceneLoadOperation op = new Scene_Controller.SceneLoadOperation();
            op.OnOpCreated = (asyncOp) => NetworkManager.loadingSceneAsync = asyncOp;

            Scene_Controller.Instance.NewTransition()
                .Load(msg.Slot, msg.Scene, op, true)
                .EnableOverlay(true)
                .Execute();

        }


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // volontairement vide
        }

        public override void ServerChangeScene(string newSceneName)
        {
            base.OnServerSceneChanged(newSceneName);
            NetworkServer.isLoadingScene = false;
        }
        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);
            Debug.Log($"[Server] Client ready : {conn.connectionId}");

            if (_prefabs.TryGetValue(SceneManager.GetActiveScene().name, out GameObject prefab))
            {

                Debug.LogError($"load :{SceneManager.GetActiveScene().name}");
                if (prefab != null)
                    SpawnPlayer(conn, prefab);
            }
        }
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Debug.Log("OnClientDisconnect appelé");
        }

    public override void OnStopClient()
    {
        Debug.Log("Client trying to stop");
            base.OnStopClient(); 
            Steam_Lobby.Instance.LeaveLobby();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
             Scene_Controller.Instance.NewTransition()
                .Load("Menu", "Menu", true)
                .Unload("Multi_Server")
                .Unload("Multi_Game")
                .EnableOverlay(true)
                .Execute();
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

            Debug.Log($"Scene : {SceneManager.GetActiveScene()}");
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
        Debug.Log($"[DEBUG-Replay] Avant ChangeScene -> Objets réseau actifs : {NetworkServer.spawned.Count}");
        if (NetworkServer.isLoadingScene)
        {
            Debug.LogWarning("[Server] ChangeScene ignoré : un chargement est déjà en cours.");
            return;
        }

        NetworkServer.SetAllClientsNotReady();
        networkSceneName = scene;
        NetworkServer.isLoadingScene = true;


        if (NetworkServer.active)
        {
            NetworkServer.SendToAll(new custom_change_scene { Slot = slot, Scene = scene });
        }

        OnServerChangeScene(scene);

        startPositionIndex = 0;
        startPositions.Clear();
    }

}
