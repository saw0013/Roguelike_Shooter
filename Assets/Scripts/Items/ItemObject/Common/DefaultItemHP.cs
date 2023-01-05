using Mirror;
using UnityEngine;

public class DefaultItemHP : NetworkBehaviour
{
    [SerializeField] private GameObject _imageItem;

    private PlayerData _owner;

    private void OnTriggerEnter(Collider other)
    {
        if(other != null & other.tag == "Player") 
            Buff(other.gameObject);
    }

    [TargetRpc]
    private void Buff(GameObject player)
    {
        _owner = player.GetComponent<PlayerData>();
        _owner.BuffHealth(150f);
        _owner.BuffGive++;
        var item = Instantiate(_imageItem, _owner.ItemsGrind);
        item.GetComponent<DefaultItemHPUI>().RegisterOwner(_owner);
        Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }
}
