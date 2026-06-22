using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] private TMP_Text nameTagGui;

    private string _nameTag = string.Empty;

    [SyncVar(hook = nameof(OnNameReceived))]
    private string _steamName;

    public string GetName()
    {
        return _steamName;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
        {
            nameTagGui.gameObject.SetActive(false);
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        CmdSetName(SteamFriends.GetPersonaName());
    }

    [Command]
    private void CmdSetName(string name)
    {
        _steamName = name;
    }

    private void OnNameReceived(string oldName, string newName)
    {
        _nameTag = newName;
        nameTagGui.SetText(_nameTag);
    }

    private void LateUpdate()
    {
        if (Camera.main != null && !isLocalPlayer)
        {
            Vector3 direction = Camera.main.transform.position - nameTagGui.transform.position;

            direction.y = 0;
            if (direction != Vector3.zero)
            {
                nameTagGui.transform.rotation = Quaternion.LookRotation(-direction);
            }
        }
    }
}