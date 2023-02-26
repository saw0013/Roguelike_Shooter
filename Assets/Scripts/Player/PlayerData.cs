using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmo;
using Cosmoground;
using Mirror;
using TMPro;
using MirrorBasics;
using FMOD.Studio;
using static UnityEngine.UI.GridLayoutGroup;
using Mirror.Examples.Chat;
using Cinemachine;

public class PlayerData : HealthController, ICharacter
{
    #region Variables
    [Header("===UI===")]
    public TMP_Text WaveText;

    public UIStatPlayer ProgressPlayerStat;

    [Header("===Stat PlayerData===")]

    [SyncVar(hook = nameof(UpdateLocalScore))]
    public int ScorePlayer;

    [SyncVar] public int EnemyKilled;
    [SyncVar] public int BuffGive;
    [SyncVar] public int AmmoWasted;

    [Header("===UI PlayerData===")]
    [SerializeField] private TMP_Text TextScorePlayer;

    [Space(10), Header("===PlayerData===")]
    private bool InputActive/* = true*/;

    public PlayerBuffController _playerBuffController;
    private bool EscapeMenuActive;
    public Transform ItemsGrind;

    [SyncVar] public int SpeedPlayer;
    [SyncVar] public int DamagePlayer;
    [SyncVar] public int MaxCartridges;

    [SyncVar] public float BuletRate;
    [SyncVar] public float AmmoReload;
    [SyncVar] public float SizeBullet;

    [SyncVar] public int guardPlayer;

    [SerializeField] private float _startAmmoReload;
    [SerializeField] private float _startRateBulet;
    [SerializeField] private int _guardStart;
    [SerializeField] private int _speedStart;
    [SerializeField] private int _damageStart;
    [SerializeField] private TMP_Text _textGuard;
    [SerializeField] internal TMP_Text _nameDisplayRpc;

    [SyncVar(hook = nameof(NameDisplay))]
    public string _nameDisplay;

    internal static readonly HashSet<string> playerNames = new HashSet<string>();

    [Space(10), Header("===Ragdoll===")]
    public DeathBy deathBy = DeathBy.Animation;

    [HideInInspector]
    public bool debugActionListener;

    private PlayerMovementAndLookNetwork _player;

    /// <summary>
    /// Локальное свойство жив игрок или нет
    /// </summary>

    #region ICharacter & Ragdoll
    [SerializeField] protected OnActiveRagdoll _onActiveRagdoll = new OnActiveRagdoll();
    public OnActiveRagdoll onActiveRagdoll { get { return _onActiveRagdoll; } protected set { _onActiveRagdoll = value; } }

    public Animator animator { get; protected set; }

    public bool _ragdolled = false;
    public virtual bool ragdolled { get { return _ragdolled; } set { _ragdolled = value; } }

    protected AnimatorParameter hitDirectionHash;
    protected AnimatorParameter reactionIDHash;
    protected AnimatorParameter triggerReactionHash;
    protected AnimatorParameter triggerResetStateHash;
    protected AnimatorParameter recoilIDHash;
    protected AnimatorParameter triggerRecoilHash;

    #endregion
    #endregion

    #region Awake, Start, Update
    protected override void Awake()
    {
        base.Awake();
        DamagePlayer = _damageStart;
        AmmoReload = _startAmmoReload;
        BuletRate = _startRateBulet;
        guardPlayer = _guardStart;
        SpeedPlayer = _speedStart;
    }

    private void OnEnable() => Action_OnDead += CmdDie;
    private void OnDisable() => Action_OnDead -= CmdDie;

    public void CmdDie()
    {

    }


    protected override void Start()
    {
        _player = GetComponent<PlayerMovementAndLookNetwork>();
        base.Start();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _nameDisplay = (string)connectionToClient.authenticationData;
        //playerHealth = _maxHealth;
    }

    public override void OnStartLocalPlayer()
    {
        GameChatUI.localPlayerName = _nameDisplay;
    }

