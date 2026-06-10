using Mirror;
using UnityEngine;

public class CarPlayState : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnCanPlayChanged))]
    public bool canPlay = false;

    private RCCP_CarController _car;
    private RCCP_Engine _engine;

    private void Awake()
    {
        _car = GetComponent<RCCP_CarController>();
        _engine = GetComponentInChildren<RCCP_Engine>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
   
        ApplyState(canPlay);
    }

    [Server]
    public void SetCanPlay(bool value) => canPlay = value;

    private void OnCanPlayChanged(bool _, bool next)
    {
        ApplyState(next);
    }

    private void ApplyState(bool active)
    {
        if (isLocalPlayer && _car != null)
            _car.canControl = active;

        if (_engine != null)
        {
            if (active) _engine.StartEngine();
            else _engine.StopEngine();
        }
    }
}