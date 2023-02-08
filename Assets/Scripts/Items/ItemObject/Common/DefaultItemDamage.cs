using Mirror;
using UnityEngine;

public class DefaultItemDamage : MonoBehaviour
{
    [Header("OptionBuff")]
    [SerializeField] private int _damageAdd;

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
        _owner.ChangeDamage(_damageAdd, _imageItem);
        _owner.BuffGive++;

        Destroy(gameObject);
    }
}