    void Update()
    {
        //if (hasAuthority)
        //{
        //    if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        StartCoroutine(ChangeCameraToLiveParty());
        //    }
        //}
        //TODO : только для тестов. В релизе убрать
        if (Input.GetKeyDown(KeyCode.F4))
        {
            InputIsActive(true);
            Debug.LogWarning(GetInputActive());
        }
    }

    // RuntimeInitializeOnLoadMethod -> fast playmode without domain reload
    [RuntimeInitializeOnLoadMethod]
    static void ResetStatics()
    {
        playerNames.Clear();
    }

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

    #region Common

    #region  CommonHP

    internal void BuffHealth(float maxHealth, GameObject item)
    {
        if (isLocalPlayer) //Просто проверить Наверное так и надо делать. Нужно проверить если будут подбираться по 2 бафа далее
        {
            var _newMaxHealth = MaxHealth + (int)maxHealth;

            MaxHealth = _newMaxHealth;
            _healthSlider.maxValue = _newMaxHealth / 100;
            _healthSliderRpc.maxValue = _newMaxHealth / 100;
            _healthDamageSlider.maxValue = _newMaxHealth / 100;
            _healthDamageSliderRpc.maxValue = _newMaxHealth / 100;

            ClientServerChangeHp(_newMaxHealth);//?
            LocalShowHP(_newMaxHealth);//?Исправлю. Но скорее всего так и оставлю не меняй!!!           
            if (!_playerBuffController.BuffIsExist(nameof(DefaultItemHPUI)))
            {
                var _item = Instantiate(item, ItemsGrind);
                //_item.GetComponent<DefaultItemHPUI>().RegisterOwner(this);
            }
        }
    }

    #endregion

    #region CommonGuard

    /// <summary>
    /// Баф защиты
    /// </summary>
    /// <param name="BuffGuard"></param>
    /// <param name="item"></param>
    public void ChangeGuard(int BuffGuard, GameObject item)
    {
        guardPlayer += guardPlayer >= 40 ? 0 : BuffGuard;
        _textGuard.text = $"Щит: {guardPlayer}";

        if (!_playerBuffController.BuffIsExist(nameof(DefaultItemGuardUI)))
        {
            var _item = Instantiate(item, ItemsGrind);
            //_item.GetComponent<DefaultItemGuardUI>().RegisterOwner(this);
        }
        else
        {
            var _findedItem = ItemsGrind.FindChildObjectByType<DefaultItemGuardUI>();
            if (_findedItem) _findedItem.GetComponent<DefaultItemGuardUI>().UpdateBuff();
        }
    }

    public void StopBuffGuard()
    {
        guardPlayer = _guardStart;
        _textGuard.text = $"Щит: {guardPlayer}";
    }

    #endregion

    #region CommonMoveSpeed

    /// <summary>
    /// Баф скорости
    /// </summary>
    /// <param name="BuffSpeed"></param>
    /// <param name="item"></param>
    public void ChangeMoveSpeed(int BuffSpeed, GameObject item)
    {

        SpeedPlayer += SpeedPlayer == 5 ? 1 : SpeedPlayer == 6 ? 0 : BuffSpeed; //Логика такова. Если скорость игрока в настроящий момент >= 5 то просто пооставим 6 если нет то прибавим баф. Таким образом мы не будем разгоняться до бесконечности

        if (!_playerBuffController.BuffIsExist(nameof(DefaultItemMoveSpeedUI)))
        {
            var _item = Instantiate(item, ItemsGrind);
            //_item.GetComponent<DefaultItemMoveSpeedUI>().RegisterOwner(this);
        }
        else
        {
            //var _findedItem = GameObject.FindObjectOfType<DefaultItemMoveSpeedUI>(); //Не проверено
            var _findedItem = ItemsGrind.FindChildObjectByType<DefaultItemMoveSpeedUI>(); //Не проверено
            if (_findedItem) _findedItem.GetComponent<DefaultItemMoveSpeedUI>().UpdateBuff();
        }

    }

