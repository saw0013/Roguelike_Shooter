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
     * Клиенты и сервер запускают OnTriggerEnter, поэтому нам просто нужно проверить
     * только для сервера, это означает, что вы можете избежать [Command], так как она
    *  уже на стороне сервера
     */
    #region Variables

    [Header("---COLOR COLLIDER ONLY INSPECTOR---")]
    [SerializeField] private bool ShowWire = false;
    [SerializeField] private Color color;

    [SerializeField] private bool _destroy = false;
    public float delayDestroy = .1f;

    //[SerializeField] private string NpcPatroolNameFind = "PatroolPoints";
    [HideInInspector,]
    public bool isTriggered = false;

    [HideInInspector,]
    public bool hasAthorityTrigger = true;

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
            ServerSpawn(other.GetComponent<PlayerMovementAndLookNetwork>().matchID.ToGuid());
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
    /// Сам спавн пауков
    /// </summary>
    /// <param name="matchID"></param>
    /// <returns></returns>
    public IEnumerator SpawnBigSpiderRandomPoints(Guid matchID, string who)
    {
        var rp = GetPointPatrool.Instance.RandomPoints; //Получим все точки спавна

        //for (int i = 0; i < CountSpawn; i++) //Спавнить будем в цикле
        //{
        //    var StartPointNpc = rp[Random.Range(0, rp.Count)]; //Точка спавна рандомная из списка

        //    var PreSpawn = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
        //        x.name == "ParticleSpawnNpc"), StartPointNpc, Quaternion.Euler(-90, 0, 0));
        //    PreSpawn.GetComponent<NetworkMatch>().matchId = matchID;
        //    NetworkServer.Spawn(PreSpawn);

        //    yield return new WaitForSeconds(1.2f);
        //    var npc = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
        //        x.name == "SpiderNpc"), StartPointNpc, Quaternion.identity);
        //    npc.GetComponent<NetworkMatch>().matchId = matchID;
        //    NetworkServer.Spawn(npc); //Спавним паука в рандомной точке

        //    rp.Remove(StartPointNpc); //Удалим эту точку, чтобы следующий паук не заспавнился там же
        //    yield return new WaitForSeconds(2.0f);
        //}

        var StartPointNpc = rp[Random.Range(0, rp.Count)]; //Точка спавна рандомная из списка

        var PreSpawn = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
            x.name == "ParticleSpawnNpc"), StartPointNpc, Quaternion.Euler(-90, 0, 0));
        PreSpawn.GetComponent<NetworkMatch>().matchId = matchID;
        NetworkServer.Spawn(PreSpawn);

        yield return new WaitForSeconds(1.2f);

        Destroy(PreSpawn); //Удалим партиклы спавна

        var npc = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
            x.name == who), StartPointNpc, Quaternion.Euler(0, Random.Range(0,360), 0)); //Паучок будет рандомно повёртнут
        npc.GetComponent<NetworkMatch>().matchId = matchID;
        NetworkServer.Spawn(npc); //Спавним паука в рандомной точке

        rp.Remove(StartPointNpc); //Удалим эту точку, чтобы следующий паук не заспавнился там же
        yield return new WaitForSeconds(2.0f);
        ServerSpawn(matchID);
    }

    [ServerCallback]
    public void ServerSpawn(Guid matchID)
    {
        if(spawningWho != SpawningNPC.None)
        {
            if (_managerWave == null) return;

            if(_managerWave.matchId == null) _managerWave.matchId = matchID;

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

        #region
       // GameObject timer = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GameTimer"));
       //
       // // Add a callback when the timer reaches 0
       // GameTimer gameTimer = timer.GetComponent<GameTimer>();
       // if (gameTimer)
       //     gameTimer.ClockReady.AddListener(EndOfTimer);
        #endregion

        yield return null;
    }

    public void EndOfTimer()
    {
        Debug.Log("timer ready.");

        // End of match code here
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
