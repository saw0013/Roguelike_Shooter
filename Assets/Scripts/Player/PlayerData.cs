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
using Unity.Entities.UniversalDelegates;
using System.Linq;
using ModestTree.Util;
using System;

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

    //Данные которые будем синхронизировать.
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(ChangeCameraToLiveParty());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            //TODO : Сделать счётчик и вести счётчик в BulletPool OnCollisionEnter
            Debug.LogWarning($"Тимдамаг от {other.GetComponent<BulletPool>().owner}");
        }

    }

    void OnCollisionEnter(Collision collision)
    {

    }

    void Start()
    {
        playerHealth = _maxHealth;
    }

    #endregion

    #region Server Client Call ChangeHealth

    void ClientServerChangeHp(float hp)
    {
        if (isServer) ChangeHealthValue(hp);//если мы являемся сервером, то переходим к непосредственному изменению переменной
        else CmdChangeHealth(hp); //в противном случае делаем на сервер запрос об изменении переменной
    }
    //метод который будет выставлять Health в соответствии с синхронизированным значением
    void SyncHealth(float oldvalue, float newValue) => playerHealth = newValue; //обязательно делаем два значения - старое и новое. 

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

    internal void BuffHealth(float maxHealth)
    {
        if (hasAuthority) //Если мы имеем право на изменение
        {
            _maxHealth = maxHealth;
            _healthSlider.maxValue = maxHealth / 100;
            _healthSliderRpc.maxValue = maxHealth / 100;
            ClientServerChangeHp(maxHealth);
            LocalShowHP(maxHealth);
        }
    }


    #region Локальные ХП
    /// <summary>
    /// Отобразим локальные ХП в верхнем левом углу
    /// </summary>
    /// <param name="PlayerHp"></param>
    void LocalShowHP(float PlayerHp)
    {
        if (isLocalPlayer) //Если мы локальный игрок
        {
            _healthSlider.DOValue(PlayerHp / 100, Time.deltaTime * 20);
            _textHealth.text = $"{PlayerHp}/{_maxHealth}";
        }
    }
    #endregion  

    #endregion

    #region Server Client Call DisplayName

    //метод не выполнится, если старое значение равно новому
    void NameDisplay(string oldName, string newName) => _nameDisplayRpc.text = newName; //обязательно делаем два значения - старое и новое. 

    [Server]
    public void ShowDisplayName(string newName) => _nameDisplay = newName;

    [Command]
    public void CmdShowName(string newName) => ShowDisplayName(newName);

    #endregion

    #region ItemsBuff

    #region CommonGuard

    public void ChangeGuard(float BuffGuard)
    {
        if (hasAuthority)
        {
            guardPlayer += BuffGuard;
            _textGuard.text = $"Щит: {guardPlayer}";
        }
    }

    public void StopBuffGuard()
    {
        if (hasAuthority)
        {
            guardPlayer = _guardStart;
            _textGuard.text = $"Щит: {guardPlayer}";
        }
    }
    #endregion

    #region CommonMoveSpeed
    public void ChangeMoveSpeed(int BuffSpeed)
    {
        if (hasAuthority)
            SpeedPlayer += BuffSpeed;
    }

    public void StopBuffMoveSpeed()
    {
        if (hasAuthority)
            SpeedPlayer = _speedStart;
    }

    #endregion

    #region CommonDamage


    public void ChangeDamage(float BuffDamage)
    {
        if (hasAuthority)
            DamagePlayer += BuffDamage;
    }

    public void StopBuffDamage()
    {
        if(hasAuthority)
            DamagePlayer = _damageStart;
    }
    #endregion

    #region CommonAmmo
    public void ChangeAmmo(float BuffAmmoReload, int BuffAmmoForce)
    {
        if (hasAuthority)
        {
            AmmoReload -= BuffAmmoReload;
            BuletForce += BuffAmmoForce;
        }
    }

    public void StopBuffAmmo()
    {
        if (hasAuthority)
        {
            AmmoReload = _startAmmoReload;
            BuletForce = _startForceBulet;
        }
    }
    #endregion

    #endregion

    #region Virtual method

    public virtual void TakeDamage(Damage damage)
    {
        if (hasAuthority)
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
                    InputActive = false;
                    StartCoroutine(ChangeCameraToLiveParty());
                }
            }
        }

    }

    private List<Transform> RoomPlayers()
    {
        var playerlist = new List<Transform>();
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
            if (p.TryGetComponent(out PlayerMovementAndLookNetwork playerMov))
            {
                if (playerMov.matchID == GetComponent<PlayerMovementAndLookNetwork>().matchID)
                {
                    playerlist.Add(p.transform);
                }
            }

        return playerlist;
    }


    private IEnumerator ChangeCameraToLiveParty()
    {
        var listPlayer = RoomPlayers();
        yield return new WaitForSeconds(5.0f);

        foreach (var player in listPlayer)
        {
            if (player.GetComponent<NetworkIdentity>().netId != netId)
                GetComponent<PlayerMovementAndLookNetwork>().vCamera.Follow = player;
        }

        yield return new WaitForSeconds(3.0f);
        //GetComponentsInChildren<Collider>().ToList().ForEach(col => { DestroyImmediate(col); });
    }

    #endregion
}
