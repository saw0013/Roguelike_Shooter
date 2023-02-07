using Mirror;
using UnityEngine;

public class DefaultItemDamage : NetworkBehaviour
{
    [SerializeField] private GameObject _imageItem;

    private PlayerData _owner;

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other != null & other.tag == "Player")
            Buff(other.gameObject);
    }

    
    private void Buff(GameObject player)
    {
        Debug.LogWarning(" “Œ ¬€«€¬¿À Ã≈“Œƒ?:????????????" + player.GetComponent<NetworkIdentity>().connectionToClient);
        _owner = player.GetComponent<PlayerData>();
        _owner.ChangeDamage(50);
        _owner.BuffGive++;
        var item = Instantiate(_imageItem, _owner.ItemsGrind);
        item.GetComponent<DefaultItemDamageUI>().RegisterOwner(_owner);
        Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }
}
