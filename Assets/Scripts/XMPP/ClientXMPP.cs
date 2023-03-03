using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientXMPP : MonoBehaviour
{
    public void CreateAcc()
    {
        CoreXMPP.CreateAccount("test03", "test03");
    }
}
