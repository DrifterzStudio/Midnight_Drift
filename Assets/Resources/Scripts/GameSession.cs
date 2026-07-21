using UnityEngine;

// Small static holder, only knows which vehicle is currently selected
public static class GameSession
{
    public static VehicleDefinition SelectedVehicle { get; private set; }

    public static void SelectVehicle(VehicleDefinition vehicle)
    {
        SelectedVehicle = vehicle;
    }
}