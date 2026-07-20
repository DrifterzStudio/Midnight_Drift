using Mirror;
using Steamworks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class LeaderBoard_script : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objects = new List<GameObject>();

    private bool init = false;
    [SerializeField] private GameObject canva;
    private Callback<AvatarImageLoaded_t> avatarLoadedCallback;
    void Start()
    {
        Score_Manager.Instance.ScoreData.OnChange += OnScoreDataChanged;
        avatarLoadedCallback = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    
    private void OnDestroy()
    {
        Score_Manager.Instance.ScoreData.OnChange -= OnScoreDataChanged;
    }

    private void Update()
    {
       

        if (!NetworkServer.active && !NetworkClient.active)
            return;

       


        if (Keyboard.current.tabKey.wasPressedThisFrame && init)
        {
            canva.SetActive(!canva.activeSelf);
        }


        if (!init && Score_Manager.Instance)
        {
            init = true;
          
            foreach (var pair in Score_Manager.Instance.ScoreData)
            {
                CSteamID steamID = new CSteamID(pair.Key);

                foreach (var obj in objects)
                {
                    if (obj.gameObject.activeSelf)
                        continue;

                    obj.SetActive(true);

                    TMP_Text comp = obj.transform.GetChild(1).GetComponent<TMP_Text>();
                    comp.SetText(SteamFriends.GetFriendPersonaName(steamID));

                    SetAvatar(obj, steamID);
                    TMP_Text score = obj.transform.GetChild(2).GetComponent<TMP_Text>();
                    score.SetText($"score : {pair.Value}");
                    break;
                }
            }

          
        }

    }

    private void OnScoreDataChanged(
        SyncDictionary<ulong, float>.Operation op,
        ulong key,
        float value)
    {
        if(!NetworkServer.active ||!NetworkClient.active) return;

        CSteamID steamID = new CSteamID(key);

        switch (op)
        {
            case SyncDictionary<ulong, float>.Operation.OP_ADD:
            {
                Debug.Log($"Ajout : {key} -> {value}");

                foreach (GameObject obj in objects)
                {
                    if(obj.activeSelf)
                        continue;

                    obj.SetActive(true);
                    TMP_Text comp = obj.transform.GetChild(1).GetComponent<TMP_Text>();
                    comp.SetText(SteamFriends.GetFriendPersonaName(steamID));
                    
                    SetAvatar(obj, steamID);
                    TMP_Text score = obj.transform.GetChild(2).GetComponent<TMP_Text>();
                    score.SetText($"score : {value}");
                        return;
                }

                break;
            }
            case SyncDictionary<ulong, float>.Operation.OP_SET:
            {
                Debug.Log($"Modification : {key} -> {value}");

                foreach (GameObject obj in objects)
                {
                    if (!obj.activeSelf)
                        continue;
                    TMP_Text comp = obj.transform.GetChild(1).GetComponent<TMP_Text>();
                    if (comp.text != SteamFriends.GetFriendPersonaName(steamID))
                        continue;
                    TMP_Text score = obj.transform.GetChild(2).GetComponent<TMP_Text>();
                    score.SetText($"score : {value}");
                }
                break;
            }

            case SyncDictionary<ulong, float>.Operation.OP_REMOVE:
            {

                Debug.Log($"Suppression : {key}");
                foreach (GameObject obj in objects)
                {
                    if (!obj.activeSelf)
                        continue;
                    TMP_Text comp = obj.transform.GetChild(1).GetComponent<TMP_Text>();
                    if (comp.text != SteamFriends.GetFriendPersonaName(steamID))
                        continue;
                    obj.SetActive(false);
                }
                break;

            }

            case SyncDictionary<ulong, float>.Operation.OP_CLEAR:
            {
                Debug.Log("Dictionnaire vidé");
                foreach (GameObject obj in objects)
                {
                    if (obj.activeSelf)
                        obj.SetActive(false);
                }
                break;
            }


        }
    }

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
