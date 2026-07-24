using Mirror;
using Steamworks;
using UnityEngine;

public class SteamOverlay : MonoBehaviour
{
    private CallResult<NumberOfCurrentPlayers_t> _mNumberOfCurrentPlayers;
    public GameObject playerPrefab;

    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            _mNumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnNumberOfCurrentPlayers);
        }
    }

    private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bSuccess != 1 || bIOFailure)
        {
            Debug.Log("There was an error retrieving the NumberOfCurrentPlayers.");
        }
        else
        {
            Debug.Log("Player Number: " + pCallback.m_cPlayers);
        }
    }
    void Start()
    {
       
    }
}