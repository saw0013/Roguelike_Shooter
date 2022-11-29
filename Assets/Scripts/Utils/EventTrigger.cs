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

    public float delayDestroy = .1f;

    [SerializeField] private string NpcPatroolNameFind = "PatroolPoints";
    [HideInInspector,]
    public bool isTriggered = false;

    [SerializeField] SpawningNPC spawningWho;
    [SerializeField] private Transform spawnMob;

    public UnityEvent OnEnterTrigger;
    public UnityEvent OnExitTrigger;


   

    #endregion

    public override void OnStartServer()
    {
        base.OnStartServer();
    }


    #region OnInspectorSetting
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
        if (NetworkServer.active == false) return;

        if (other.CompareTag("Player") && !isTriggered)
        {

            StartCoroutine(OnTrigEnter());
            isTriggered = true;
            ServerSpawn(other.GetComponent<PlayerMovementAndLookNetwork>().networkMatch.matchId);
        }
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    //Clients and server run OnTriggerEnter so we just need to check           
    //    //for server only, this means you can avoid the [Command] since it         
    //    //is already server side
    //    if (NetworkServer.active == false) return;

    //    if (other.tag == "Player")
    //        ServerSpawn(other.GetComponent<PlayerMovementAndLookNetwork>().networkMatch.matchId);
    //}

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //DO
            OnExitTrigger?.Invoke();
        }
    }

    #endregion

    private void ServerSpawn(Guid matchID)
    {
        switch (spawningWho)
        {
            case SpawningNPC.BigSpider:
                var StartPointNpc = GameObject.Find(NpcPatroolNameFind).transform.GetChild(0);

                var npc = Instantiate(ShooterNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x =>
                    x.name == "SpiderNpc"), StartPointNpc.position, Quaternion.identity);
                npc.GetComponent<NetworkMatch>().matchId = matchID;
                NetworkServer.Spawn(npc);

                break;
        }
    }

    private IEnumerator OnTrigEnter()
    {
        yield return new WaitForSeconds(.1f);
        OnEnterTrigger.Invoke();
        Destroy(this, delayDestroy);
    }

    #region Enums

    enum SpawningNPC
    {
        None = 0,
        BigSpider = 1,
        LowSpider = 1,
        Solder = 2,
        Boss = 10
    }

    #endregion
}
