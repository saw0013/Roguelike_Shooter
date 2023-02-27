using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mirror;
using MirrorBasics;
using UnityEngine;

[Serializable]
public class ManagerWave : NetworkBehaviour
{
    [SerializeField] private int _allWawe;

    public SyncList<GameObject> _enemyLive = new SyncList<GameObject>();

    [SerializeField] private int[] EnemyToWave;

    [SerializeField] private int _firstIndexDoorClose;

    public int currentWave;
    private int countSpawned;
    public bool isActive;

    bool isStartedSpawn = false; //Заглушка

    bool isStartedTimer = false; //Заглушка

    bool isStartedAddEnemy = false; //Заглушка

    private Dictionary<int, int> EnemyToCurrentWave = new Dictionary<int, int>();

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
            //Debug.LogWarning($"currentWave={currentWave} | _allWawe={_allWawe}");

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
                    timer.timer = 30;
                    timer.running = true;
                    isStartedTimer = true;
                });

                isStartedSpawn = true;
            }
            else
            {
                GetComponent<EventTrigger>().RpcChangeMusic("Ambience");

                ActiveAhtorityDoors();
                isActive = false;
                if (!MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveNextManagerWave())
                {
                    Debug.LogWarning("MATCHID" + GetComponent<NetworkMatch>().matchId);
                    MatchMaker.instance.matches.FirstOrDefault(m => m.matchID.ToGuid() == GetComponent<NetworkMatch>().matchId).players.ForEach(_player =>
                    {
                        TargetPlayerShowStat(_player.GetComponent<NetworkIdentity>().connectionToClient, _player.GetComponent<PlayerData>());
                    });
                }

                MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).players.ForEach(p =>
                {
                    var player = p.GetComponent<PlayerData>();
                    player.ChangeWaveNuberText("");
                });
            }
        }
    }

    [TargetRpc]
    void TargetPlayerShowStat(NetworkConnection conn, PlayerData pdata)
    {
        pdata.ShowStat();
    }

    private void NextSpawnEnemy()
    {
        countSpawned = 1;
        if (isStartedSpawn)
        {
            isStartedSpawn = false;
            GetComponent<EventTrigger>().ServerSpawn(GetComponent<NetworkMatch>().matchId);
           // Debug.LogWarning("SpawnedBegin");
        }
    }

    public void EndOfTimer()
    {
        NetworkConnection ownerConnection = null;

        MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).players.ForEach(p => {
            p.GetComponent<GameTimer>().showTime = "";

            if(ownerConnection == null)
                ownerConnection = p.GetComponent<NetworkIdentity>().connectionToClient;
            });

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
           // Debug.LogWarning("Открываем дверь");
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
        //Debug.LogWarning("Враг " + Enemy);
        _enemyLive.Add(Enemy);
        //Debug.LogWarning("ListEnemyCount " + _enemyLive.Count);
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
        _enemyLive.Remove(enemy);
       // Debug.LogWarning(_enemyLive.Count);
    }
}