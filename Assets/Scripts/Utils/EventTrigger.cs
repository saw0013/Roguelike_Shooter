using System;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cosmoground;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using Mirror;
using Random = UnityEngine.Random;
using MirrorBasics;

//[ExecuteAlways]
[RequireComponent(typeof(BoxCollider))]
public class EventTrigger : NetworkBehaviour
{
    /*
     * Clients and server run OnTriggerEnter so we just need to check
     * for server only, this means you can avoid the [Command] since it
     * is already server side
     *
     * ===========RU============
     * ������� � ������ ��������� OnTriggerEnter, ������� ��� ������ ����� ���������
     * ������ ��� �������, ��� ��������, ��� �� ������ �������� [Command], ��� ��� ���
    *  ��� �� ������� �������
     */
    #region Variables

    [Header("---COLOR COLLIDER ONLY INSPECTOR---")]
    [SerializeField] private bool ShowWire = false;
    [SerializeField] private Color color;

    [SerializeField] private bool _destroy = false;
    [SerializeField] private bool _once;
    public float delayDestroy = .1f;

    //[SerializeField] private string NpcPatroolNameFind = "PatroolPoints";
    [HideInInspector]
    public bool isTriggered = false;

    //[HideInInspector]
    public bool hasAthorityTrigger = true;

    [Tooltip("��������� �������, ��� �������, ��� ��������� � ��������� ������ ��������, ���������� ������ � ������� ������")]
    [SerializeField] private float _maxDistanceToPlayer;

    [SerializeField] SpawningNPC spawningWho;
    //[SerializeField] int CountSpawn = 5;
    public ManagerWave _managerWave;

    public UnityEvent OnEnterTrigger;
    public UnityEvent OnExitTrigger;

    #endregion

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    #region DrawGizmos
    void OnDrawGizmos()
    {

        BoxCollider bCol = GetComponent<BoxCollider>();
        SphereCollider sCol = GetComponent<SphereCollider>();

        Gizmos.color = color;

        if (bCol != null)
        {
            if (ShowWire)
                Gizmos.DrawWireCube(bCol.bounds.center, bCol.bounds.size);
            else
                Gizmos.DrawCube(bCol.bounds.center, bCol.bounds.size);
        }

        if (sCol != null)
        {
            float maxScale = Math.Max(gameObject.transform.localScale.x,
            Math.Max(gameObject.transform.localScale.y, gameObject.transform.localScale.z));

            Gizmos.DrawWireSphere(sCol.bounds.center, sCol.radius * maxScale);
        }
    }

    #endregion

    #region Trigger Methods

    [ClientRpc]
    void RpcTrigger()
    {
        StartCoroutine(OnTrigEnter());
    }
    
    [ClientRpc]
    public void RpcChangeMusic(string Music)
    {
        var musicManager = GameObject.Find("MusicManager").GetComponent<ChangeTheme>();

        musicManager.ChangeMusic(Music);
    }

    [ClientRpc]
    public void RpcTeleport(Transform _transform, Vector3 newPosition)
    {
        _transform.position = newPosition;
    }

