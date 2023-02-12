using System.Collections;
using System.Collections.Generic;
using Mirror; 
using UnityEngine;

public class DestroyOnTriggerNetwork : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other != null & other.tag == "Player") Destroy();
    }

    private async void Destroy()
    {
        await System.Threading.Tasks.Task.Delay(500);
        gameObject.SetActive(false);
    }
}
