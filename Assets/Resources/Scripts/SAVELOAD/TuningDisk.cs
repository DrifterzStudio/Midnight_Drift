using System.IO;
using UnityEngine;

// loads a vehicle's saved tuning from disk, for races launched without the garage. the container
// sits on a throwaway GameObject - destroy it after reading.
public static class TuningDisk
{
    public static T Load<T>(string folder, string prefix, string vehicleId) where T : Component, IGameData
    {
        if (string.IsNullOrEmpty(vehicleId))
            return null;

        string path = Path.Combine(Application.persistentDataPath, folder, prefix + "_" + vehicleId + ".json");

        if (!File.Exists(path))
            return null;

        GameObject temp = new GameObject("TempTuning");
        T container = temp.AddComponent<T>();

        if (new DataFileHandler().load(container, path))
            return container;

        Object.Destroy(temp);
        return null;
    }
}
