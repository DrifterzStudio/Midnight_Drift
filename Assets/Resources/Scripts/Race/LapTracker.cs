using System;
using UnityEngine;

// lap counting + timing for one car, shared by solo (RaceManager) and multi. pure C# (just
// vector3/mathf), no mono/mirror/ui.
// for multi: one per player, on the SERVER. give every player's tracker the SAME finish line pose,
// then sync the outputs to the clients.
public class LapTracker
{
    // settings - keep the same in solo and multi
    public int MaxLaps = 5;
    public float LeaveDistance = 25f; // how far from the line before a crossing counts
    public float LineHalfWidth = 25f; // how far sideways from the line still counts as crossing it

    // read-only, sync these to the clients for the hud
    public int CurrentLap { get; private set; } = 1;
    public float RaceTime { get; private set; }
    public float LapTime { get; private set; }
    public float BestLap { get; private set; }
    public bool Started { get; private set; }  // true once the car leaves the line (clock running)
    public bool Finished { get; private set; }

    // hook your ClientRpc on these. heads up: RaceFinished is per PLAYER, so in multi you still need
    // a little server thing to decide when the whole race is over (first done / everyone done / timer)
    public event Action LapCompleted;
    public event Action RaceFinished;

    Vector3 linePosition;
    Vector3 lineForward;
    float prevSide;
    bool hasLeftStart;

    // the finish line. same pose for every player (one shared FinishLine on the track)
    public void SetLine(Vector3 position, Vector3 forward)
    {
        linePosition = position;
        forward.y = 0f;
        lineForward = forward.sqrMagnitude > 0.0001f ? forward.normalized : Vector3.forward;
        prevSide = 0f;
    }

    // call every frame with this player's car position (server side in multi)
    public void Tick(Vector3 carPosition, float dt)
    {
        if (Finished) return;

        if (!hasLeftStart && Vector3.Distance(carPosition, linePosition) > LeaveDistance)
        {
            hasLeftStart = true;
            Started = true; // clock starts when the car first leaves the line
        }

        if (Started)
        {
            RaceTime += dt;
            LapTime += dt;
        }

        DetectCrossing(carPosition);
    }

    void DetectCrossing(Vector3 carPosition)
    {
        Vector3 toCar = carPosition - linePosition;
        float side = Vector3.Dot(toCar, lineForward);
        float lateral = Vector3.Dot(toCar, Vector3.Cross(Vector3.up, lineForward));

        // went from behind the line to in front of it, close enough, and had left the start first
        if (prevSide < 0f && side >= 0f && Mathf.Abs(lateral) <= LineHalfWidth &&
            hasLeftStart && Started && !Finished)
            CompleteLap();

        prevSide = side;
    }

    void CompleteLap()
    {
        if (BestLap <= 0f || LapTime < BestLap)
            BestLap = LapTime;

        LapTime = 0f;
        hasLeftStart = false; // need to drive away again before the next lap counts

        if (CurrentLap >= MaxLaps)
        {
            Finished = true;
            RaceFinished?.Invoke();
        }
        else
        {
            CurrentLap++;
            LapCompleted?.Invoke();
        }
    }
}
