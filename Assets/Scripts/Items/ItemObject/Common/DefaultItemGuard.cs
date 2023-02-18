using Mirror;
using UnityEngine;

public class DefaultItemGuard : MonoBehaviour
{
    [Header("OptionBuff")]
    [SerializeField] private int _guardAdd;

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
        if (onceGive)
        {
            onceGive = true;
            Debug.LogWarning("Игрок подобрал " + player.name);

            _owner = player.GetComponent<PlayerData>();
            _owner.ChangeGuard(_guardAdd, _imageItem);
            _owner.BuffGive++;

            Destroy(gameObject);
        }
    }
}
