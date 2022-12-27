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

    [Tooltip("Анимация открытия коробки")]
    [SerializeField] private float TimeAnimOpen;

    [SerializeField] private Transform[] _spawnPosition; 

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(GiveBuffsEnum());
    }

    private IEnumerator GiveBuffsEnum()
    {
        yield return new WaitForSeconds(TimeAnimOpen);

        GiveBuff();
    }

    private void GiveBuff()
    {
        Debug.LogWarning(" зашел GiveBuff");
        for(int i = 0; i < MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).players.Count; i++)
        {
            var Chance = UnityEngine.Random.Range(1, 101);

            Debug.LogWarning(Chance);

            animator.SetInteger("BuffDrop", i + 1);

            if (Chance <= (100 - CommonChance))
            {
                SpawnBuff(CommonBuff, i);
            }
            else
            {
                if (Chance <= (100 - RareChance))
                {
                    SpawnBuff(RareBuff, i);
                }
                else
                {
                    if (Chance <= (100 - EpicChance))
                    {
                        //SpawnBuff(EpicBuff, i);
                        SpawnBuff(RareBuff, i);
                        Debug.LogWarning("Epic");
                    }
                    else
                    {
                        if (Chance <= (100 - LegenderyChance))
                        {
                            //SpawnBuff(LegenderyBuff, i);
                            SpawnBuff(RareBuff, i);
                            Debug.LogWarning("Legendery");
                        }
                    }
                }
            }
        }
    }
    private void SpawnBuff(List<GameObject> Buffs, int index)
    {
        //CmdSpawnBuff(Buffs, index);
        int indexBuffs = UnityEngine.Random.Range(0, Buffs.Count);
        Debug.LogWarning("Индекс" + indexBuffs);
        var buff = Instantiate(Buffs[indexBuffs], _spawnPosition[index]);
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

