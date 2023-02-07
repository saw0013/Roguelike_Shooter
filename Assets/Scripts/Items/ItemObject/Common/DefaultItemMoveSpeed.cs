using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefaultItemMoveSpeed : NetworkBehaviour
{
    [SerializeField] private GameObject _imageItem;

    private PlayerData _owner;

    internal NetworkMatch networkMatch;

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other != null & other.tag == "Player")
            Buff(other.gameObject);
    }

    
    private void Buff(GameObject player)
    {
        Debug.LogWarning(" “Œ ¬€«€¬¿À Ã≈“Œƒ?:????????????" + player.GetComponent<NetworkIdentity>().connectionToClient);
        _owner = player.GetComponent<PlayerData>();
        _owner.ChangeMoveSpeed(5);

        _owner.BuffGive++;
        var item = Instantiate(_imageItem, _owner.ItemsGrind);
        item.GetComponent<DefaultItemMoveSpeedUI>().RegisterOwner(_owner);
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
