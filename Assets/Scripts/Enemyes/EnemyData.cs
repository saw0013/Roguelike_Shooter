using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyData : EnemyBehaviour
{
    [Space(30)]
    [Header("Enemy Data"), Tooltip("Health")]
    [SerializeField] private float _maxHealth = 150;
    [SerializeField] private Slider _healthSliderRpc;
    //[SyncVar(hook = nameof(SyncHealth))]
    public float _SyncHealth;
    private float health;
    [SerializeField] private bool FillHealth = true;

    void Awake()
    {
        if (FillHealth)
        {
            health = _maxHealth;
            _healthSliderRpc.maxValue = _maxHealth / 100;
        }
    }

    void Start()
    {
        StartCoroutine(FOVRoutine());
        OnStart();
    }

    #region Server Client Call ChangeHealth
    //метод который будет выставлять Health в соответствии с синхронизированным значением
    void SyncHealth(float oldvalue, float newValue) => health = newValue;

    //[Command] //обозначаем, что этот метод должен будет выполняться на сервере по запросу клиента
    public void CmdChangeHealth(float newValue) //обязательно ставим Cmd в начале названия метода
    {
        ChangeHealthValue(newValue);  //переходим к непосредственному изменению переменной
    }

    internal void ChangeHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        _healthSliderRpc.maxValue = maxHealth / 100;
        LocalShowHP(maxHealth);
        CmdShowHP(maxHealth);
        CmdChangeHealth(maxHealth);
    }

    //метод, который будет менять переменную _SyncHealth. Этот метод будет выполняться только на сервере.
    //[Server]
    public void ChangeHealthValue(float newValue)
    {
        _SyncHealth = newValue;
    }

    //Сделаем изменения на всех клиентах в полосочке над головой
    //[ClientRpc]
    void RpcShowHP(float PlayerHp) => _healthSliderRpc.DOValue(PlayerHp, Time.deltaTime * 20);

    //Выполним команду с сервера чтобы обновить у всех клиентов
    //[Command]
    void CmdShowHP(float PlayerHp) => RpcShowHP(PlayerHp);

    void LocalShowHP(float PlayerHp)
    {
        //_textHealth.text = $"{health}/{_maxHealth}";
    }

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
