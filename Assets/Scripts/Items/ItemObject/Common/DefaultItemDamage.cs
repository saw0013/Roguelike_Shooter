using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DefaultItemDamage : NetworkBehaviour
{
    [SerializeField] private GameObject _imageItem;
    [SerializeField] private UnityEvent _event;

    private PlayerData _owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other != null & other.tag == "Player")
            Buff(other.gameObject);
    }

    private void Buff(GameObject player)
    {
        _owner = player.GetComponent<PlayerData>();
        _owner.ChangeDamage(50);
        var item = Instantiate(_imageItem, _owner.ItemsGrind);
        item.GetComponent<DefaultItemMoveSpeedUI>().RegisterOwner(_owner);
        _event.Invoke();
    }
}
