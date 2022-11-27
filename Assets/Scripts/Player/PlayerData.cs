using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Mirror;
using MirrorBasics;
using TMPro;
using UnityEngine.Networking.Types;

public class PlayerData : NetworkBehaviour
{
    #region Variables
    public Transform ItemsGrind;

    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _healthSliderRpc;
    [SerializeField] private TMP_Text _textHealth;
    [SerializeField] private TMP_Text _textGuard;

    [SerializeField] private float _startAmmoReload;
    [SerializeField] private int _startForceBulet;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _guardStart;
    [SerializeField] private int _speedStart;
    [SerializeField] private float _damageStart;

    public bool InputActive = true;
    public bool EscapeMenuActive;

    public int SpeedPlayer;
    public float DamagePlayer;
    public float AmmoReload;
    public int BuletForce;

    private float guardPlayer;

    //������ ������� ����� ����������������.
    [SyncVar(hook = nameof(SyncHealth))]
    public float _SyncHealth;

    private float health;

    [SerializeField] internal TMP_Text _nameDisplayRpc;

    [SyncVar(hook = nameof(NameDisplay))]
    public string _nameDisplay;

    [SerializeField] protected OnReceiveDamage _onStartReceiveDamage = new OnReceiveDamage();
    [SerializeField] protected OnReceiveDamage _onReceiveDamage = new OnReceiveDamage();
    [SerializeField] protected OnDead _onDead = new OnDead();
    public OnReceiveDamage onStartReceiveDamage { get { return _onStartReceiveDamage; } protected set { _onStartReceiveDamage = value; } }
    public OnReceiveDamage onReceiveDamage { get { return _onReceiveDamage; } protected set { _onReceiveDamage = value; } }
    public OnDead onDead { get { return _onDead; } protected set { _onDead = value; } }

    private bool isImmortal = false;

    private bool isDead = false;

    #endregion

    #region Awake, Start, Update
    void Awake()
    {
        AmmoReload = _startAmmoReload;
        BuletForce = _startForceBulet;
        guardPlayer = _guardStart;
        SpeedPlayer = _speedStart;
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
                if (isServer) //���� �� �������� ��������, �� ��������� � ����������������� ��������� ����������
                    ChangeHealthValue(health - 10);
                else
                    CmdChangeHealth(health - 10); //� ��������� ������ ������ �� ������ ������ �� ��������� ����������

                LocalShowHP(health - 10);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Bullet"))
        {
            var _damage = collision.transform.GetComponent<Damage>();
            gameObject.ApplyDamage(_damage);
        }
    }

    #endregion

    #region Server Client Call ChangeHealth

    //����� ������� ����� ���������� Health � ������������ � ������������������ ���������
    void SyncHealth(float oldvalue, float newValue) => health = newValue; //����������� ������ ��� �������� - ������ � �����. 

    /// <summary>
    /// �����, ������� ����� ������ ���������� _SyncHealth. ���� ����� ����� ����������� ������ �� ������� � ������ �� ������
    /// </summary>
    /// <param name="newValue"></param>
    [Server]
    public void ChangeHealthValue(float newValue)
    {
        _SyncHealth = newValue;
    }

    /// <summary>
    /// ����� ������ � ���������������� ��
    /// </summary>
    /// <param name="newValue"></param>
    [Command] //����������, ��� ���� ����� ������ ����� ����������� �� ������� �� ������� �������
    public void CmdChangeHealth(float newValue) //����������� ������ Cmd � ������ �������� ������
    {
        ChangeHealthValue(newValue);  //��������� � ����������������� ��������� ����������
        RpcShowHP(newValue);
    }

    /// <summary>
    /// ������� ��� ���� �������� HP ��� ������� ����� ���� �����
    /// </summary>
    /// <param name="PlayerHp"></param>
    [ClientRpc]
    void RpcShowHP(float PlayerHp) => _healthSliderRpc.DOValue(PlayerHp / 100, Time.deltaTime * 20);

