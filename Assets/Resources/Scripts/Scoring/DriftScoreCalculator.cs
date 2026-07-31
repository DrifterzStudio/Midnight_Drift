using System;

// drift scoring, shared by solo (Score) and multi (PlayerScore). pure C#, no unity/mirror/ui.
// for multi: one per player, on the SERVER. feed it Tick() and sync the values below to the clients.
// don't run it client-side or the score counts twice.
public class DriftScoreCalculator
{
    // settings - keep the same in solo and multi
    public float DriftSlipThreshold = 0.25f; // how sideways you need to be to count as drifting
    public float DriftTimeoutDelay = 2f;     // delay before a stopped drift banks
    public float PointsRate = 2f;            // points/sec (x speed), bump it for a faster score
    public float MultiplierStepTime = 0.8f;  // seconds of drift for each +1 on the multiplier
    public int MaxMultiplier = 10;

    // read-only, sync these to the clients for the hud
    public float Score { get; private set; }          // banked total
    public float PendingPoints { get; private set; }  // the "+X" you see while drifting
    public int Multiplier { get; private set; } = 1;  // current multiplier (x1..MaxMultiplier)
    public bool IsAccumulating { get; private set; }  // drifting right now?
    public bool Ended { get; private set; }

    // hook your ClientRpc on these (bank sound, crash shake...)
    public event Action Banked; // pending drift just got added to Score
    public event Action Lost;   // crashed, pending drift wiped

    float meters;
    float timer;
    float driftTime;

    // call every frame with the car's slip + speed (server side in multi)
    public void Tick(float sidewaysSlip, float speed, float dt)
    {
        if (Ended) return;

        IsAccumulating = Math.Abs(sidewaysSlip) >= DriftSlipThreshold && speed >= 0f;

        if (IsAccumulating)
            Accumulate(speed, dt);
        else
            HandleTimeout(dt);

        UpdateMultiplier();
    }

    // flat bonus into the banked total (near-miss)
    public void AddBonus(int points)
    {
        if (Ended || points <= 0) return;
        Score += points;
    }

    // call when the car hits an obstacle (server side). loses the current drift, keeps the score
    public void RegisterCollision()
    {
        bool hadPoints = (int)PendingPoints > 0;
        ResetRun();
        if (hadPoints)
            Lost?.Invoke();
    }

    // ends the run, banks what's still pending, returns the total. call it at the finish line
    public float Finalize()
    {
        if (!Ended && (int)PendingPoints > 0)
            Score += (int)PendingPoints * Multiplier;

        Ended = true;
        return Score;
    }

    void Accumulate(float speed, float dt)
    {
        meters += Math.Abs(speed) * dt * PointsRate;
        timer = 0f;
        driftTime += dt;
        PendingPoints += meters;
        meters = 0f;
    }

    void HandleTimeout(float dt)
    {
        if ((int)PendingPoints <= 0)
        {
            ResetRun();
            return;
        }

        // grace: the mult stays while you straighten a bit, so you can chain, until it banks
        timer += dt;
        if (timer >= DriftTimeoutDelay)
            Bank();
    }

    void Bank()
    {
        bool hadPoints = (int)PendingPoints > 0;
        if (hadPoints)
            Score += (int)PendingPoints * Multiplier;

        ResetRun();

        if (hadPoints)
            Banked?.Invoke();
    }

    // wipe the current drift - points, timers and mult back to base
    void ResetRun()
    {
        PendingPoints = 0f;
        meters = 0f;
        timer = 0f;
        driftTime = 0f;
        Multiplier = 1;
    }

    void UpdateMultiplier()
    {
        int steps = (int)(driftTime / MultiplierStepTime);
        Multiplier = Math.Min(1 + steps, MaxMultiplier);
    }
}
