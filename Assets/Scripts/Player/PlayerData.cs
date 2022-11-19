using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Mirror;
using MirrorBasics;
using TMPro;

public class PlayerData : NetworkBehaviour
{
    #region Variables
    public Transform ItemsGrind;

    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _healthSliderRpc;
    [SerializeField] private TMP_Text _textHealth;
    [SerializeField] private TMP_Text _textGuard;

    [SerializeField] private float _maxHealth;
    [SerializeField] private float _guardStart;

    public bool InputActive = true;
    public bool EscapeMenuActive;

    [SyncVar(hook = nameof(SyncHealth))]
    private float _SyncGuardPlayer;
    private float guardPlayer;

    //Данные которые будем синхронизировать.
    [SyncVar(hook = nameof(SyncHealth))]
    public float _SyncHealth;
    private float health;

    [SerializeField] internal TMP_Text _nameDisplayRpc;

    [SyncVar(hook = nameof(NameDisplay))]
    public string _nameDisplay;

    #endregion

    #region Awake, Start, Update
    void Awake()
    {
        guardPlayer = _guardStart;
        health = _maxHealth;
        _healthSlider.maxValue = _maxHealth / 100;
        _healthSliderRpc.maxValue = _maxHealth / 100;
    }

    void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CmdChangeHealth(health - 10);
                CmdShowHP(health / 100);
                LocalShowHP(health / 100);
            }
        }
    }

    #endregion

    #region Server Client Call ChangeHealth
    //метод который будет выставлять Health в соответствии с синхронизированным значением
    void SyncHealth(float oldvalue, float newValue) => health = newValue;

    [Command] //обозначаем, что этот метод должен будет выполняться на сервере по запросу клиента
    public void CmdChangeHealth(float newValue) //обязательно ставим Cmd в начале названия метода
    {
        ChangeHealthValue(newValue);  //переходим к непосредственному изменению переменной
    }

    internal void ChangeHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        _healthSlider.maxValue = maxHealth / 100;
        _healthSliderRpc.maxValue = maxHealth / 100;
        LocalShowHP(maxHealth);
        CmdShowHP(maxHealth);
        CmdChangeHealth(maxHealth);
    }

    //метод, который будет менять переменную _SyncHealth. Этот метод будет выполняться только на сервере.
    [Server]
    public void ChangeHealthValue(float newValue)
    {
        _SyncHealth = newValue;
    }

    //Сделаем изменения на всех клиентах в полосочке над головой
    [ClientRpc]
    void RpcShowHP(float PlayerHp) => _healthSliderRpc.DOValue(PlayerHp, Time.deltaTime * 20);

    //Выполним команду с сервера чтобы обновить у всех клиентов
    [Command]
    void CmdShowHP(float PlayerHp) => RpcShowHP(PlayerHp);

    void LocalShowHP(float PlayerHp)
    {
        _healthSlider.DOValue(PlayerHp, Time.deltaTime * 20);
        _textHealth.text = $"{health}/{_maxHealth}";
    }

    #endregion

    #region Server Client Call DisplayName

    void NameDisplay(string oldName, string newName) => _nameDisplayRpc.text = newName;
    [Server]
    public void ShowDisplayName(string newName) => _nameDisplay = newName;
    [Command]
    public void CmdShowName(string newName) => ShowDisplayName(newName);

    #endregion

    #region ItemsBuff

    #region CommonGuard
    void SyncGuard(float oldvalue, float newValue) => guardPlayer = newValue;

    [Command] 
    public void CmdChangeGuard(float newValue) 
    {
        ChangeGuardValue(newValue);  
    }

    [Server]
    public void ChangeGuardValue(float newValue)
    {
        _SyncHealth = newValue;
    }

    public void ChangeGuard(float BuffGuard)
    {
        guardPlayer += BuffGuard;
        _textGuard.text = $"Щит: {guardPlayer}";
    }

    public void StopBuffGuard()
    {
        guardPlayer = _guardStart;
        _textGuard.text = $"Щит: {guardPlayer}";
    }

    #endregion

    #endregion
}
