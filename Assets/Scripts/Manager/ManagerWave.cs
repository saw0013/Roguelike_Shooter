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

    [SerializeField] private int[] EnemyToWave;

    [SerializeField] private float TimeDelayToWave;

    [SerializeField] private int _firstIndexDoorClose;

    public int currentWave;
    private int countSpawned;
    private float currentTimeDalay;
    public bool isActive;

    [SyncVar] private int needEnemyKiled;

    bool isStarted = false; //Заглушка

    [SyncVar(hook = nameof(OnChangeWave))] private int killedEnemy;

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
        //Выполняется на клиента??? Если выполнится условие с клиента то будет ошибка и код не пойдёт дальше.
        //GetEnemySpawn заменить попробовать на локальную переменную
        // Debug.LogWarning($"_New=={_New} ||| GetEnemySpawn {GetEnemySpawn()}");
        // if (_New == GetEnemySpawn() & !isStarted)

        Debug.LogWarning("_New " + _New + ", needKilledEnemy " + needEnemyKiled);

        if (!isStarted && _New >= needEnemyKiled)
        {
            CmdNextWave();
        }
    }

    private void NextSpawnEnemy()
    {
        countSpawned = 0;
        killedEnemy = 0;
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
    public int SetEnemySpawned() => countSpawned++;

    [Server]
    public void SetKilledEnemy()
    {
        Debug.LogWarning("Убито"  + killedEnemy);
        Debug.LogWarning("SetKilledEnemy отработал");
        killedEnemy++;

        if(needEnemyKiled != GetEnemySpawn()) needEnemyKiled = GetEnemySpawn() * MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult;

        Debug.LogWarning(needEnemyKiled);

    }
}