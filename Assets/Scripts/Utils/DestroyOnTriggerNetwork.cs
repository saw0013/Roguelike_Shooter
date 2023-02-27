using System.Collections;
using System.Collections.Generic;
using Mirror; 
using UnityEngine;

public class DestroyOnTriggerNetwork : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other != null & other.tag == "Player") DestroyAsync();
    }

    private async void DestroyAsync()
    {
        await System.Threading.Tasks.Task.Delay(500);
        //gameObject.SetActive(false);
        if(gameObject) Destroy(gameObject);
    }
}
