using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefaultItemGuard : NetworkBehaviour
{
    //[SerializeField] private TMP_Text _textTime;

    //[SerializeField] private float _timeBuff;

    private PlayerData _owner;

   // private bool buffStart;

    private void Update()
    {

        //if (buffStart && _timeBuff <= 0)
        //{
        //    _timeBuff = Time.deltaTime;
        //    Mathf.RoundToInt(_timeBuff);
        //    _textTime.text = $"Ùèò: {_timeBuff}";
        //} 
        //else BuffStop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null & other.tag == "Player")
            Buff(other.gameObject);
    }

    private void Buff(GameObject player)
    {
        _owner = player.GetComponent<PlayerData>();
        _owner.ChangeGuard(50);
        Destroy(gameObject);
    }

    private void BuffStop()
    {
        //buffStart = false;
        _owner.StopBuffGuard();
    }
}
