using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] private TMP_Text nameTagGui;
    private string nameTag = string.Empty;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isLocalPlayer)
        {
            nameTagGui.gameObject.SetActive(false);
        }
        else
        {
            CmdSetName(SteamFriends.GetPersonaName());
        }

    }


    [SyncVar(hook = nameof(OnNameReceived))]
    private string steamName;

    // Appelé sur le joueur LOCAL uniquement
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetName(SteamFriends.GetPersonaName());
    }

 
    [Command]
    private void CmdSetName(string name)
    {
        steamName = name;
    }
    private void OnNameReceived(string oldName, string newName)
    {
        nameTag = newName;
        nameTagGui.SetText(nameTag);
    }

 
    void Update()
    {
        
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
