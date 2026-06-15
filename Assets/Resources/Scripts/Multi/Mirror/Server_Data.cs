using Mirror;
using System;
using UnityEngine;
    public class Server_Data : NetworkBehaviour
    {
        private void Awake()
        {
            NetworkClient.RegisterHandler<custom_change_scene>(OnSceneChangeMessage);
        }

        [SyncVar(hook = nameof(OnSlotChanged))] private string _slot = " ";
        [SyncVar(hook = nameof(OnNameChanged))] private string _name = " ";

        private void OnSlotChanged(string oldVal, string newVal) => _slot = newVal;
        private void OnNameChanged(string oldVal, string newVal) => _name = newVal;
        [Server] public void SetSceneData(string slot, string name)
        {
            _slot = slot;
            _name = name;
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

    public string GetSceneSlot() => _slot;
        public string GetSceneName() => _name;
    }
