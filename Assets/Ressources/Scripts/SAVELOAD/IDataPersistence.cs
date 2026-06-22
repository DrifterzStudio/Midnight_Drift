using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    public void LoadGame(IGameData data);

    public void SaveGame(IGameData data);

    public string getDataFileName();
}
