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

    [SyncVar(hook = nameof(OnChangeWave))] public SyncList<GameObject> _enemyLive = new SyncList<GameObject>();

    [SerializeField] private int[] EnemyToWave;

    [SerializeField] private float TimeDelayToWave;

    [SerializeField] private int _firstIndexDoorClose;

    public int currentWave;
    private int countSpawned;
    private float currentTimeDalay;
    public bool isActive;

    [SyncVar] private int needEnemyKiled;

    bool isStarted = false; //Заглушка

    //[SyncVar(hook = nameof(OnChangeWave))] private int killedEnemy;

    private Dictionary<int, int> EnemyToCurrentWave = new Dictionary<int, int>();

    private void Start()
    {
        for (int i = 0; i < _allWawe; i++)
        {
            EnemyToCurrentWave.Add(i, EnemyToWave[i]);
        }
    }

    private void OnChangeWave(SyncList<GameObject> _Old, SyncList<GameObject> _New)
    {
        //Выполняется на клиента??? Если выполнится условие с клиента то будет ошибка и код не пойдёт дальше.
        //GetEnemySpawn заменить попробовать на локальную переменную
        // Debug.LogWarning($"_New=={_New} ||| GetEnemySpawn {GetEnemySpawn()}");
        // if (_New == GetEnemySpawn() & !isStarted)

        //Debug.LogWarning("_New " + _New + ", needKilledEnemy " + needEnemyKiled);

        //if (!isStarted && _New >= needEnemyKiled)
        //{
        //    CmdNextWave();
        //}

        Debug.LogWarning("_oldList " + _Old.Count + ", _newList " + _New.Count);


        if(_New == null) CmdNextWave();

    }

    private void NextSpawnEnemy()
    {
        countSpawned = 0;
        //killedEnemy = 0;
        GetComponent<EventTrigger>().ServerSpawn(GetComponent<NetworkMatch>().matchId);
    }

    private void NextWave(GameObject player)
    {
        isStarted = true;
    }

    [Command(requiresAuthority = false)]
    public void CmdNextWave(NetworkConnectionToClient sender =null) //NetworkConnectionToClient смотреть https://mirror-networking.gitbook.io/docs/guides/communications/remote-actions                             
    {
        Debug.LogWarning("Отправлено с  " + sender.identity.name);
        //NextWave(sender.identity.gameObject);

        var timer = sender.identity.gameObject.GetComponent<GameTimer>();

        if (!isStarted)
        {
            if (timer)
                timer.ClockReady
                    .AddListener(EndOfTimer); //запуск Event таймера

            isStarted = true; //
        }

        timer.running = true;
        //NextWave(sender.identity.gameObject);
    }

    public void EndOfTimer()
    {
        Debug.Log("timer ready.");
        currentWave++;

        Debug.LogWarning($"currentWave={currentWave} | _allWawe={_allWawe}");

        if (currentWave >= _allWawe)
        {
            isActive = false;
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveNextManagerWave();
            ActiveAhtorityDoors();
        }

    }

    public void DeactiveAhtorityDoors()
    {
        for (int i = 0; i < 2; i++)
        {
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Door[_firstIndexDoorClose + i]
                .hasAthorityTrigger = false;
        }
    }

    public void ActiveAhtorityDoors()
    {
        for (int i = 0; i < 2; i++)
        {
            Debug.LogWarning("Открываем дверь");
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Door[_firstIndexDoorClose + i]
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
        isStarted = true;
    }
    public void SetEnemySpawned()
    {
        countSpawned++;
    }

    [Server]
    public void SetKilledEnemy()
    {
        //Debug.LogWarning("Убито" + killedEnemy);
        //Debug.LogWarning("SetKilledEnemy отработал");
        //killedEnemy++;

        //if (needEnemyKiled != GetEnemySpawn()) needEnemyKiled = GetEnemySpawn() * MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult;

        //Debug.LogWarning(needEnemyKiled);

        for(int i = 0; i < _enemyLive.Count; i++)
        {
            Debug.LogWarning("Enemy " + i + ", Check 1 " + _enemyLive[i]);
            if (_enemyLive[i] == null) _enemyLive.Remove(_enemyLive[i]);
            Debug.LogWarning("Enemy " + i + ", Check 2 " + _enemyLive[i]);
        }

        Debug.LogWarning(_enemyLive.Count);

    }
}