    internal void BuffHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        _healthSlider.maxValue = maxHealth / 100;
        _healthSliderRpc.maxValue = maxHealth / 100;
        if (isServer) //���� �� �������� ��������, �� ��������� � ����������������� ��������� ����������
        {
            ChangeHealthValue(_maxHealth);
            Debug.LogWarning("����� ��� � �� ������");
        }

        else
        {
            Debug.LogWarning("����� ��� �� ��������");
            CmdChangeHealth(_maxHealth); //� ��������� ������ ������ �� ������ ������ �� ��������� ����������
        }

        LocalShowHP(_maxHealth);
    }


    #region ��������� ��
    /// <summary>
    /// ��������� ��������� �� � ������� ����� ����
    /// </summary>
    /// <param name="PlayerHp"></param>
    void LocalShowHP(float PlayerHp)
    {
        _healthSlider.DOValue(PlayerHp / 100, Time.deltaTime * 20);
        _textHealth.text = $"{health}/{_maxHealth}";
    }
    #endregion  

    #endregion

    #region Server Client Call DisplayName

    //����� �� ����������, ���� ������ �������� ����� ������
    void NameDisplay(string oldName, string newName) => _nameDisplayRpc.text = newName; //����������� ������ ��� �������� - ������ � �����. 



    [Server]
    public void ShowDisplayName(string newName) => _nameDisplay = newName;
    [Command]
    public void CmdShowName(string newName) => ShowDisplayName(newName);

    #endregion

    #region ItemsBuff

    #region CommonGuard

    public void ChangeGuard(float BuffGuard)
    {
        guardPlayer += BuffGuard;
        _textGuard.text = $"���: {guardPlayer}";
    }

    public void StopBuffGuard()
    {
        guardPlayer = _guardStart;
        _textGuard.text = $"���: {guardPlayer}";
    }
    #endregion

    #region CommonMoveSpeed
    public void ChangeMoveSpeed(int BuffSpeed)
    {
        SpeedPlayer += BuffSpeed;
    }

    public void StopBuffMoveSpeed()
    {
        SpeedPlayer = _speedStart;
    }

    #endregion

    #region CommonDamage
    [Command]
    public void CmdChangeDamage(int newValue)
    {
        ChangeDamageValue(newValue);
    }

    [Server]
    public void ChangeDamageValue(int newValue)
    {
        DamagePlayer = newValue;
    }

    public void ChangeDamage(float BuffDamage)
    {
        DamagePlayer += BuffDamage;
    }

    public void StopBuffDamage()
    {
        DamagePlayer = _damageStart;
    }
    #endregion

    #region CommonAmmo
    public void ChangeAmmo(float BuffAmmoReload, int BuffAmmoForce)
    {
        AmmoReload -= BuffAmmoReload;
        BuletForce += BuffAmmoForce;
    }

    public void StopBuffAmmo()
    {
        AmmoReload = _startAmmoReload;
        BuletForce = _startForceBulet;
    }
    #endregion

    #endregion

    #region Virtual method

    public virtual void TakeDamage(Damage damage)
    {
        if (damage != null)
        {
            onReceiveDamage.Invoke(damage);

            if (health > 0 && !isImmortal)
            {
                if (isServer) //���� �� �������� ��������, �� ��������� � ����������������� ��������� ����������
                {
                    ChangeHealthValue(health - damage.damageValue);
                    Debug.LogWarning("�������� ���� �� ������");
                }

                else
                {
                    Debug.LogWarning("�������� ���� �� ��������");
                    CmdChangeHealth(health - damage.damageValue); //� ��������� ������ ������ �� ������ ������ �� ��������� ����������
                }
                LocalShowHP(health / 100);
            }

            if (damage.damageValue > 0)
                onReceiveDamage.Invoke(damage);

            if (!isDead && health <= 0)
            {
                isDead = true;
                onDead.Invoke(gameObject);
            }
        }

    }

    #endregion
}
