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

    public float currentWave;
    private int countSpawned;
    private float currentTimeDalay;

    public bool isActive;
    bool isStarted = false; //��� ���������� � �������, �� ����� ������ ����� ������ ������� ��� ������� �������. ��� ������ ��������

    [SyncVar(hook = nameof(CheckKilled))]
    private float killedEnemy;

    [Tooltip("���������� ������, �� �������� �����")]
    private Dictionary<int, int> EnemyToCurrentWave = new Dictionary<int, int>();

    private void Start()
    {
        for (int i = 0; i < _allWawe; i++)
        {
            EnemyToCurrentWave.Add(i, EnemyToWave[i]);
        }
    }

    //private void OnChangeWave(float _Old, float _New)
    //{
    //    Debug.LogWarning(GetEnemySpawn());
    //    Debug.LogWarning(_New);
    //    Debug.LogWarning(killedEnemy);


    //    if (killedEnemy >= GetEnemySpawn() & !isStarted)
    //    {
    //        Debug.LogWarning("����. �����");
    //        //if (isServer) NextWave();
    //        //else CmdNextWave();
    //        CmdNextWave();
    //    }
    //}

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
        isStarted = false;

        Debug.LogWarning("��� ����������� " + sender.identity.name);

        sender.identity.gameObject.GetComponent<PlayerData>().ChangeWaveNuberText(true, (int)currentWave);

        var timer = sender.identity.gameObject.GetComponent<GameTimer>();

        timer.timer = 1 * 60;

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

    public void EndOfTimer()
    {
        Debug.Log("timer end.");

        Debug.LogWarning($"currentWave={currentWave} | _allWawe={_allWawe}");
        if (currentWave >= _allWawe && isActive)
        {
            isActive = false;
            ActiveAhtorityDoors();
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveNextManagerWave();
        }
        else if(!isStarted)
        {
            NextSpawnEnemy();

            isStarted = true;
        }
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
    public float GetEnemySpawn() => EnemyToCurrentWave[(int)currentWave] * MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult;

    /// <summary>
    /// ������� ����� ���������� ���
    /// </summary>
    /// <returns></returns>
    public int GetEnemySpawned() => countSpawned;

    public void SetEnemySpawned()
    {
        countSpawned++;
    }

    void CheckKilled(float oldvalue, float newValue)
    {
        Debug.LogWarning(GetEnemySpawn());

        Debug.LogWarning(newValue);

        if (newValue >= GetEnemySpawn())
        {
            Debug.LogWarning("����. �����");

            var player = MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult;
            float wave = 1 / (float)player;
            currentWave += wave;
            Debug.Log(currentWave);

            //if (isServer) NextWave();
            //else CmdNextWave();
            CmdNextWave();
        }
    }

    [Server]
    public void SetKilledEnemy()
    {
        Debug.LogWarning("SetKilledEnemy ���������");

        Debug.LogWarning(MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult);

        var player = MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult;

        float killedAdd = 1 / (float)player;

        Debug.LogWarning(killedAdd);

        Debug.LogWarning("�� " + killedEnemy);

        killedEnemy += killedAdd;

        Debug.LogWarning("����� " + killedEnemy);
    }

}
