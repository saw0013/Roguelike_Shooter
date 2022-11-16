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

    [SerializeField] private float _maxHealth;

    public bool InputActive = true;
    public bool EscapeMenuActive;

    [SyncVar(hook = nameof(SyncHealth))]
    float _SyncHealth;

    private float health;

    void Awake()
    {
        health = _maxHealth;
        _healthSlider.maxValue = _maxHealth / 100;
        _healthSliderRpc.maxValue = _maxHealth / 100;
    }

    [ClientCallback]
    void Update()
    {
        if (hasAuthority)
        {
            // Test
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CmdChangeHealth(health - 10);
                CmdShowHP(_SyncHealth / 100);
                Debug.Log("Hit");
            }
            // Test
        }
    }

    void SyncHealth(float newValue) => health = newValue;

    [Command] //����������, ��� ���� ����� ������ ����� ����������� �� ������� �� ������� �������
    public void CmdChangeHealth(float newValue) //����������� ������ Cmd � ������ �������� ������
    {
        ChangeHealthValue(newValue);  //��������� � ����������������� ��������� ����������
    }

    [Server] //����������, ��� ���� ����� ����� ���������� � ����������� ������ �� �������
    public void ChangeHealthValue(float newValue)
    {
        _SyncHealth = newValue;
    }

    [Command]
    public void CmdShowHP(float PlayerHp)
    {
        _healthSlider.DOValue(PlayerHp, Time.deltaTime * 20);
        _healthSliderRpc.DOValue(PlayerHp, Time.deltaTime * 20);
        _textHealth.text = $"{_SyncHealth}/{_maxHealth}";
    }
}
