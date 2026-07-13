using UnityEngine;


public interface IGameData
{
    
    public void setData(IGameData data);

    public string getDataDirPath();
    public string getDataFileName();
    public bool useEncryption();
    public string getEncryptionKey();
    public bool usePrettyPrint();


}
