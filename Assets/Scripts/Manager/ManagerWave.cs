using System;
using System.Collections;
using System.Collections.Generic;
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
        if (killedEnemy >= EnemyToWave[currentWave])
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
    /// ��������� �����
    /// </summary>
    private void NextWave()
    {

        //currentWave++;
        //currentTimeDalay = 0;
        //if (currentWave >= _allWawe)
        //{
        //    isActive = false;
        //    MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveNextManagerWave();
        //    ActiveAhtorityDoors();
        //}
        //else NextSpawnEnemy();

        #region AddTimer ����� ������������ ��������� �����
        Debug.LogWarning($"isServer={isServer} | isClient={isClient} | isLocalPlayer={isLocalPlayer} | hasAuthority={hasAuthority}");
        foreach (var _player in MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).players)
        {
            var parentCanvasPlayer = _player.transform.Find("CanvasPlayer");
            GameObject timer = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.Find(prefab => prefab.name == "GameTimer"), parentCanvasPlayer);

            Debug.LogWarning("��� ����� ������ " + timer.name);
            GameTimer gameTimer = timer.GetComponent<GameTimer>();
            if (gameTimer)
                gameTimer.ClockReady.AddListener(EndOfTimer);

        }
        #endregion
    }

    public void EndOfTimer()
    {
        Debug.Log("timer ready.");
        currentWave++;

        if (currentWave >= _allWawe)
        {
            isActive = false;
            MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveNextManagerWave();
            ActiveAhtorityDoors();
        }
        // End of match code here
    }

    public void DeactiveAhtorityDoors()
    {
        for (int i = 0; i < 2; i++)
        {
            Debug.LogWarning("����� �����������");
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
    /// �������� ���������� ��� � ������� �����
    /// </summary>
    /// <returns></returns>
    public int GetEnemySpawn() => EnemyToCurrentWave[currentWave] * MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).Difficult;

    /// <summary>
    /// ������� ����� ���������� ���
    /// </summary>
    /// <returns></returns>
    public int GetEnemySpawned() => countSpawned;

    public int SetEnemySpawned() => countSpawned++;

    public int SetKilledEnemy()
    {
        Debug.LogWarning("����� �����");
        return killedEnemy++;
    }
}
