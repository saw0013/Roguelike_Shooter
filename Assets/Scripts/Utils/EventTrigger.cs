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

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            CmdSpawnSpider();
            StartCoroutine(OnTrigEnter());
            isTriggered = true;
            
        }
    }

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
            x.name == "Level");

        var _obj = Instantiate(go, transform);
        //SceneManager.MoveGameObjectToScene(_obj, gameObject.scene);
        //NetworkServer.Spawn(_obj);

        Debug.Log("Спавним Уровень");
        switch (spawningNPC)
        {
            case SpawningNPC.Spider:
                break;
        }
    }

    [Command]
    void CmdSpawnSpider()
    {
        RpcSpawn();
    }

    enum SpawningNPC
    {
        None = 0,
        Spider = 1,
        Solder = 2,
        Boss = 3
    }
}
