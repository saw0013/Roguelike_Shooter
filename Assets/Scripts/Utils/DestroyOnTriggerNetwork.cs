using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cosmoground;
using Mirror;
using UnityEngine;

public class DestroyOnTriggerNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject MoveParticleToObject;

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other != null & other.tag == "Player")
        {
            RpcEffect(other.transform);
            DestroyAsync();
        }
    }

    [ClientRpc]
    void RpcEffect(Transform player)
    {
        if (!player.Find(MoveParticleToObject.name))
        {
            var Effect = Instantiate(MoveParticleToObject, new Vector3(player.position.x, player.position.y + 1, player.position.z), Quaternion.Euler(-90, 0, 0), player);
            Effect.GetComponent<NetworkMatch>().matchId = player.GetComponent<NetworkMatch>().matchId;
        }
    }

    private async void DestroyAsync()
    {
        if (gameObject == null) return;

        await System.Threading.Tasks.Task.Delay(500);
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
