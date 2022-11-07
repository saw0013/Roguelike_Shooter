using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Connection : MonoBehaviour
{
    
    public NetworkManager NetworkManager;

    void Start()
    {
     if(Application.isBatchMode)  
         NetworkManager.StartClient();
    }

    public void JoinClient()
    {
        NetworkManager.networkAddress = "localhost";
        NetworkManager.StartClient();
    }
}
