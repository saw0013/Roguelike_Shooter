using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cosmo;
using MirrorBasics;

public class EnemyData : EnemyBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        StartCoroutine(FOVRoutine());
        base.Start();
        OnStart();
    }

    public override void Update()
    {
        base.Update();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        var cursorManager = GameObject.Find("Cursor").GetComponent<MouseCursorMan>();
        cursorManager.cursor = cursorManager.cursorAttack;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var cursorManager = GameObject.Find("Cursor").GetComponent<MouseCursorMan>();
        cursorManager.cursor = cursorManager.cursorNormal;
    }

    #region BASE class

    public override void FieldOfViewCheck()
    {
        base.FieldOfViewCheck();
    }

    public override IEnumerator FOVRoutine()
    {
        return base.FOVRoutine();
    }

    public override void OnStart() => base.OnStart();


    

    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);
        if (currentHealth <= 0)
        {
            //Debug.LogWarning("Умертвляем павука"); //Выполняется только на стороне клиента
        }
    }


   

    #endregion

}
