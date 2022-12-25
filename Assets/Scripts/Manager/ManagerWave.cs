using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using MirrorBasics;
using UnityEngine;

public class ManagerWave : NetworkBehaviour
{
    [SerializeField] private int _allWawe;

    [SerializeField] private int[] EnemyToWave;

    [SerializeField] private float TimeDelayToWave;

    [Tooltip("Первый индекс, который будет закрты первый")]
    [SerializeField] private int _firstIndexDoorClose;

    public int currentWave;
    private int countSpawned;
    private float currentTimeDalay;

    public bool isActive;

    [SyncVar(hook = nameof(OnChangeWave))]
    private int killedEnemy;

    [Tooltip("Количество врагов, по значению волны")]
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
        GetComponent<EventTrigger>().ServerSpawn(GetComponent<NetworkMatch>().matchId);
    }

    /// <summary>
    /// Следующая волна
    /// </summary>
    private void NextWave()
    {
        #region
        foreach(var _player in MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).players)
        {
            var parentCanvasPlayer = _player.transform.Find("CanvasPlayer");
            GameObject timer = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.Find(prefab => prefab.name == "GameTimer"), parentCanvasPlayer);

            GameTimer gameTimer = timer.GetComponent<GameTimer>();
            if (gameTimer)
                gameTimer.ClockReady.AddListener(EndOfTimer);
        }
        
        #endregion

        if (currentTimeDalay >= TimeDelayToWave)
        {
            currentWave++;
            currentTimeDalay = 0;
            if (currentWave >= _allWawe)
            {
                isActive = false;
                MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveNextManagerWave();
                ActiveAhtorityDoors();
            }
            else NextSpawnEnemy();
        }
        else currentTimeDalay += Time.deltaTime;
    }

    public void EndOfTimer()
    {
        Debug.Log("timer ready.");

        // End of match code here
    }

    public void DeactiveAhtorityDoors()
    { 
        for(int i = 0; i < 2; i++)
        {
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Door[_firstIndexDoorClose + i].hasAthorityTrigger = false;
        }
    }

    public void ActiveAhtorityDoors()
    {
        for (int i = 0; i < 2; i++)
        {
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Door[_firstIndexDoorClose + i].hasAthorityTrigger = true;
        }
    }

    /// <summary>
    /// Полчуает количество НПЦ в текущей волне
    /// </summary>
    /// <returns></returns>
    public int GetEnemySpawn() => EnemyToCurrentWave[currentWave] * MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult;

    /// <summary>
    /// Сколько всего заспавнено НПЦ
    /// </summary>
    /// <returns></returns>
    public int GetEnemySpawned() => countSpawned;

    public int SetEnemySpawned() => countSpawned++;

    public int SetKilledEnemy() => killedEnemy++;
}
