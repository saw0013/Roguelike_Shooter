using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _DELETE_DefaultItemMoveSpeed : MonoBehaviour
{
    [SerializeField] private GameObject _imageItem;

    private PlayerData _owner;

    internal NetworkMatch networkMatch;

    private void OnTriggerEnter(Collider other)
    {
        if (other != null & other.tag == "Player")
            Buff(other.gameObject);
    }

    //наъгюрекэмн!!!! опнярн наъгюрекэмн с пндхрекэяйнцн йнлонмемрю йнрнпши яеребни ядекюрэ рнфе яйпхор х рпхцеп врнаш нм хявегюк опх опнунде
    private void Buff(GameObject player)
    {
        _owner = player.GetComponent<PlayerData>();
        _owner.ChangeMoveSpeed(5, _imageItem);
        _owner.BuffGive++;

        //var item = Instantiate(_imageItem, _owner.ItemsGrind);
        //item.GetComponent<DefaultItemMoveSpeedUI>().RegisterOwner(_owner);
        //NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }


}
