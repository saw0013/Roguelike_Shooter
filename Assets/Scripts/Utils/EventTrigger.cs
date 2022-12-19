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
    [HideInInspector,]
    public bool isTriggered = false;

    [SerializeField] SpawningNPC spawningWho;
    [SerializeField] int CountSpawn = 5;

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
        if (other.CompareTag("Player") && !isTriggered)
        {
            StartCoroutine(OnTrigEnter());
            isTriggered = true;
            ServerSpawn(other.GetComponent<PlayerMovementAndLookNetwork>().matchID.ToGuid());
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //DO
            OnExitTrigger?.Invoke();
        }
    }

    #endregion

    /// <summary>
    /// —ам спавн пауков
    /// </summary>
    /// <param name="matchID"></param>
    /// <returns></returns>
    public IEnumerator SpawnBigSpiderRandomPoints(Guid matchID)
    {
        var rp = GetPointPatrool.Instance.RandomPoints; //ѕолучим все точки спавна

        for (int i = 0; i < CountSpawn; i++) //—павнить будем в цикле
        {
            var StartPointNpc = rp[Random.Range(0, rp.Count)]; //“очка спавна рандомна€ из списка

            var PreSpawn = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
                x.name == "ParticleSpawnNpc"), StartPointNpc, Quaternion.Euler(-90,0,0));
            PreSpawn.GetComponent<NetworkMatch>().matchId = matchID;
            NetworkServer.Spawn(PreSpawn); 

            yield return new WaitForSeconds(1.2f);
            var npc = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
                x.name == "SpiderNpc"), StartPointNpc, Quaternion.identity);
            npc.GetComponent<NetworkMatch>().matchId = matchID;
            NetworkServer.Spawn(npc); //—павним паука в рандомной точке

            rp.Remove(StartPointNpc); //”далим эту точку, чтобы следующий паук не заспавнилс€ там же
            yield return new WaitForSeconds(2.0f);
        }

    }

    [ServerCallback]
    private void ServerSpawn(Guid matchID)
    {
        switch (spawningWho)
        {
            case SpawningNPC.BigSpider:
                StartCoroutine(SpawnBigSpiderRandomPoints(matchID));
                break;
        }
    }

    private IEnumerator OnTrigEnter()
    {
        yield return new WaitForSeconds(.1f);
        OnEnterTrigger.Invoke();

        if(_destroy)
            Destroy(this, delayDestroy);
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
