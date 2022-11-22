using System;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

//[ExecuteAlways]
[RequireComponent(typeof(BoxCollider))]
public class EventTrigger : MonoBehaviour
{

    [Header("---COLOR COLLIDER ONLY INSPECTOR---")]
    [SerializeField] private bool ShowWire = false;
    [SerializeField] private Color color;

    public float delayDestroy = .1f;

    [HideInInspector,]
    public bool isTriggered;

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
}
