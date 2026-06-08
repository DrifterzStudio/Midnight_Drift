//using Unity.Netcode;
//using UnityEngine;

//public class NetworkUI : MonoBehaviour
//{
//    private void OnGUI()
//    {
//        if (!NetworkManager.Singleton.IsClient &&
//            !NetworkManager.Singleton.IsServer)
//        {
//            if (GUI.Button(new Rect(10, 10, 100, 40), "Host"))
//            {
//                NetworkManager.Singleton.StartHost();
//            }

//            if (GUI.Button(new Rect(10, 60, 100, 40), "Client"))
//            {
//                NetworkManager.Singleton.StartClient();
//            }

//            if (GUI.Button(new Rect(10, 110, 100, 40), "Server"))
//            {
//                NetworkManager.Singleton.StartServer();
//            }
//        }
//    }
//}