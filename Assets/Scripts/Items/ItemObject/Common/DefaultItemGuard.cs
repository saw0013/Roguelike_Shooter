using Mirror;
using UnityEngine;

public class DefaultItemGuard : MonoBehaviour
{
    [Header("OptionBuff")]
    [SerializeField] private int _guardAdd;

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

        if (_owner._playerBuffController.PickOrDestroy()) //PickOrDestroy помечен как Summary
        {
            _owner.ChangeGuard(_guardAdd, _imageItem);
            _owner.BuffGive++;
        }

        Destroy(gameObject);
    }
}
