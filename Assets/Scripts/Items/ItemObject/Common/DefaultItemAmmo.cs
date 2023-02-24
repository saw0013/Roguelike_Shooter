using UnityEngine;
using Mirror;

public class DefaultItemAmmo : MonoBehaviour
{
    [Header("OptionBuff")]

    [SerializeField] private float _ammoReload;
    [SerializeField] private float _ammoRate;

    [SerializeField] private GameObject _imageItem;

    private PlayerData _owner;

    private bool onceGive;

    internal NetworkMatch networkMatch;

    private void OnTriggerEnter(Collider other)
    {
        if (other != null & other.tag == "Player")
            Buff(other.gameObject);
    }

   
    private void Buff(GameObject player)
    {
        _owner = player.GetComponent<PlayerData>();

        if (_owner._playerBuffController.PickOrDestroy())
        {
            _owner.ChangeAmmo(_ammoReload, _ammoRate, _imageItem);
            _owner.BuffGive++;
        }

        Destroy(gameObject);
    }
}
