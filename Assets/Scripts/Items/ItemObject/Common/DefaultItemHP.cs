using Mirror;
using UnityEngine;

public class DefaultItemHP : MonoBehaviour
{
    [Header("OptionBuff")]
    [SerializeField] private float _healthAdd;

    [SerializeField] private GameObject _imageItem;

    private PlayerData _owner;

    internal NetworkMatch networkMatch;


    private void OnTriggerEnter(Collider other)
    {
        if(other != null & other.tag == "Player")
            Buff(other.gameObject);
    }

    
    private void Buff(GameObject player)
    {
        _owner = player.GetComponent<PlayerData>();
        _owner.BuffHealth(_healthAdd, _imageItem);
        _owner.BuffGive++;

       // var item = Instantiate(_imageItem, _owner.ItemsGrind);
       // item.GetComponent<DefaultItemHPUI>().RegisterOwner(_owner);
        Destroy(gameObject);
        //NetworkServer.Destroy(gameObject);
    }
}
