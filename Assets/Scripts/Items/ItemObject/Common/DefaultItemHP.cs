using Mirror;
using UnityEngine;

public class DefaultItemHP : MonoBehaviour
{
    [Header("OptionBuff")]
    [SerializeField] private float _healthAdd;

    [SerializeField] private GameObject _imageItem;

    private PlayerData _owner;

 //   private bool onceGive; //я не полн€л зачем это тут...

    internal NetworkMatch networkMatch;


    private void OnTriggerEnter(Collider other)
    {
        if(other != null & other.tag == "Player")
            Buff(other.gameObject);
    }

    
    private void Buff(GameObject player)
    {

        //if (onceGive)
        //{
        //    onceGive = true;
            Debug.LogWarning("»грок подобрал " + player.name);

            _owner = player.GetComponent<PlayerData>();

            if (_owner._playerBuffController.PickOrDestroy()) //PickOrDestroy помечен как Summary
            {
                _owner.BuffHealth(_healthAdd, _imageItem);
                _owner.BuffGive++;
            }

            Destroy(gameObject);
        //}
    }
}
