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

    //[Tooltip("Анимация открытия коробки")]
    //[SerializeField] private float TimeAnimOpen;

    public void SpawnBuff()
    {
        CmdSpawnBuff();
    }

    [Command(requiresAuthority = false)]
    private void CmdSpawnBuff(NetworkConnectionToClient sender = null)
    {
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
                    //SpawnBuff(EpicBuff, i);
                    _spawnBuff_(RareBuff);
                }
                else
                {
                    if (Chance <= (100 - LegenderyChance))
                    {
                        //SpawnBuff(LegenderyBuff, i);
                        _spawnBuff_(RareBuff);

                    }
                }
            }
        }
    }

    private void _spawnBuff_(List<GameObject> Buffs)
    {
        //CmdSpawnBuff(Buffs, index);
        //Debug.LogWarning("Индекс" + indexBuffs);

        int indexBuffs = UnityEngine.Random.Range(0, Buffs.Count);
        var positionSpawn = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
        var buff = Instantiate(Buffs[indexBuffs], positionSpawn, Quaternion.identity);
        buff.GetComponent<NetworkMatch>().matchId = GetComponent<NetworkMatch>().matchId;
        NetworkServer.Spawn(buff);
    }

    //[Command]
    //public void CmdSpawnBuff(List<GameObject> Buffs, int index)
    //{
    //    int indexBuffs = UnityEngine.Random.Range(0, Buffs.Count);
    //    Debug.LogWarning("Индекс" + indexBuffs);
    //    var buff = Instantiate(Buffs[indexBuffs], _spawnPosition[index]);
    //    buff.GetComponent<NetworkMatch>().matchId = GetComponent<NetworkMatch>().matchId;
    //    NetworkServer.Spawn(buff);
    //}

}

