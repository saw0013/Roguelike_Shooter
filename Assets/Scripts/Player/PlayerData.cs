using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Mirror;
using TMPro;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _healthSliderRpc;
    [SerializeField] private TMP_Text _textHealth;
    [SerializeField] private TMP_Text _textHealthRpc;

    [SerializeField] private float _maxHealth;

    public bool InputActive = true;
    public bool EscapeMenuActive;

    //Данные которые будем синхронизировать.
    [SyncVar(hook = nameof(SyncHealth))]
    public float _SyncHealth;

    [SyncVar]
    public string UserName;

    private float health;

    void Awake()
    {
        UserName = PlayerPrefs.GetString("PlayerName");
        _textHealthRpc.text = UserName;
        health = _maxHealth;
        _healthSlider.maxValue = _maxHealth / 100;
        _healthSliderRpc.maxValue = _maxHealth / 100;
    }

    void Update()
    {
        if (hasAuthority)
        {
            // Test
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CmdChangeHealth(health - 10);
                CmdShowHP(health / 100);
                LocalShowHP(health / 100);
                Debug.Log("Hit");
            }
            // Test
        }
    }

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
        _SyncHealth = maxHealth;
        _healthSlider.maxValue = maxHealth / 100;
        _healthSliderRpc.maxValue = maxHealth / 100;
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
}
