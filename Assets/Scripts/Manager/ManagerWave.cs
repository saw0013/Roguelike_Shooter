using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mirror;
using MirrorBasics;
using UnityEngine;

[Serializable]
public class ManagerWave : NetworkBehaviour
{
    [SerializeField] private int _allWawe;

    /*[SyncVar(hook = nameof(OnChangeWave))]*/ public SyncList<GameObject> _enemyLive = new SyncList<GameObject>();

    [SerializeField] private int[] EnemyToWave;

    [SerializeField] private float TimeDelayToWave;

    [SerializeField] private int _firstIndexDoorClose;

    public int currentWave;
    private int countSpawned;
    private float currentTimeDalay;
    public bool isActive;

    [SyncVar] private int needEnemyKiled;

    bool isStartedSpawn = false; //Заглушка

    bool isStartedTimer = false; //Заглушка

    bool isStartedAddEnemy = false; //Заглушка

    //[SyncVar(hook = nameof(OnChangeWave))] private int killedEnemy;

    private Dictionary<int, int> EnemyToCurrentWave = new Dictionary<int, int>();

    private int localSetWave;

    private void Start()
    {
        for (int i = 0; i < _allWawe; i++)
        {
            EnemyToCurrentWave.Add(i, EnemyToWave[i]);
        }
    }

    private void Update()
    {
        if (isActive & isStartedAddEnemy)
        {
            if (_enemyLive.Count == 0)
            {
                isStartedAddEnemy = false;
                NextWave();
            }
        }
    }

    [Server]
    private void NextWave()
    {
        if (!isStartedTimer)
        {
            currentWave++;
            Debug.LogWarning($"currentWave={currentWave} | _allWawe={_allWawe}");

            if (currentWave < _allWawe)
            {
                MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).players.ForEach(p =>
                {
                    Debug.LogWarning("Player " + p.name);
                    var player = p.GetComponent<PlayerData>();
                    var Wave = currentWave;
                    Debug.LogWarning(Wave);
                    Wave++;
                    player.ChangeWaveNuberText("волна " + Wave + "/" + _allWawe);
                    var timer = p.gameObject.GetComponent<GameTimer>();
                    timer.timer = 60;
                    timer.running = true;
                    isStartedTimer = true;
                });

                isStartedSpawn = true;
            }
            else
            {
                ActiveAhtorityDoors();
                isActive = false;
                MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveNextManagerWave();
                MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).players.ForEach(p =>
                {
                    var player = p.GetComponent<PlayerData>();
                    player.ChangeWaveNuberText("");
                });
            }
        }
    }

    //private void OnChangeWave(SyncList<GameObject> _Old, SyncList<GameObject> _New)
    //{
    //    //Выполняется на клиента??? Если выполнится условие с клиента то будет ошибка и код не пойдёт дальше.
    //    //GetEnemySpawn заменить попробовать на локальную переменную
    //    // Debug.LogWarning($"_New=={_New} ||| GetEnemySpawn {GetEnemySpawn()}");

    //    Debug.LogWarning("Прооизошло изменение");

    //    Debug.LogWarning("_oldList " + _Old.Count + ", _newList " + _New.Count);

    //}

    private void NextSpawnEnemy()
    {
        //killedEnemy = 0;
        countSpawned = 1;
        if (isStartedSpawn)
        {
            isStartedSpawn = false;
            GetComponent<EventTrigger>().ServerSpawn(GetComponent<NetworkMatch>().matchId);
            Debug.LogWarning("SpawnedBegin");
        }
    }

    public void EndOfTimer()
    {
        Debug.Log("timer ready.");

        //currentWave = localSetWave;

        //Debug.LogWarning($"currentWave={currentWave} | _allWawe={_allWawe}");

        //if (currentWave >= _allWawe)
        //{
        //    isActive = false;
        //    MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveNextManagerWave();
        //    MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).players.ForEach(p =>
        //    {
        //        var player = p.GetComponent<PlayerData>();
        //        player.ChangeWaveNuberText("");
        //    });
        //    ActiveAhtorityDoors();
        //}
        //else
        //{
        //    NextSpawnEnemy();
        //}

        NextSpawnEnemy();

        isStartedTimer = false;
    }

    public void DeactiveAhtorityDoors()
    {
        for (int i = 0; i < 2; i++)
        {
            var index = _firstIndexDoorClose + i;
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Door[index]
                .hasAthorityTrigger = false;
        }
    }

    public void ActiveAhtorityDoors()
    {
        for (int i = 0; i < 2; i++)
        {
            Debug.LogWarning("Открываем дверь");
            var index = _firstIndexDoorClose + i;
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Door[index]
                .hasAthorityTrigger = true;
        }
    }

    //TODO : Это выполняется на клиенте или выше в хуке
    public int GetEnemySpawn() => EnemyToCurrentWave[currentWave] * MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult;

    public int GetEnemySpawned() => countSpawned;

    public void AddInListEnemy(GameObject Enemy)
    {
        Debug.LogWarning("Враг " + Enemy);
        _enemyLive.Add(Enemy);
        Debug.LogWarning("ListEnemyCount " + _enemyLive.Count);
        isStartedAddEnemy = true;
    }
    public void SetEnemySpawned()
    {
        countSpawned++;
    }

    public int GetAllWave() => _allWawe;

    [Server]
    public void SetKilledEnemy(GameObject enemy)
    {
        //Debug.LogWarning("Убито" + killedEnemy);
        //Debug.LogWarning("SetKilledEnemy отработал");
        //killedEnemy++;

        //if (needEnemyKiled != GetEnemySpawn()) needEnemyKiled = GetEnemySpawn() * MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult;

        //Debug.LogWarning(needEnemyKiled);

        //for(int i = 0; i < _enemyLive.Count; i++)
        //{
        //    Debug.LogWarning("Enemy " + i + ", Check 1 " + _enemyLive[i]);
        //    if (_enemyLive[i] == null) _enemyLive.Remove(_enemyLive[i]);
        //    Debug.LogWarning("Enemy " + i + ", Check 2 " + _enemyLive[i]);
        //}

        //Debug.LogWarning(_enemyLive.Count);

        //isStartedSpawn = true;

        _enemyLive.Remove(enemy);
        Debug.LogWarning(_enemyLive.Count);

    }
}