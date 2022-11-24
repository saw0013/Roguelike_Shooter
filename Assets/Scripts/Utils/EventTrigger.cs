using System;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using Mirror;
using UnityEngine.SceneManagement;

//[ExecuteAlways]
[RequireComponent(typeof(BoxCollider))]
public class EventTrigger : NetworkBehaviour
{

    [Header("---COLOR COLLIDER ONLY INSPECTOR---")]
    [SerializeField] private bool ShowWire = false;
    [SerializeField] private Color color;

    public float delayDestroy = .1f;

    [HideInInspector,]
    public bool isTriggered = false;

    [SerializeField] SpawningNPC spawningNPC;

    public UnityEvent OnEnterTrigger;

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


    void Start()
    {
        
    }

    [ServerCallback]
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")/* && !isTriggered*/)
        {
            Debug.Log("����� ����� � ConnID "+ other.GetComponent<NetworkIdentity>().netId);
            CmdSpawnSpider();
            //StartCoroutine(OnTrigEnter());
            //isTriggered = true;

        }
    }

    [Command]
    void Cmd_AssignLocalAuthority(GameObject player)
    {
        NetworkIdentity ni = player.GetComponent<NetworkIdentity>();
        ni.AssignClientAuthority(connectionToClient);
    }

    //public void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        other.GetComponent<NetworkIdentity>()
    //            .RemoveClientAuthority();
    //    }
    //
    //    Debug.Log("���������� ������ ���");
    //}

    private IEnumerator OnTrigEnter()
    {
        yield return new WaitForSeconds(.1f);
        OnEnterTrigger.Invoke();
        Destroy(this, delayDestroy);
    }

    [ClientRpc]
    private void RpcSpawn()
    {
        var go = ((ShooterNetworkManager)NetworkManager.singleton).spawnPrefabs.FirstOrDefault(x =>
            x.name == "BarrelExpl");

        var _obj = Instantiate(go, transform);
        //SceneManager.MoveGameObjectToScene(_obj, gameObject.scene);
        //NetworkServer.Spawn(_obj);

        Debug.Log("������� �������");
        switch (spawningNPC)
        {
            case SpawningNPC.Spider:
                break;
        }
    }

    [Command]
    void CmdSpawnSpider()
    {
        var go = Instantiate(Resources.LoadAsync("Prefabs/BarrelExpl").asset as GameObject);
        var _obj = Instantiate(go);
        NetworkServer.Spawn(_obj);
    }

    enum SpawningNPC
    {
        None = 0,
        Spider = 1,
        Solder = 2,
        Boss = 3
    }
}
