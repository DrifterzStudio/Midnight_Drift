using UnityEngine;

[CreateAssetMenu(fileName = "VehicleDefinition", menuName = "Garage/Vehicle Definition")]
public class VehicleDefinition : ScriptableObject
{
    // Unique id used to build save file names, do not change it once used
    public string vehicleId;
    public string displayName;
    public GameObject prefab;
    public Sprite icon;
}