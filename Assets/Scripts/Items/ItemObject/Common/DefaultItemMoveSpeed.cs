using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultItemMoveSpeed : MonoBehaviour
{
    [SerializeField] private GameObject _imageItem;

    private PlayerData _owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other != null & other.tag == "Player")
            Buff(other.gameObject);
    }

    private void Buff(GameObject player)
    {
        _owner = player.GetComponent<PlayerData>();
        _owner.ChangeMoveSpeed(5);
        var item = Instantiate(_imageItem, _owner.ItemsGrind);
        item.GetComponent<DefaultItemMoveSpeedUI>().RegisterOwner(_owner);
        Destroy(gameObject);
    }
}
