using Mirror;
using Steamworks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiLobbySlot
{
    [SerializeField] private RawImage profilePicture;
    [SerializeField] private TMP_Text pseudoText;
    [SerializeField] private RawImage leaderIcon;
}

public class RefreshLobbyUi : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> objects = new List<GameObject>();

    private bool init = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnDestroy()
    {
        if(init) 
            Steam_Lobby.Instance.lobbyUpdateCallBack = null;

    }
    public void CallBack(ulong steamIDU, LobbyChatStateUpdate state)
    {
        CSteamID steamID = new CSteamID(steamIDU);
        if (state == LobbyChatStateUpdate.Enter)
        {
            foreach (var obj in objects)
            {
                if (obj.gameObject.activeSelf)
                    continue;
             
                obj.SetActive(true);

                // TODo all operation

                // NAME
                TMP_Text comp = obj.transform.GetChild(1).GetComponent<TMP_Text>();
                comp.SetText(SteamFriends.GetFriendPersonaName(steamID));

                // break
                return;
            }
        }

        if (state == LobbyChatStateUpdate.Exit)
        {
            foreach (var obj in objects)
            {
                if (!obj.gameObject.activeSelf)
                    continue;

                TMP_Text comp = obj.transform.GetChild(1).GetComponent<TMP_Text>();
                if(comp.text != SteamFriends.GetFriendPersonaName(steamID))
                    continue;

                obj.SetActive(false);


                // break
                return;
            }
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if(!NetworkServer.active && !NetworkClient.active)
            return;
            
        if(init)
            return;

        if(!init && Steam_Lobby.Instance )
        {
            init = true;
            List<ulong> initIds = Steam_Lobby.Instance.GetLobbyMembers();
            Steam_Lobby.Instance.lobbyUpdateCallBack = CallBack;
            foreach (ulong Id in initIds)
            {
                CSteamID steamID = new CSteamID(Id);
                foreach (var obj in objects)
                {
                    if(obj.gameObject.activeSelf)
                        continue;

                    obj.SetActive(true);

                    //TODO all operation

                    // NAME 
                    TMP_Text comp = obj.transform.GetChild(1).GetComponent<TMP_Text>();
                    comp.SetText(SteamFriends.GetFriendPersonaName(steamID));


                    // break
                    Steam_Lobby.Instance.lobbyUpdateCallBack = CallBack;
                    break;
                }
            }
        }

      
    }
}
