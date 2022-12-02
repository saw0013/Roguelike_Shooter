using Mirror;
using UnityEngine;

public class RareItemBullet : NetworkBehaviour
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
        _owner.ChangeBullet(0.5f);
        var item = Instantiate(_imageItem, _owner.ItemsGrind);
        item.GetComponent<RareItemBulletUI>().RegisterOwner(_owner);
        Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }
}