    [ServerCallback]
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered && hasAthorityTrigger)
        {
            if (_once && hasAthorityTrigger) hasAthorityTrigger = false;
            //MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).TestCmd();
            //StartCoroutine(OnTrigEnter());
            RpcTrigger();
            isTriggered = true;
            if (spawningWho != SpawningNPC.None)
            {
                RpcChangeMusic("BattleTheme");
                var AllWave = MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveWave.GetAllWave();
                MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).players.ForEach(p =>
                {
                    var _distance = Vector3.Distance(other.transform.position, p.transform.position);
                    var playerData = p.GetComponent<PlayerData>();

                    playerData.ChangeWaveNuberText("����� " + 1 + "/" + AllWave);

                    Debug.LogWarning("HP Player on trigger " + playerData._SyncHealth);

                    if (_distance > _maxDistanceToPlayer && playerData._SyncHealth > 0)
                    {
                        var rndRadius = Random.Range(-3, 3);
                        //p.transform.position = new Vector3(other.transform.position.x + 5, other.transform.position.z + 5);
                        RpcTeleport(p.transform, new Vector3(other.transform.position.x + rndRadius, other.transform.position.y, other.transform.position.z + rndRadius));
                        //Debug.LogWarning($"����� {p.name} ������ �� ������ {other.name}");
                    }
                    //else Debug.LogWarning($"����� {p.name} ������ � ������ {other.name}");
                });

                ServerSpawn(other.GetComponent<PlayerMovementAndLookNetwork>().matchID.ToGuid());
                _managerWave.DeactiveAhtorityDoors();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && hasAthorityTrigger)
        {
            //DO
            StartCoroutine(OnTrigerExit());
            isTriggered = false;
            //OnExitTrigger?.Invoke();
        }
    }

    #endregion


    /// <summary>
    /// ��� ����� ������
    /// </summary>
    /// <param name="matchID"></param>
    /// <returns></returns>
    public IEnumerator SpawnBigSpiderRandomPoints(Guid matchID, string who)
    {
        var rp = GetComponentInChildren<GetPointPatrool>().RandomPoints;

        #region ������ ����� ???
        //for (int i = 0; i < CountSpawn; i++) //�������� ����� � �����
        //{
        //    var StartPointNpc = rp[Random.Range(0, rp.Count)]; //����� ������ ��������� �� ������

        //    var PreSpawn = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
        //        x.name == "ParticleSpawnNpc"), StartPointNpc, Quaternion.Euler(-90, 0, 0));
        //    PreSpawn.GetComponent<NetworkMatch>().matchId = matchID;
        //    NetworkServer.Spawn(PreSpawn);

        //    yield return new WaitForSeconds(1.2f);
        //    var npc = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
        //        x.name == "SpiderNpc"), StartPointNpc, Quaternion.identity);
        //    npc.GetComponent<NetworkMatch>().matchId = matchID;
        //    NetworkServer.Spawn(npc); //������� ����� � ��������� �����

        //    rp.Remove(StartPointNpc); //������ ��� �����, ����� ��������� ���� �� ����������� ��� ��
        //    yield return new WaitForSeconds(2.0f);
        //}
        #endregion

        var StartPointNpc = rp[Random.Range(0, rp.Count)]; //����� ������ ��������� �� ������

        var PreSpawn = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
            x.name == "ParticleSpawnNpc"), StartPointNpc, Quaternion.Euler(-90, 0, 0));
        PreSpawn.GetComponent<NetworkMatch>().matchId = matchID;
        NetworkServer.Spawn(PreSpawn);

        yield return new WaitForSeconds(1.2f);

        var npc = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
            x.name == who), StartPointNpc, Quaternion.Euler(0, Random.Range(0, 360), 0)); //������ ����� �������� ��������
        npc.GetComponent<NetworkMatch>().matchId = matchID;
        NetworkServer.Spawn(npc); //������� ����� � ��������� �����
        MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveWave.AddInListEnemy(npc);

        rp.Remove(StartPointNpc); //������ ��� �����, ����� ��������� ���� �� ����������� ��� ��
        yield return new WaitForSeconds(2.0f);
        Destroy(PreSpawn); //������ �������� ������
        ServerSpawn(matchID); 
    }

    [ServerCallback]
    public void ServerSpawn(Guid matchID)
    {
        if (spawningWho != SpawningNPC.None)
        {
            if (_managerWave == null) return;

            //if(_managerWave.matchId == null) _managerWave.matchId = matchID;

            //if (_managerWave.GetEnemySpawned() < _managerWave.GetEnemySpawn())
            if (_managerWave.GetEnemySpawned() < _managerWave.GetEnemySpawn())
            {
                SpawningNPC NPCSpawn = new SpawningNPC();

                if (spawningWho == SpawningNPC.Random)
                {
                    NPCSpawn = (SpawningNPC)Random.Range(1, 4);
                }
                else
                {
                    NPCSpawn = spawningWho;
                }

                switch (NPCSpawn)
                {
                    case SpawningNPC.BigSpider:
                        StartCoroutine(SpawnBigSpiderRandomPoints(matchID, "SpiderNpc"));
                        break;
                    case SpawningNPC.LowSpider:
                        StartCoroutine(SpawnBigSpiderRandomPoints(matchID, "SmalSpiderNpc"));
                        break;
                    case SpawningNPC.Solder:
                        StartCoroutine(SpawnBigSpiderRandomPoints(matchID, "RangerNpc"));
                        break;
                }
                _managerWave.SetEnemySpawned();
            }
        }
    }

    private IEnumerator OnTrigEnter()
    {
        //yield return new WaitForSeconds(.1f);
        OnEnterTrigger.Invoke();

        if (_destroy)
            Destroy(this, delayDestroy);

        yield return null;
    }


    private IEnumerator OnTrigerExit()
    {
        //yield return new WaitForSeconds(.1f);
        OnExitTrigger?.Invoke();

        if (_destroy)
            Destroy(this, delayDestroy);
        yield return null;
    }

    #region Enums

    enum SpawningNPC
    {
        None = 0,
        BigSpider = 1,
        LowSpider = 2,
        Solder = 3,
        Random = 4,
        Boss = 10
    }

    #endregion
}
