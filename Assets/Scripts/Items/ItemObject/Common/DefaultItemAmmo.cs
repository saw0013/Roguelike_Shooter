using UnityEngine;
using Mirror;

public class DefaultItemAmmo : MonoBehaviour
{
    [Header("OptionBuff")]

    [SerializeField] private float _ammoReload;
    [SerializeField] private int _ammoForce;

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
        _owner.ChangeAmmo(_ammoReload, _ammoForce, _imageItem);
        _owner.BuffGive++;

        Destroy(gameObject);
    }
}
