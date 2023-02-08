using Mirror;
using UnityEngine;
using UnityEngine.Playables;

public class RareItemBullet : MonoBehaviour
{
    [Header("OptionBuff")]
    [SerializeField] private float _sizeAdd;

    [SerializeField] private GameObject _imageItem;

    private PlayerData _owner;

    internal NetworkMatch networkMatch;

    private void OnTriggerEnter(Collider other)
    {
        if (other != null & other.tag == "Player")
          Buff(other.gameObject);
    }

    private void Buff(GameObject player)
    {
        _owner = player.GetComponent<PlayerData>();
        _owner.ChangeBullet(_sizeAdd, _imageItem);
        _owner.BuffGive++;

        Destroy(gameObject);
    }

    
}
