using System;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using Mirror;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using Utils;
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
     *  лиенты и сервер запускают OnTriggerEnter, поэтому нам просто нужно проверить
     * только дл€ сервера, это означает, что вы можете избежать [Command], так как она
    *  уже на стороне сервера
     */
    #region Variables

    [Header("---COLOR COLLIDER ONLY INSPECTOR---")]
    [SerializeField] private bool ShowWire = false;
    [SerializeField] private Color color;

    [SerializeField] private bool _destroy = false;
    public float delayDestroy = .1f;

    //[SerializeField] private string NpcPatroolNameFind = "PatroolPoints";
    [HideInInspector]
    public bool isTriggered = false;

    //[HideInInspector]
    public bool hasAthorityTrigger = true;

    [Tooltip("ƒистанци€ котора€, при условии, что находитс€ в расто€нии меньше значени€, переместит игрока к другому игроку")]
    [SerializeField] private float _maxDistanceToPlayer;

    [SerializeField] SpawningNPC spawningWho;
    //[SerializeField] int CountSpawn = 5;
    [SerializeField] private ManagerWave _managerWave;

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

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered && hasAthorityTrigger)
        {
            StartCoroutine(OnTrigEnter());
            isTriggered = true;
            if(spawningWho != SpawningNPC.None)
            {
                MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).players.ForEach(p =>
                {
                    Debug.LogWarning($" игрок {p.name}");
                    var _distance = Vector3.Distance(other.transform.position, p.transform.position);
                    if (_distance > _maxDistanceToPlayer)
                    {
                        p.transform.position = new Vector3(other.transform.position.x + 5, other.transform.position.z + 5);
                        Debug.LogWarning($"игрок {p.name} ƒалеко от игрока {other.name}");
                    }
                    else Debug.LogWarning($"игрок {p.name} близко к игроку {other.name}");
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
    /// —ам спавн пауков
    /// </summary>
    /// <param name="matchID"></param>
    /// <returns></returns>
    public IEnumerator SpawnBigSpiderRandomPoints(Guid matchID, string who)
    {
        var rp = GetPointPatrool.Instance.RandomPoints; //ѕолучим все точки спавна

        //for (int i = 0; i < CountSpawn; i++) //—павнить будем в цикле
        //{
        //    var StartPointNpc = rp[Random.Range(0, rp.Count)]; //“очка спавна рандомна€ из списка

        //    var PreSpawn = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
        //        x.name == "ParticleSpawnNpc"), StartPointNpc, Quaternion.Euler(-90, 0, 0));
        //    PreSpawn.GetComponent<NetworkMatch>().matchId = matchID;
        //    NetworkServer.Spawn(PreSpawn);

        //    yield return new WaitForSeconds(1.2f);
        //    var npc = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
        //        x.name == "SpiderNpc"), StartPointNpc, Quaternion.identity);
        //    npc.GetComponent<NetworkMatch>().matchId = matchID;
        //    NetworkServer.Spawn(npc); //—павним паука в рандомной точке

        //    rp.Remove(StartPointNpc); //”далим эту точку, чтобы следующий паук не заспавнилс€ там же
        //    yield return new WaitForSeconds(2.0f);
        //}

        var StartPointNpc = rp[Random.Range(0, rp.Count)]; //“очка спавна рандомна€ из списка

        var PreSpawn = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
            x.name == "ParticleSpawnNpc"), StartPointNpc, Quaternion.Euler(-90, 0, 0));
        PreSpawn.GetComponent<NetworkMatch>().matchId = matchID;
        NetworkServer.Spawn(PreSpawn);

        yield return new WaitForSeconds(1.2f);

        Destroy(PreSpawn); //”далим партиклы спавна

        var npc = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
            x.name == who), StartPointNpc, Quaternion.Euler(0, Random.Range(0,360), 0)); //ѕаучок будет рандомно повЄртнут
        npc.GetComponent<NetworkMatch>().matchId = matchID;
        NetworkServer.Spawn(npc); //—павним паука в рандомной точке

        rp.Remove(StartPointNpc); //”далим эту точку, чтобы следующий паук не заспавнилс€ там же
        yield return new WaitForSeconds(2.0f);
        ServerSpawn(matchID); //Ќет смысла циклить спавн
    }

    [ServerCallback]
    public void ServerSpawn(Guid matchID)
    {
        if(spawningWho != SpawningNPC.None)
        {
            if (_managerWave == null) return;

            //if(_managerWave.matchId == null) _managerWave.matchId = matchID;

            if (_managerWave.GetEnemySpawned() < _managerWave.GetEnemySpawn())
            {
                SpawningNPC randomNPC = (SpawningNPC)Random.Range(1, 3);

                switch (randomNPC)
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

        if(_destroy)
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
        Boss = 10
    }

    #endregion
}
