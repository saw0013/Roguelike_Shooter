using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cosmo;

public class PlayerData : HealthController, ICharacter
{
    #region Variables

    [Space(10), Header("===PlayerData===")]
    public bool InputActive = true;
    public bool EscapeMenuActive;

    public int SpeedPlayer;
    public float DamagePlayer;
    public float AmmoReload;
    public int BuletForce;
    private float guardPlayer;

    [SerializeField] private float _startAmmoReload;
    [SerializeField] private int _startForceBulet;
    [SerializeField] private float _guardStart;
    [SerializeField] private int _speedStart;
    [SerializeField] private float _damageStart;

    [SyncVar(hook = nameof(NameDisplay))]
    public string _nameDisplay;

    [Space(10), Header("===Ragdoll===")]
    public DeathBy deathBy = DeathBy.Animation;
    public bool removeComponentsAfterDie;
    [HideInInspector]
    public bool debugActionListener;

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
        AmmoReload = _startAmmoReload;
        BuletForce = _startForceBulet;
        guardPlayer = _guardStart;
        SpeedPlayer = _speedStart;

    }

    protected override void Start() => base.Start();

    public override void OnStartServer()
    {
        base.OnStartServer();
        //playerHealth = _maxHealth;
    }

    void Update()
    {
        if (hasAuthority)
        {
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
            //TODO : ������� ������� � ����� ������� � BulletPool OnCollisionEnter
            Debug.LogWarning($"�������� �� {other.GetComponent<BulletPool>().owner}");
        }

    }

    void OnCollisionEnter(Collision collision) { }

    #endregion

    #region Server Client Call ChangeHealth



    internal void BuffHealth(float maxHealth)
    {
        if (hasAuthority) //���� �� ����� ����� �� ���������
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
        if (hasAuthority)
        {
            guardPlayer += BuffGuard;
            _textGuard.text = $"���: {guardPlayer}";
        }
    }

    public void StopBuffGuard()
    {
        if (hasAuthority)
        {
            guardPlayer = _guardStart;
            _textGuard.text = $"���: {guardPlayer}";
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
        if (hasAuthority)
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

    #region Override TakeDamage

    public override void TakeDamage(Damage damage)
    {
        if (isLocalPlayer)
        {
            TriggerDamageReaction(damage);
            if (!isDead && currentHealth <= 0)
            {
                Debug.LogWarning("�� ��������� ����� ������� ����");
                isDead = true;
                onDead.Invoke(gameObject);
                InputActive = false;
                StartCoroutine(ChangeCameraToLiveParty());
            }
        }

        
        base.TakeDamage(damage);
        


    }

    #endregion

    #region Camera change after Die
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

    public void EnableRagdoll()
    {
        throw new System.NotImplementedException();
    }

    public void ResetRagdoll()
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region Ragdoll

    public enum DeathBy
    {
        Animation,
        AnimationWithRagdoll,
        Ragdoll
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
}
