using Mirror;
using TMPro;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] private TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        text.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
