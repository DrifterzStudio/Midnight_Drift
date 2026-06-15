using System;
using Mirror;
using UnityEngine;
    public class Server_Data : NetworkBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
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

        private void Update()
        {
            Debug.Log($"slot : {_slot}" + $" name : {_name}");
        }

        public string GetSceneSlot() => _slot;
        public string GetSceneName() => _name;
    }
