using Mirror;
using System;
using UnityEngine;
    public class Server_Data : NetworkBehaviour
    {
    public void Start()
    {
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
 

    public string GetSceneSlot() => _slot;
        public string GetSceneName() => _name;
    }
