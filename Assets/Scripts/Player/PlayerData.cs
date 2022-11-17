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

    //������ ������� ����� ����������������.
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

    //����� ������� ����� ���������� Health � ������������ � ������������������ ���������
    void SyncHealth(float oldvalue, float newValue) => health = newValue;

    [Command] //����������, ��� ���� ����� ������ ����� ����������� �� ������� �� ������� �������
    public void CmdChangeHealth(float newValue) //����������� ������ Cmd � ������ �������� ������
    {
        ChangeHealthValue(newValue);  //��������� � ����������������� ��������� ����������
    }

    internal void ChangeHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        _SyncHealth = maxHealth;
        _healthSlider.maxValue = maxHealth / 100;
        _healthSliderRpc.maxValue = maxHealth / 100;
        CmdChangeHealth(maxHealth);
    }

    //�����, ������� ����� ������ ���������� _SyncHealth. ���� ����� ����� ����������� ������ �� �������.
    [Server]
    public void ChangeHealthValue(float newValue)
    {
        _SyncHealth = newValue;
    }

    //������� ��������� �� ���� �������� � ��������� ��� �������
    [ClientRpc]
    void RpcShowHP(float PlayerHp) => _healthSliderRpc.DOValue(PlayerHp, Time.deltaTime * 20);

    //�������� ������� � ������� ����� �������� � ���� ��������
    [Command]
    void CmdShowHP(float PlayerHp) => RpcShowHP(PlayerHp);

    void LocalShowHP(float PlayerHp)
    {
        _healthSlider.DOValue(PlayerHp, Time.deltaTime * 20);
        _textHealth.text = $"{health}/{_maxHealth}";
    }
}
