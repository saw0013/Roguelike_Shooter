using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MirrorBasics;
using System;

public class ManagerGiveBuff : NetworkBehaviour
{
    [Header("Buff's")]
    [SerializeField] private List<GameObject> CommonBuff;
    [SerializeField] private List<GameObject> RareBuff;
    [SerializeField] private List<GameObject> EpicBuff;
    [SerializeField] private List<GameObject> LegenderyBuff;

    [Header("ChanceBuff")]
    [SerializeField] private int CommonChance;
    [SerializeField] private int RareChance;
    [SerializeField] private int EpicChance;
    [SerializeField] private int LegenderyChance;
    //TODO : Сделать рандом на выпадение 20%

    public void SpawnBuff()
    {
        CmdSpawnBuff();
    }

    [Command(requiresAuthority = false)]
    private void CmdSpawnBuff(NetworkConnectionToClient sender = null)
    {
        var chanceSpawnBuff = UnityEngine.Random.Range(0f, 1f);

        if (chanceSpawnBuff > 0.2f) return;

        var Chance = UnityEngine.Random.Range(1, 101);

        //Debug.LogWarning(Chance);

        if (Chance <= (100 - CommonChance))
        {
            _spawnBuff_(CommonBuff);
        }
        else
        {
            if (Chance <= (100 - RareChance))
            {
                _spawnBuff_(RareBuff);
            }
            else
            {
                if (Chance <= (100 - EpicChance))
                {
                    _spawnBuff_(RareBuff);
                }
                else
                {
                    if (Chance <= (100 - LegenderyChance))
                    {
                        _spawnBuff_(RareBuff);
                    }
                }
            }
        }
    }

    private void _spawnBuff_(List<GameObject> Buffs)
    {
        int indexBuffs = UnityEngine.Random.Range(0, Buffs.Count);
        var positionSpawn = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
        var buff = Instantiate(Buffs[indexBuffs], positionSpawn, Quaternion.identity);
        buff.GetComponent<NetworkMatch>().matchId = GetComponent<NetworkMatch>().matchId;
        NetworkServer.Spawn(buff);
    }

}

