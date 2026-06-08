using NUnit.Framework;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public string name;
    public Texture2D image;
    public GameObject FriendListview;
    public GameObject FriendListPrefab;

    private List<PlayerInfoItem> playerInfoItems = new List<PlayerInfoItem>();

    public void addFriend()
    {
        
        GameObject test = Instantiate(FriendListPrefab) as GameObject;
        PlayerInfoItem newplayer = test.gameObject.GetComponent<PlayerInfoItem>();
        newplayer.PlayerName = name;
        newplayer.Avatar = image;
        newplayer.setPlayerValues();
        test.transform.SetParent(FriendListview.transform);
        playerInfoItems.Add(newplayer);
    }
}
