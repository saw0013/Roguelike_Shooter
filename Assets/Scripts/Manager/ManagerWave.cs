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

    [Tooltip("������ ������, ������� ����� ������ ������")]
    [SerializeField] private int _firstIndexDoorClose;

    public int currentWave;
    private int countSpawned;
    private float currentTimeDalay;

    public bool isActive;
    bool isStarted = false; //��� ���������� � �������, �� ����� ������ ����� ������ ������� ��� ������� �������. ��� ������ ��������

    [SyncVar(hook = nameof(OnChangeWave))]
    private int killedEnemy;

    [Tooltip("���������� ������, �� �������� �����")]
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
        if (killedEnemy >= GetEnemySpawn() & !isStarted)
        {
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


        #region ������� ����� �� ������ ��� �������

        //var _player = player.GetComponent<PlayerMovementAndLookNetwork>();
        //var parentCanvasPlayer = _player.transform.Find("CanvasPlayer");
        //if (_player.transform.Find("GameTimer(Clone)")) return;
        //else
        //{
        //    //TODO : ��������� ������ �� ������� [Client]
        //    Debug.LogWarning("�� � ����� � ����� �������� �������");
        //    //GameObject timer = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.Find(prefab => prefab.name == "GameTimer"), parentCanvasPlayer);
        //    var timer = Instantiate(Resources.LoadAsync("Prefabs/PlayerCommon/GameTimer").asset as GameObject, parentCanvasPlayer);
        //    GameTimer gameTimer = timer.GetComponent<GameTimer>();
        //    if (gameTimer)
        //        gameTimer.ClockReady.AddListener(EndOfTimer);

        //}

        #region ������� ����� �� ������ ��� �������
        //foreach (var _player in MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).players)
        //{
        //    var parentCanvasPlayer = _player.transform.Find("CanvasPlayer");
        //    if (parentCanvasPlayer.Find("GameTimer(Clone)")) return;
        //    else
        //    {
        //        //TODO : ���������� �� ��������  � �� ������� ��������� (�� ������� ��� �����������, �� �������� ��� �������)
        //        Debug.LogWarning("�� � ����� � ����� �������� �������");
        //        //GameObject timer = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.Find(prefab => prefab.name == "GameTimer"), parentCanvasPlayer);
        //        var timer = Instantiate(Resources.LoadAsync("Prefabs/PlayerCommon/GameTimer").asset as GameObject, parentCanvasPlayer);
        //        GameTimer gameTimer = timer.GetComponent<GameTimer>();
        //        if (gameTimer)
        //            gameTimer.ClockReady.AddListener(EndOfTimer);
        //
        //    }
        //}
        #endregion

        #endregion
    }


    [Command(requiresAuthority = false)]
    public void CmdNextWave(NetworkConnectionToClient sender = null) //NetworkConnectionToClient �������� �������� ��� https://mirror-networking.gitbook.io/docs/guides/communications/remote-actions
    {
        Debug.LogWarning("��� ����������� " + sender.identity.name);

        var timer = sender.identity.gameObject.GetComponent<GameTimer>();

        //if (!isStarted)
        //{
        //    if (timer)
        //        timer.ClockReady
        //            .AddListener(EndOfTimer); //���������� �� Event. �� ����� ������ ������ �� ������� � ������� ��� ������� ������� � ������

        //    isStarted = true; //�������� ����� ���������� 1 ���. �� ���������!!!
        //}

        timer.running = true;
        //NextWave(sender.identity.gameObject);
    }



    public void EndOfTimer(GameTimer timer)
    {
        Debug.Log("timer end.");
        currentWave++;

        timer.timer = 1 * 60;

        timer.showTime = "";

        Debug.LogWarning($"currentWave={currentWave} | _allWawe={_allWawe}");
        if (currentWave >= _allWawe)
        {
            isActive = false;
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveNextManagerWave();
            ActiveAhtorityDoors();
        }

        NextSpawnEnemy();

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
            Debug.LogWarning("����� �����������");
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Door[_firstIndexDoorClose + i].hasAthorityTrigger = true;
        }
    }

    /// <summary>
    /// �������� ���������� ��� � ������� �����
    /// </summary>
    /// <returns></returns>
    public int GetEnemySpawn() => EnemyToCurrentWave[currentWave] * MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult;

    /// <summary>
    /// ������� ����� ���������� ���
    /// </summary>
    /// <returns></returns>
    public int GetEnemySpawned() => countSpawned;

    public void SetEnemySpawned()
    {
        countSpawned++;
    }
    [Server]
    public void SetKilledEnemy()
    {
        Debug.LogWarning("SetKilledEnemy ���������");
        killedEnemy++;
    }

}
