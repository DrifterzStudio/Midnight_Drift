using Mirror;
using UnityEngine;

public class CustomSceneData
{
    public string Slot = " ";
    public string Name = " ";
}

public class Server_Data : NetworkBehaviour
{
    [SyncVar] private CustomSceneData _data = new CustomSceneData();

    [Server]
    public void SetSceneData(string slot, string name)
    {
        _data.Slot = slot;
        _data.Name = name;
    }
    [Client]
    public string GetSceneSlot()
    {
        return _data.Slot;
    }
    [Client]
    public string GetSceneName()
    {
        return _data.Name;
    }

}
