using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWheels : MonoBehaviour, IDataPersistence
{

    public string dataFileName;

    [Header("Customization")]
    public List<MeshRenderer> meshRenderer;
    public List<Material> materials;

    public Text wheelsText;

    public int materialIndex = 0;

    public static ChangeWheels instance;

    public void LoadGame(IGameData data)
    {
        SaveCustom tmp = data as SaveCustom;
        if (tmp != null)
        {
            materialIndex = tmp.currentMat;
        }
    }

    public void SaveGame(IGameData data)
    {
        SaveCustom tmp = data as SaveCustom;
        if (tmp != null)
        {
            tmp.currentMat = materialIndex;
        }
    }

    public string getDataFileName()
    {
        return dataFileName;
    }



    private void Awake()
    {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);

    }

    void Update()
    {
        instance = this;

        meshRenderer[0].material = materials[materialIndex];
        meshRenderer[1].material = materials[materialIndex];
        meshRenderer[2].material = materials[materialIndex];
        meshRenderer[3].material = materials[materialIndex];

        if (wheelsText != null) wheelsText.text = materialIndex.ToString();

    }

    public void OnButtonClicked()
    {
        if (materialIndex + 1 > materials.Count - 1) materialIndex = 0;
        else materialIndex += 1;
    }


    public int GetMatIndex()
    {
        return materialIndex;
    }

    public Material GetCurrentMat()
    {
        return materials[materialIndex];
    }
}
