using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Etrigger : MonoBehaviour
{
    [Header("---COLOR COLLIDER ONLY INSPECTOR---")]
    [SerializeField] private bool ShowWire = false;
    [SerializeField] private Color color;

    public float delayDestroy = .1f;

    //[SerializeField] private string NpcPatroolNameFind = "PatroolPoints";
    [HideInInspector,]
    public bool isTriggered = false;

    public UnityEvent OnEnterTrigger;
    public UnityEvent OnExitTrigger;



 

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

    }

    #endregion

    #region Trigger Methods

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            StartCoroutine(OnTrigEnter());
            isTriggered = true;
         
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

   


    private IEnumerator OnTrigEnter()
    {
        yield return new WaitForSeconds(.1f);
        OnEnterTrigger.Invoke();
        Destroy(this, delayDestroy);
    }

    #region Enums

   

    #endregion
}
