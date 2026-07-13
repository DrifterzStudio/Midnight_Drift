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

    [SerializeField] private Texture2D crownIcon;

    private bool init = false;

    private Callback<AvatarImageLoaded_t> avatarLoadedCallback;

    void Start()
    {
        avatarLoadedCallback = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    void OnDestroy()
    {
        if (init)
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

                // NAME
                TMP_Text comp = obj.transform.GetChild(1).GetComponent<TMP_Text>();
                comp.SetText(SteamFriends.GetFriendPersonaName(steamID));

                SetAvatar(obj, steamID);

                // break
                break;
            }
        }

        if (state == LobbyChatStateUpdate.Exit)
        {
            foreach (var obj in objects)
            {
                if (!obj.gameObject.activeSelf)
                    continue;

                TMP_Text comp = obj.transform.GetChild(1).GetComponent<TMP_Text>();
                if (comp.text != SteamFriends.GetFriendPersonaName(steamID))
                    continue;

                obj.SetActive(false);

                // break
                break;
            }
        }

        // Peu importe Enter ou Exit : on resynchronise la couronne sur tout le monde
        RefreshAllLeaderIcons();
    }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkServer.active && !NetworkClient.active)
            return;

        if (init)
            return;

        if (!init && Steam_Lobby.Instance)
        {
            init = true;
            List<ulong> initIds = Steam_Lobby.Instance.GetLobbyMembers();
            Steam_Lobby.Instance.lobbyUpdateCallBack = CallBack;
            foreach (ulong Id in initIds)
            {
                CSteamID steamID = new CSteamID(Id);

                foreach (var obj in objects)
                {
                    if (obj.gameObject.activeSelf)
                        continue;

                    obj.SetActive(true);

                    TMP_Text comp = obj.transform.GetChild(1).GetComponent<TMP_Text>();
                    comp.SetText(SteamFriends.GetFriendPersonaName(steamID));

                    SetAvatar(obj, steamID);

                    Steam_Lobby.Instance.lobbyUpdateCallBack = CallBack;
                    break;
                }
            }

            // Une fois tous les slots initiaux peuplés, on pose la couronne
            RefreshAllLeaderIcons();
        }
    }

    // ---------------------------------------------------------
    // OWNER / COURONNE — resynchronise TOUS les slots actifs
    // ---------------------------------------------------------
    private void RefreshAllLeaderIcons()
    {
        if (Steam_Lobby.Instance == null)
            return;

        CSteamID ownerId = SteamMatchmaking.GetLobbyOwner(Steam_Lobby.Instance.LobbyID);
        string ownerName = SteamFriends.GetFriendPersonaName(ownerId);

        foreach (var obj in objects)
        {
            if (!obj.gameObject.activeSelf)
                continue;

            TMP_Text comp = obj.transform.GetChild(1).GetComponent<TMP_Text>();
            bool isOwner = comp.text == ownerName;

            RawImage leaderIconComp = obj.transform.GetChild(3).GetComponent<RawImage>();
            if (leaderIconComp == null)
                continue;

            leaderIconComp.texture = crownIcon;
            leaderIconComp.gameObject.SetActive(isOwner);
        }
    }

    // ---------------------------------------------------------
    // PHOTO DE PROFIL
    // ---------------------------------------------------------
    private void SetAvatar(GameObject obj, CSteamID steamID)
    {
        int avatarHandle = SteamFriends.GetLargeFriendAvatar(steamID);

        if (avatarHandle == -1)
            return;

        RawImage pictureComp = obj.transform.GetChild(0).GetComponent<RawImage>();
        ApplyAvatar(pictureComp, avatarHandle);
    }

    private void ApplyAvatar(RawImage img, int avatarHandle)
    {
        SteamUtils.GetImageSize(avatarHandle, out uint width, out uint height);

        byte[] buffer = new byte[width * height * 4];
        SteamUtils.GetImageRGBA(avatarHandle, buffer, buffer.Length);

        Texture2D tex = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
        tex.LoadRawTextureData(buffer);
        tex.Apply();

        img.texture = FlipTextureVertically(tex);
    }

    private Texture2D FlipTextureVertically(Texture2D source)
    {
        int width = source.width;
        int height = source.height;
        Texture2D flipped = new Texture2D(width, height, source.format, false);

        for (int y = 0; y < height; y++)
        {
            flipped.SetPixels(0, y, width, 1, source.GetPixels(0, height - y - 1, width, 1));
        }

        flipped.Apply();
        return flipped;
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        string loadedName = SteamFriends.GetFriendPersonaName(callback.m_steamID);

        foreach (var obj in objects)
        {
            if (!obj.gameObject.activeSelf)
                continue;

            TMP_Text comp = obj.transform.GetChild(1).GetComponent<TMP_Text>();
            if (comp.text != loadedName)
                continue;

            RawImage pictureComp = obj.transform.GetChild(0).GetComponent<RawImage>();
            ApplyAvatar(pictureComp, callback.m_iImage);
        }
    }
}