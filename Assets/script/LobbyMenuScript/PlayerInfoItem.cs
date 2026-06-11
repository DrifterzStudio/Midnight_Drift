using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerInfoItem : MonoBehaviour
{
    public string PlayerName;
    public Texture2D Avatar;

    public TMP_Text PlayerNameText;
    public RawImage PlayerIcon;

    public GameObject Lobby = null;
    public GameObject PlayerPrefab;

    private void Awake()
    {
        GameObject parent = transform.parent.gameObject;
        Debug.Log(parent.name);
    }

    public void setPlayerValues()
    {
        PlayerIcon.texture = Avatar;
        PlayerNameText.text = PlayerName;
    }

    public void OnInvite()
    {
        GameObject test = Instantiate(PlayerPrefab) as GameObject;
        PlayerInfoItem newplayer = test.gameObject.GetComponent<PlayerInfoItem>();
        newplayer.PlayerNameText = PlayerNameText;
        newplayer.PlayerIcon = PlayerIcon;
        newplayer.setPlayerValues();
        test.transform.SetParent(Lobby.transform);
    }
}