    public void StopBuffMoveSpeed()
    {
        SpeedPlayer = _speedStart;
    }

    #endregion

    #region CommonDamage

    public void ChangeDamage(int BuffDamage, GameObject item)
    {
        //Debug.LogWarning("Damage Есть права " + hasAuthority);

        DamagePlayer += DamagePlayer >= 45 ? 0 : BuffDamage;

        if (!_playerBuffController.BuffIsExist(nameof(DefaultItemDamageUI)))
        {
            var _item = Instantiate(item, ItemsGrind);
            //_item.GetComponent<DefaultItemDamageUI>().RegisterOwner(this);
        }
        else
        {
            var _findedItem = ItemsGrind?.FindChildObjectByType<DefaultItemDamageUI>();
            if (_findedItem != null) _findedItem.GetComponent<DefaultItemDamageUI>().UpdateBuff();
        }
    }

    public void StopBuffDamage()
    {
        DamagePlayer = _damageStart;
    }
    #endregion

    #region CommonAmmo
    public void ChangeAmmo(float BuffAmmoReload, float BuffAmmoRate, GameObject item)
    {
        AmmoReload -= AmmoReload <= 1 ? 0 : BuffAmmoReload;
        BuletRate -= BuletRate <= 0.1f ? 0 : BuffAmmoRate;

        if (!_playerBuffController.BuffIsExist(nameof(DefaultItemAmmoUI)))
        {
            var _item = Instantiate(item, ItemsGrind);
            //_item.GetComponent<DefaultItemAmmoUI>().RegisterOwner(this);
        }
        else
        {
            var _findedItem = ItemsGrind?.FindChildObjectByType<DefaultItemAmmoUI>();
            if (_findedItem != null) _findedItem.GetComponent<DefaultItemAmmoUI>().UpdateBuff();
        }
    }

    public void StopBuffAmmo()
    {
        AmmoReload = _startAmmoReload;
        BuletRate = _startRateBulet;
    }

    #endregion

    #endregion

    #region Rare

    #region RareBullet

    public void ChangeBullet(float BuffBullet, GameObject item)
    {
        SizeBullet += SizeBullet == 0.5f ? 0 : BuffBullet;

        if (!_playerBuffController.BuffIsExist(nameof(RareItemBulletUI)))
        {
            var _item = Instantiate(item, ItemsGrind);
            //_item.GetComponent<RareItemBulletUI>().RegisterOwner(this);
        }

        //Debug.LogWarning("Нету прав");
        //if (hasAuthority)
        //{
        //    Debug.LogWarning("Есть права");
        //    SizeBullet += BuffBullet;
        //}
        //TargetChangeBullet(BuffBullet);
    }


    #endregion

    #endregion

    #endregion

    #region Public

    /// <summary>
    /// Показывает статистику персонажа
    /// </summary>
    [Client]
    public void ShowStat()
    {
        ProgressPlayerStat.SetStatPlayerText(AmmoWasted, EnemyKilled, BuffGive, ScorePlayer);

        ProgressPlayerStat.gameObject.SetActive(true);
    }

    /// <summary>
    /// Устанавливаем, можно ли управлять персонажем
    /// </summary>
    /// <param name="input"></param>
    public void InputIsActive(bool input) => InputActive = input;

    /// <summary>
    /// Проверяет, можно ли управлять персонажем
    /// </summary>
    /// <returns></returns>
    internal bool GetInputActive() => InputActive;

    /// <summary>
    /// Устанавливаем, можно ли открывать элементы UI игрока
    /// </summary>
    /// <param name="input"></param>
    public void MenuInputIsActive(bool input) => EscapeMenuActive = input;

    /// <summary>
    /// Устанавливает свойство мёртв игрок или нет
    /// </summary>
    /// <param name="setDead"></param>
    public void SetPlayerDead(bool setDead) { Debug.LogWarning("Ставим переменную bool о том что персонаж умер"); }

    public bool GetPlayerDead() => _player;

