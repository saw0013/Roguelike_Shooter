using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyData : EnemyBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Space(30)]
    [Header("Enemy Data"), Tooltip("Health")]
    [SerializeField] private float _maxHealth = 150;
    [SerializeField] private Slider _healthSliderRpc;

    [SyncVar(hook = nameof(SyncHealth))]
    public float _SyncHealth;

    private float health;
    [SerializeField] private bool FillHealth = true;


    void Start()
    {
        if (FillHealth)
        {
            health = _maxHealth;
            _healthSliderRpc.maxValue = _maxHealth / 100;
        }
        StartCoroutine(FOVRoutine());
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


    #region Server Client Call ChangeHealth
    //метод который будет выставлять Health в соответствии с синхронизированным значением
    void ClientServerChangeHp(float hp)
    {
        if (isServer) ChangeHealthValue(hp);//если мы являемся сервером, то переходим к непосредственному изменению переменной
        else CmdChangeHealth(hp); //в противном случае делаем на сервер запрос об изменении переменной
    }
    //метод который будет выставлять Health в соответствии с синхронизированным значением
    void SyncHealth(float oldvalue, float newValue) => health = newValue; //обязательно делаем два значения - старое и новое. 

    /// <summary>
    /// метод, который будет менять переменную _SyncHealth. Этот метод будет выполняться только на сервере и менять ХП игрока
    /// </summary>
    /// <param name="newValue"></param>
    [Server]
    public void ChangeHealthValue(float newValue)
    {
        _SyncHealth = newValue;
    }

    /// <summary>
    /// Будем менять и синхронизировать ХП
    /// </summary>
    /// <param name="newValue"></param>
    [Command] //обозначаем, что этот метод должен будет выполняться на сервере по запросу клиента
    public void CmdChangeHealth(float newValue) //обязательно ставим Cmd в начале названия метода
    {
        ChangeHealthValue(newValue);  //переходим к непосредственному изменению переменной
        RpcShowHP(newValue);
    }

    /// <summary>
    /// Обновим для всех клиентов HP над головой чтобы было видно
    /// </summary>
    /// <param name="PlayerHp"></param>
    [ClientRpc]
    void RpcShowHP(float PlayerHp) => _healthSliderRpc.DOValue(PlayerHp / 100, Time.deltaTime * 20);

    #endregion

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

    #endregion
}
