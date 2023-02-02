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

    public float currentWave;
    private int countSpawned;
    private float currentTimeDalay;

    public bool isActive;
    bool isStarted = false;//Заглушка
    private float killedEnemy;

    [SyncVar(hook = nameof(OnChangeWave))]
    private int localKilled;

    private int localWave;


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
        Debug.LogWarning($"LocalKiled={_New} >= EnemyToWave[currentWave]={GetEnemySpawn()}");
        if (_New >= GetEnemySpawn() & !isStarted)
        {
            //if (isServer) NextWave();
            //else CmdNextWave();
            //if (isServer) NextWave();
            //else CmdNextWave();
            CmdNextWave();
        }
    }

    private void NextSpawnEnemy()
    {
        countSpawned = 0;
        killedEnemy = 0;
        GetComponent<EventTrigger>().ServerSpawn(GetComponent<NetworkMatch>().matchId);
    }

    /// <summary>
    /// ��������� �����
    /// </summary>

    private void NextWave(GameObject player)
    {
        isStarted = true;
        //Debug.LogWarning($"�� �� ��\r\nisServer={isServer} | isClient={isClient} | isLocalPlayer={isLocalPlayer} | hasAuthority={hasAuthority} | NetworkClient.active={NetworkClient.active}");
        //currentWave++;
        //currentTimeDalay = 0;
        //if (currentWave >= _allWawe)
        //{
        //    isActive = false;
        //    MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveNextManagerWave();
        //    ActiveAhtorityDoors();
        //}
        //else NextSpawnEnemy();

        #region  Спавн таймера

        //var _player = player.GetComponent<PlayerMovementAndLookNetwork>();
        //var parentCanvasPlayer = _player.transform.Find("CanvasPlayer");
        //if (_player.transform.Find("GameTimer(Clone)")) return;
        //else
        //{
            ////TODO :  [Client]

            ////GameObject timer = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.Find(prefab => prefab.name == "GameTimer"), parentCanvasPlayer);
            //var timer = Instantiate(Resources.LoadAsync("Prefabs/PlayerCommon/GameTimer").asset as GameObject, parentCanvasPlayer);
            //GameTimer gameTimer = timer.GetComponent<GameTimer>();
            //if (gameTimer)
                //gameTimer.ClockReady.AddListener(EndOfTimer);

            //#region  Разобрать

        //}
        //var _player = player.GetComponent<PlayerMovementAndLookNetwork>();
        //var parentCanvasPlayer = _player.transform.Find("CanvasPlayer");
        //if (_player.transform.Find("GameTimer(Clone)")) return;
        //else
        //{
        //    //TODO :  [Client]
        //    Debug.LogWarning("�� � ����� � ����� �������� �������");
        //    //GameObject timer = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.Find(prefab => prefab.name == "GameTimer"), parentCanvasPlayer);
        //    var timer = Instantiate(Resources.LoadAsync("Prefabs/PlayerCommon/GameTimer").asset as GameObject, parentCanvasPlayer);
        //    GameTimer gameTimer = timer.GetComponent<GameTimer>();
        //    if (gameTimer)
        //        gameTimer.ClockReady.AddListener(EndOfTimer);

        //}

        #region Через MatchMaker
        //foreach (var _player in MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).players)
        //{
        //    var parentCanvasPlayer = _player.transform.Find("CanvasPlayer");

        //    }
        //}
        #endregion

        #endregion

    }


    [Command(requiresAuthority = false)]
    public void CmdNextWave(NetworkConnectionToClient sender = null) //NetworkConnectionToClient смотреть https://mirror-networking.gitbook.io/docs/guides/communications/remote-actions
    {
        Debug.LogWarning("Отправлено с  " + sender.identity.name);
        //NextWave(sender.identity.gameObject);

        sender.identity.gameObject.GetComponent<PlayerData>().ChangeWaveNuberText(true, (int)currentWave);

        var timer = sender.identity.gameObject.GetComponent<GameTimer>();

        timer.timer = 1 * 60;

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
        currentWave += 1 / MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult;

        if (currentWave > localWave) localWave++;

        Debug.LogWarning($"currentWave={currentWave} | _allWawe={_allWawe}");
        if (currentWave >= _allWawe)
        {
            isActive = false;
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveNextManagerWave();
            ActiveAhtorityDoors();
        }

        isStarted = false;
    }

    public void DeactiveAhtorityDoors()
    {
        for (int i = 0; i < 2; i++)
        {
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Door[_firstIndexDoorClose + i].hasAthorityTrigger = false;
        }
    }

    public void ActiveAhtorityDoors()
    {
        for (int i = 0; i < 2; i++)
        {
            Debug.LogWarning("Открываем дверь");
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Door[_firstIndexDoorClose + i].hasAthorityTrigger = true;
        }
    }

    /// <summary>
    /// �������� ���������� ��� � ������� �����
    /// </summary>
    /// <returns></returns>
    public int GetEnemySpawn() => EnemyToCurrentWave[localWave] * MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult;

    /// <summary>
    /// ������� ����� ���������� ���
    /// </summary>
    /// <returns></returns>
    public int GetEnemySpawned() => countSpawned;

    public int SetEnemySpawned() => countSpawned++;

    [Command(requiresAuthority = false)]
    public void CmdSetKilledEnemy() => SetKilledEnemy();

    [Server]
    public void ServerKilledEnemy()
    {
        Debug.LogWarning("SetKilledEnemy отработал");

        killedEnemy++;

        if (killedEnemy / MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult > localKilled) localKilled++;
    }


    public void SetKilledEnemy()
    {
        if (isServer) ServerKilledEnemy();
        else CmdSetKilledEnemy();
    }

}
