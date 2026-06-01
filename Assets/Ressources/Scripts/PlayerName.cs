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
        nameTag = SteamFriends.GetPersonaName();
        if (isLocalPlayer)
        {
            nameTagGui.gameObject.SetActive(false);
        }
        else
        {
            nameTagGui.SetText(nameTag);
        }

    }

    // Update is called once per frame
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
