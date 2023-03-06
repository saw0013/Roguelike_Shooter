using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientXMPP : MonoBehaviour
{
    public void CreateAcc()
    {
        CoreXMPP.CreateAccount("test04", "test04");
    }
}
