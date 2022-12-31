using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmo;
using Mirror;
using TMPro;
using MirrorBasics;

public class PlayerData : HealthController, ICharacter
{
    #region Variables
    [SyncVar(hook = nameof(UpdateLocalScore))]
    public int ScorePlayer;

    [Header("===UI PlayerData===")]
    [SerializeField] private TMP_Text TextScorePlayer;

    [Space(10), Header("===PlayerData===")]
    private bool InputActive/* = true*/;
    private bool EscapeMenuActive;
    public Transform ItemsGrind;
    public int SpeedPlayer;
    public int DamagePlayer;
    public float AmmoReload;
    public int BuletForce;

    public float SizeBullet;

    public int guardPlayer;

    [SerializeField] private float _startAmmoReload;
    [SerializeField] private int _startForceBulet;
    [SerializeField] private int _guardStart;
    [SerializeField] private int _speedStart;
    [SerializeField] private int _damageStart;
    [SerializeField] private TMP_Text _textGuard;
    [SerializeField] internal TMP_Text _nameDisplayRpc;

    [SyncVar(hook = nameof(NameDisplay))]
    public string _nameDisplay;

    [Space(10), Header("===Ragdoll===")]
    public DeathBy deathBy = DeathBy.Animation;
    public bool removeComponentsAfterDie;
    [HideInInspector]
    public bool debugActionListener;

    private PlayerMovementAndLookNetwork _player;

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
        BuletForce = _startForceBulet;
        guardPlayer = _guardStart;
        SpeedPlayer = _speedStart;
    }

    protected override void Start()
    {
        _player = GetComponent<PlayerMovementAndLookNetwork>();
        base.Start();
    } 

    public override void OnStartServer()
    {
        base.OnStartServer();
        //playerHealth = _maxHealth;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            //TODO : Сделать счётчик и вести счётчик в BulletPool OnCollisionEnter
            //Debug.LogWarning($"Тимдамаг от {other.GetComponent<BulletPool>().owner}");
        }

    }

    void OnCollisionEnter(Collision collision) { }

    #endregion

    #region Server Client Call ChangeHealth



    internal void BuffHealth(float maxHealth)
    {
        if (hasAuthority) //Если мы имеем право на изменение
        {
            MaxHealth = (int)maxHealth;
            _healthSlider.maxValue = maxHealth / 100;
            _healthSliderRpc.maxValue = maxHealth / 100;
            ClientServerChangeHp(maxHealth);
            LocalShowHP(maxHealth);
        }
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

    #region CommonGuard

    public void ChangeGuard(int BuffGuard)
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


    public void ChangeDamage(int BuffDamage)
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

    #region Rare

    #region RareBullet

    public void ChangeBullet(float BuffBullet)
    {
        //Debug.LogWarning("Нету прав");
        //if (hasAuthority)
        //{
        //    Debug.LogWarning("Есть права");
        //    SizeBullet += BuffBullet;
        //}

        SizeBullet += BuffBullet;
    }

    #endregion

    #endregion

    #endregion

    #region Public

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
    /// Проверяет, можно ли открывать элементы UI игрока
    /// </summary>
    /// <returns></returns>
    internal bool GetMenuInputActive() => EscapeMenuActive;

    #endregion

    #region Override TakeDamage


    //Если переопределить методы, то будет срабатывать при -ХП но будет работать камера
    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);
    }

    #endregion

    #region Camera change after Die

    public void TargetChangeCameraToLivePlayer()
    {
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

 

    #endregion
}
