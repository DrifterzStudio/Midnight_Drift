using System;
using System.IO;
using UnityEngine;

public class DataFileHandler
{

    // true if a file was actually read. false = no save yet for this key
    public bool load(IGameData data, string fullPath)
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
                return true;

            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }

        return false;
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
