using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Mirror;
using MirrorBasics;
using TMPro;
using UnityEngine.Networking.Types;
using Zenject.SpaceFighter;

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

    //ƒанные которые будем синхронизировать.
    [SyncVar(hook = nameof(SyncHealth))]
    public float _SyncHealth;

    private float playerHealth;

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
        _healthSlider.maxValue = _maxHealth / 100;
        _healthSliderRpc.maxValue = _maxHealth / 100;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        //playerHealth = _maxHealth;
    }

    void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
               ClientServerChangeHp(playerHealth - 10);
               LocalShowHP(playerHealth - 10);
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

    void Start()
    {
        playerHealth = _maxHealth;
    }

    #endregion

    #region Server Client Call ChangeHealth

    void ClientServerChangeHp(float hp)
    {
        if (isServer) ChangeHealthValue(hp);//если мы €вл€емс€ сервером, то переходим к непосредственному изменению переменной
        else CmdChangeHealth(hp); //в противном случае делаем на сервер запрос об изменении переменной
    }
    //метод который будет выставл€ть Health в соответствии с синхронизированным значением
    void SyncHealth(float oldvalue, float newValue) => playerHealth = newValue; //об€зательно делаем два значени€ - старое и новое. 

    /// <summary>
    /// метод, который будет мен€ть переменную _SyncHealth. Ётот метод будет выполн€тьс€ только на сервере и мен€ть ’ѕ игрока
    /// </summary>
    /// <param name="newValue"></param>
    [Server]
    public void ChangeHealthValue(float newValue)
    {
        _SyncHealth = newValue;
    }

    /// <summary>
    /// Ѕудем мен€ть и синхронизировать ’ѕ
    /// </summary>
    /// <param name="newValue"></param>
    [Command] //обозначаем, что этот метод должен будет выполн€тьс€ на сервере по запросу клиента
    public void CmdChangeHealth(float newValue) //об€зательно ставим Cmd в начале названи€ метода
    {
        ChangeHealthValue(newValue);  //переходим к непосредственному изменению переменной
        RpcShowHP(newValue);
    }

    /// <summary>
    /// ќбновим дл€ всех клиентов HP над головой чтобы было видно
    /// </summary>
    /// <param name="PlayerHp"></param>
    [ClientRpc]
    void RpcShowHP(float PlayerHp) => _healthSliderRpc.DOValue(PlayerHp / 100, Time.deltaTime * 20);

    internal void BuffHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        _healthSlider.maxValue = maxHealth / 100;
        _healthSliderRpc.maxValue = maxHealth / 100;
        ClientServerChangeHp(maxHealth);
        LocalShowHP(maxHealth);
    }


    #region Ћокальные ’ѕ
    /// <summary>
    /// ќтобразим локальные ’ѕ в верхнем левом углу
    /// </summary>
    /// <param name="PlayerHp"></param>
    void LocalShowHP(float PlayerHp)
    {
        _healthSlider.DOValue(PlayerHp / 100, Time.deltaTime * 20);
        _textHealth.text = $"{playerHealth}/{_maxHealth}";
    }
    #endregion  

    #endregion

    #region Server Client Call DisplayName

    //метод не выполнитс€, если старое значение равно новому
    void NameDisplay(string oldName, string newName) => _nameDisplayRpc.text = newName; //об€зательно делаем два значени€ - старое и новое. 



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
        _textGuard.text = $"ўит: {guardPlayer}";
    }

    public void StopBuffGuard()
    {
        guardPlayer = _guardStart;
        _textGuard.text = $"ўит: {guardPlayer}";
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

            if (playerHealth > 0 && !isImmortal)
            {
                ClientServerChangeHp(playerHealth - damage.damageValue);
                LocalShowHP(playerHealth - damage.damageValue);
            }

            if (damage.damageValue > 0)
                onReceiveDamage.Invoke(damage);

            if (!isDead && playerHealth <= 0)
            {
                isDead = true;
                onDead.Invoke(gameObject);
            }
        }

    }

    #endregion
}
