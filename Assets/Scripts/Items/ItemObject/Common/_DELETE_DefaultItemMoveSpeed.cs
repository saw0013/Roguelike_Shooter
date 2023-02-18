using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _DELETE_DefaultItemMoveSpeed : MonoBehaviour
{
    [Header("OptionBuff")]
    [SerializeField] private int _speedAdd;

    [SerializeField] private GameObject _imageItem;

    private PlayerData _owner;

    private bool onceGive;

    internal NetworkMatch networkMatch;

    private void OnTriggerEnter(Collider other)
    {
        if (other != null & other.tag == "Player")
            Buff(other.gameObject);
    }

    //наъгюрекэмн!!!! опнярн наъгюрекэмн с пндхрекэяйнцн йнлонмемрю йнрнпши яеребни ядекюрэ рнфе яйпхор х рпхцеп врнаш нм хявегюк опх опнунде
    private void Buff(GameObject player)
    {
        if (onceGive)
        {
            onceGive = true;
            Debug.LogWarning("хЦПНЙ ОНДНАПЮК " + player.name);
            _owner = player.GetComponent<PlayerData>();
            _owner.ChangeMoveSpeed(_speedAdd, _imageItem);
            _owner.BuffGive++;

            //var item = Instantiate(_imageItem, _owner.ItemsGrind);
            //item.GetComponent<DefaultItemMoveSpeedUI>().RegisterOwner(_owner);
            //NetworkServer.Destroy(gameObject);
            Destroy(gameObject);
        }
    }


}