    /// <summary>
    /// Проверяет, можно ли открывать элементы UI игрока
    /// </summary>
    /// <returns></returns>
    internal bool GetMenuInputActive() => EscapeMenuActive;

    [ClientRpc]
    public void ChangeWaveNuberText(string waveText)
    {
        WaveText.text = waveText;
    }

    #endregion

    #region Override TakeDamage


    //Если переопределить методы, то будет срабатывать при -ХП но будет работать камера
    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);
        //Debug.LogWarning($"Я {connectionToClient} и моё здоровье {_SyncHealth}"); //на сервере отслеживание происходит хорошо, на клиенте нету connectionToClient 
    }



    #endregion

    #region Camera change after Die

    public void TargetChangeCameraToLivePlayer()
    {
        if (isLocalPlayer)
            StartCoroutine(ChangeCameraToLiveParty());

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

        //yield return new WaitForSeconds(3.0f);
        //GetComponentsInChildren<Collider>().ToList().ForEach(col => { DestroyImmediate(col); });
    }



    #endregion

    #region Ragdoll

    public enum DeathBy
    {
        Animation,
        AnimationWithRagdoll,
        Ragdoll
    }

    public void EnableRagdoll()
    {
        throw new System.NotImplementedException();
    }

    public void ResetRagdoll()
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator SetTriggerRoutine(int trigger)
    {
        animator.SetTrigger(trigger);
        yield return new WaitForSeconds(0.1f);
        animator.ResetTrigger(trigger);
    }

    public virtual void SetTrigger(int trigger)
    {
        StartCoroutine(SetTriggerRoutine(trigger));
    }

    protected virtual void TriggerDamageReaction(Damage damage)
    {
        if (animator != null && animator.enabled && !damage.activeRagdoll && currentHealth > 0)
        {
            if (hitDirectionHash.isValid && damage.sender) animator.SetInteger(hitDirectionHash, (int)transform.HitAngle(damage.sender.position));

            // trigger hitReaction animation
            if (damage.hitReaction)
            {
                // set the ID of the reaction based on the attack animation state of the attacker - Check the MeleeAttackBehaviour script
                if (reactionIDHash.isValid) animator.SetInteger(reactionIDHash, damage.reaction_id);
                if (triggerReactionHash.isValid) SetTrigger(triggerReactionHash);
                if (triggerResetStateHash.isValid) SetTrigger(triggerResetStateHash);
            }
            else
            {
                if (recoilIDHash.isValid) animator.SetInteger(recoilIDHash, damage.recoil_id);
                if (triggerRecoilHash.isValid) SetTrigger(triggerRecoilHash);
                if (triggerResetStateHash.isValid) SetTrigger(triggerResetStateHash);
            }
        }
        if (damage.activeRagdoll)
            onActiveRagdoll.Invoke(damage);
    }

    #endregion

    #region ScorePlayer

    void UpdateLocalScore(int oldScore, int newScore)
    {
        TextScorePlayer.text = newScore.ToString();
    }

    public void UpdateStat(int Health, int Damage, int Catriges, int Speed, float Reload)
    {
        if (isServer) return;

        ClientServerChangeHp(Health);
        MaxHealth = Health;
        _healthSlider.maxValue = Health / 100;
        _healthSliderRpc.maxValue = Health / 100;
        LocalShowHP(Health);
        DamagePlayer = Damage;
        MaxCartridges = Catriges;
        SpeedPlayer = Speed;
        AmmoReload = Reload;

        _speedStart = Speed;
        _damageStart = Damage;
        _startAmmoReload = Reload;

        if (PlayerPrefs.HasKey("TypePilotStat")) PlayerPrefs.DeleteKey("TypePilotStat");

        PlayerPrefs.SetString("TypePilotStat", string.Join(",", new string[] { $"{Health}", $"{Damage}", $"{Catriges}", $"{Speed}", $"{Reload}" }));

        GetComponent<PlayerProjectileSpawnerNetwork>().GetCatridges();
    }
    #endregion
}
