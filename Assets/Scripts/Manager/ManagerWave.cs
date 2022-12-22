using System;
using System.Collections;
using System.Collections.Generic;
using Mirror; 
using UnityEngine;

public class ManagerWave : NetworkBehaviour
{
    [SerializeField] private int _allWawe;

    [SerializeField] private int[] EnemyToWave;

    [SerializeField] private float TimeDelayToWave;

    public int currentWave;
    private int countSpawned;
    private float currentTimeDalay;

    public Guid matchId;

    [SyncVar(hook = nameof(OnChangeWave))]
    private int killedEnemy;

    [Tooltip(" оличество врагов, по значению волны")]
    private Dictionary<int, int> EnemyToCurrentWave = new Dictionary<int, int>();

    private void Start()
    {
        for (int i = 0; i < _allWawe; i++)
        {
            EnemyToCurrentWave.Add(i, EnemyToWave[i]);    
        }
    }

    private void OnChangeWave(int _Old, int _New)
    {
        if(killedEnemy >= EnemyToWave[currentWave])
        {
            NextWave();
        }
    }

    private void NextSpawnEnemy()
    {
        countSpawned = 0;
        killedEnemy = 0;
        GetComponent<EventTrigger>().ServerSpawn(matchId);
    }

    private void NextWave()
    {
        if(currentTimeDalay >= TimeDelayToWave)
        {
            currentWave++;
            currentTimeDalay = 0;
            NextSpawnEnemy();
        }
        else currentTimeDalay += Time.deltaTime;
    }

    public int GetEnemySpawn() => EnemyToCurrentWave[currentWave];

    public int GetEnemySpawned() => countSpawned;

    public int SetEnemySpawned() => countSpawned++;

    public int SetKilledEnemy() => killedEnemy++;
}
