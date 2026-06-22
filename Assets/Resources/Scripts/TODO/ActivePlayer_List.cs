using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ActivePlayer_List : Singleton_Obj<ActivePlayer_List>
{ 
    public List<string> PlayerIdSteam;
    public List<NetworkIdentity> PlayerIdMirror = new List<NetworkIdentity>();
}
    