using System;
using System.IO;
using UnityEngine;

public class DataFileHandler
{

    public void load(IGameData data, string fullPath)
    {
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = File.ReadAllText(fullPath);

                if(data.useEncryption())
                {
                    dataToLoad = EncryptDecrypt(dataToLoad, data.getEncryptionKey());
                }

                JsonUtility.FromJsonOverwrite(dataToLoad, data);

            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        else {
            Debug.LogError("Path not found: " + fullPath);
        }

    }

    public void save(IGameData data, string fullPath)
    {
        try
        {
            string dataToSave = JsonUtility.ToJson(data, data.usePrettyPrint());

            if (data.useEncryption())
                dataToSave = EncryptDecrypt(dataToSave, data.getEncryptionKey());

            File.WriteAllText(fullPath, dataToSave);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    private string EncryptDecrypt(string data, string encryptionKey)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionKey[i % encryptionKey.Length]);
        }
        return modifiedData;
    }

}
