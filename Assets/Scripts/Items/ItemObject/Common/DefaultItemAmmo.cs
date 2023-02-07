using UnityEngine;
using Mirror;

public class DefaultItemAmmo : NetworkBehaviour
{
    [SerializeField] private GameObject _imageItem;

    private PlayerData _owner;

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
        _owner.ChangeAmmo(1f, 1000);
        _owner.BuffGive++;
        var item = Instantiate(_imageItem, _owner.ItemsGrind);
        item.GetComponent<DefaultItemAmmoUI>().RegisterOwner(_owner);
        Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }
}
