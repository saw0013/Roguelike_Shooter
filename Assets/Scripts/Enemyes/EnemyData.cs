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
    //����� ������� ����� ���������� Health � ������������ � ������������������ ���������
    void SyncHealth(float oldvalue, float newValue) => health = newValue;

    //[Command] //����������, ��� ���� ����� ������ ����� ����������� �� ������� �� ������� �������
    public void CmdChangeHealth(float newValue) //����������� ������ Cmd � ������ �������� ������
    {
        ChangeHealthValue(newValue);  //��������� � ����������������� ��������� ����������
    }

    internal void ChangeHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        _healthSliderRpc.maxValue = maxHealth / 100;
        LocalShowHP(maxHealth);
        CmdShowHP(maxHealth);
        CmdChangeHealth(maxHealth);
    }

    //�����, ������� ����� ������ ���������� _SyncHealth. ���� ����� ����� ����������� ������ �� �������.
    //[Server]
    public void ChangeHealthValue(float newValue)
    {
        _SyncHealth = newValue;
    }

    //������� ��������� �� ���� �������� � ��������� ��� �������
    //[ClientRpc]
    void RpcShowHP(float PlayerHp) => _healthSliderRpc.DOValue(PlayerHp, Time.deltaTime * 20);

    //�������� ������� � ������� ����� �������� � ���� ��������
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
