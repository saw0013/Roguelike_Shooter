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


    
    [ServerCallback]
    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);

        //Рабочий метод отслеживания кто попал по пауку
        if (damage.sender.TryGetComponent<BulletPool>(out BulletPool bullet))
        {

            if (purpose == null)
            {
                purpose = bullet._owner.GetComponent<Collider>();
            }
            //Debug.LogWarning("RECIVER у паука " + bullet._owner.name + " hp enemy: " + _SyncHealth); //Вызывается на сервере отлично. На клиенте ошибка. Попробовать использовать атрибут [ServerCallback]
            if (_SyncHealth <= 0)
            {
                var player = bullet._owner.GetComponent<PlayerData>();
                player.ScorePlayer += 20;
                player.EnemyKilled++;
            }
        }

    }


   

    #endregion

}
