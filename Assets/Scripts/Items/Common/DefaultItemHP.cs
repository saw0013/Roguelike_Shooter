using Mirror;
using UnityEngine;

public class DefaultItemHP : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other != null & other.tag == "Player") 
            Buff(other.gameObject);
    }

    private void Buff(GameObject player)
    {
        var _player = player.GetComponent<PlayerData>();
        _player.ChangeHealth(150);
        Destroy(gameObject);
    }
}
