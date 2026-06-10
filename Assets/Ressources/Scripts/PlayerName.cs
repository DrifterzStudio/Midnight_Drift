using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] private TMP_Text nameTagGui;
    private string nameTag = string.Empty;

    [SyncVar(hook = nameof(OnNameReceived))]
    private string steamName;

    [SyncVar(hook = nameof(OnScoreReceived))]
    private int networkScore;

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
        steamName = name;
    }

    [Command]
    public void CmdSetScore(int newScore)
    {
        networkScore = newScore;
    }

    private void OnNameReceived(string oldName, string newName)
    {
        nameTag = newName;
        nameTagGui.SetText(nameTag);
    }

    private void OnScoreReceived(int oldScore, int newScore)
    {
        networkScore = newScore;

        Debug.Log($"Score de {steamName} : {newScore}");

        // Si tu veux afficher le score dans un TMP_Text,
        // fais-le ici.
    }

    public int GetScore()
    {
        return networkScore;
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