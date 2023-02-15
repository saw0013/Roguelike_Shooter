using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Cosmo;
using Mirror;
using MirrorBasics;
using Mono.CSharp;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : HealthController
{
    #region  Variables
    [SerializeField] private TypeEnemy typeEnemy;

    [Header("Draw Eye")]
    [SerializeField, Range(0, 25)] internal float radius = 5f;
    [Range(0, 360)] public float angle;
    [SerializeField] internal GameObject Eyes;

    [Header("Layers")]
    [SerializeField] LayerMask AttackLayer;
    public LayerMask obstructionMask;

    private NavMeshAgent agent;

    [Header("EnemyOptions")]

    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _bullet;

    public bool LocalDead = false;

    [Tooltip("�������� �������� ������ �����")]
    [SerializeField] private float SpeedWalkAnim;

    [Tooltip("�������� � ����������, ����� ����� ��� ������� ����� ������")]
    public float DelayDistance;

    public float TimeToCharge;
    [Tooltip("�����, ��� ������ �������� ���� ���� ������ ����")]
    public float TimeFindWeak;

    [HideInInspector]
    public bool canSeePlayer;
    public bool Attacked;

    private bool chardge;
    private bool beginCharge;
    private float currentTimeFindeWeak;
    public float TimeChardge;

    private bool canAttack = false;
    //private PlayerData weakPlayer;

    [Tooltip("����")]
    private Collider purpose;

    private EnemyAnimation e_anim;

    [SerializeField, Tooltip("��� � ������� ��� � � Runtime")] internal bool isShowGizmos = false;

    [SerializeField] private Damage damage;
    [SerializeField] private NetworkDamageTrigger damageTrigger;
    [SerializeField] private float _timeAttack;

    private float currentTimeAttack;

    private bool enable = true;

    private Renderer _renderer;
    private float alphaRenderer = 0.2f;



    #endregion

    #region Base method. Start, Awake, Enable and too...

    //private void OnEnable() => Action_OnDead += CmdDie;
    //private void OnDisable() => Action_OnDead -= CmdDie;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    public virtual void OnStart()
    {
        agent = GetComponent<NavMeshAgent>();
        e_anim = GetComponent<EnemyAnimation>();
        StartCoroutine(FOVRoutine());
        e_anim.anim_WalkSpeed(SpeedWalkAnim);
        _renderer = GetComponent<Renderer>();
    }

    public void CmdLocalDead()
    {
        LocalDead = true;
        Debug.LogWarning("���� LocalDead " + LocalDead);
    }


    public virtual void Update()
    {
        //currentTimeFindeWeak += Time.deltaTime;

        //if(currentTimeFindeWeak >= TimeFindWeak)
        //{
        //    var checkPlayer = CheckAround();
        //    currentTimeFindeWeak = 0;
        //    if(checkPlayer != null)
        //    {
        //        for (int i = 0; i < checkPlayer.Length; i++)
        //        {
        //            if (weakPlayer == null)
        //                weakPlayer = checkPlayer[i].GetComponent<PlayerData>();
        //            else
        //            {
        //                if (weakPlayer.currentHealth > checkPlayer[i].GetComponent<PlayerData>().currentHealth)
        //                    weakPlayer = checkPlayer[i].GetComponent<PlayerData>();
        //            }
        //        }
        //    }
        //}

        if (_SyncHealth > 1 && enable)
        {
            if (Attacked)
            {
                if (typeEnemy != TypeEnemy.RangerBot)
                {
                    currentTimeAttack += Time.deltaTime;
                    if (currentTimeAttack >= _timeAttack)
                    {
                        currentTimeAttack = 0;
                        damageTrigger.AttackNum = 0;
                        Attacked = false;
                        agent.isStopped = false;
                        chardge = false;
                    }
                }
                else
                {
                    currentTimeAttack += Time.deltaTime;
                    if (currentTimeAttack >= _timeAttack)
                    {
                        currentTimeAttack = 0;
                        Attacked = false;
                        agent.isStopped = false;
                        CmdSpawnBullet();
                    }
                }
            }
        }
        else
        {
            agent.isStopped = true;
            enable = false;
        }

        if (beginCharge && !chardge)
        {
            agent.isStopped = true;
            TimeToCharge -= Time.deltaTime;

            if (TimeToCharge <= 0)
            {
                e_anim.anim_WalkSpeed(SpeedWalkAnim + 5);
                agent.isStopped = false;
                chardge = true;
                agent.speed += 3;
                beginCharge = false;
                DelayDistance = 0.5f;
                TimeChardge = 1.5f;
            }
        }

        Animation();
    }

#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
    void OnDrawGizmos()
    {
        if (isShowGizmos)
            this.ShowGizmos();
    }
#endif

    #endregion

    #region IEnumerators

    public virtual IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    #endregion

    #region ��������� �����

    public Collider CheckAround()
    {
        if(purpose != null)
        {
            var _purpose = purpose?.GetComponent<PlayerData>();

            Debug.LogWarning("���� ����");

            if (_purpose._SyncHealth < 0)
            {
                Debug.LogWarning("� ���� ������ 0 ��");
                agent.isStopped = true;
                canAttack = false;
            }
        }
        
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, AttackLayer);
        if (rangeChecks.Length > 0)
        {
            for (int i = 0; i < rangeChecks.Length; i++)
            {
                if (rangeChecks[i].CompareTag("Player")) //����� ����� ����� ��� ����� ���� ����� � ���� � ������ ������ 0 �� ������ �� ���)
                {
                    var player = rangeChecks[i].GetComponent<PlayerData>();

                    if (player.GetComponent<PlayerData>()._SyncHealth > 0)
                    {
                        //Debug.LogWarning($"����� {player.connectionToClient} currentHealth = {player.currentHealth}\r\n SyncHealth={player._SyncHealth}");
                        var distanceCheck = Vector3.Distance(gameObject.transform.position, rangeChecks[i].transform.position);

                        if (purpose == null && player._SyncHealth > 0) purpose = rangeChecks[i];

                        if (purpose != null && player._SyncHealth > 0)
                        {
                            Debug.LogWarning("���� ���� � � ������������ ������ ������ 0");

                            var HealthPuproce = purpose.GetComponent<PlayerData>();

                            var distancePuproce = Vector3.Distance(gameObject.transform.position, purpose.transform.position);

                            Debug.LogWarning($"HP Pupose {HealthPuproce._SyncHealth} , Distance puprose {distancePuproce},(>) Distance Check {distanceCheck}");

                            if (HealthPuproce._SyncHealth < 0 || distancePuproce > distanceCheck) purpose = rangeChecks[i];
                        }

                    }

                }

            }

            return purpose;
        }
        else return null;
    }

    private void LookTarget(Transform target)
    {
        transform.LookAt(target);
    }

    public virtual void FieldOfViewCheck()
    {
        //if (isDead) return;

        var checkPlayer = CheckAround();

        //�� ��������� ������� �� �������. ���� � ����� ������ ���� ���������� � ������ ����� ��� �� �������
        if (checkPlayer != null) 
        {
            //TODO : ��������� �� ������� � ������� �������

            Transform target = purpose.transform; //�������� ��������� ����� � CheckPlayer
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(Eyes.transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(Eyes.transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    if (distanceToTarget >= agent.stoppingDistance + DelayDistance)
                    {
                        //agent.isStopped = false;
                        agent.SetDestination(purpose.transform.position);
                        canAttack = false;
                    }
                    else
                    {
                       
                        if (typeEnemy == TypeEnemy.BigMeleeFighter)
                        {
                            canAttack = true;
                            
                            if (!Attacked)
                            {
                                LookTarget(target);
                                Attacked = true;
                                agent.isStopped = true;
                                damageTrigger.AttackNum = 1;
                            }
                            //StartCoroutine(damagePlayer(collider));
                            //collider.GetComponent<PlayerData>().TakeDamage(damage);
                        }
                        else if (typeEnemy == TypeEnemy.LittleMeleeFighter)
                        {
                            if (chardge)
                            {
                                if (!Attacked)
                                {
                                    LookTarget(target);
                                    e_anim.anim_WalkSpeed(SpeedWalkAnim);
                                    Attacked = true;
                                    agent.isStopped = true;
                                    damageTrigger.AttackNum = 1;
                                    DelayDistance = 1.5f;
                                }
                            }
                            else beginCharge = true;
                        }
                        else if (typeEnemy == TypeEnemy.RangerBot)
                        {
                            if (!Attacked)
                            {
                                LookTarget(target);
                                Attacked = true;
                                agent.isStopped = true;
                            }
                        }
                    }
                    canSeePlayer = true;
                }
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;

            //}
            //foreach (var collider in checkPlayer)
            //{

            //}
        }
        else if (canSeePlayer)
            canSeePlayer = false;

        #region ��������� ������ ������ �� ����� SyncVar?
        //else //���� � ������ ��� �������� ������, ������ ������� ��������� �����
        //{
        //    // Choose the next point in the array as the destination,
        //        // cycling to the start if necessary.
        //        //var pointPatrol = randomPoints[Random.Range(0, randomPoints.Count)]; //������� ���� �� �����
        //        //float distanceToTarget =
        //        //    Vector3.Distance(transform.position, pointPatrol); //�������� ��������� �� ��������� �����
        //        //if (distanceToTarget > 1f) //���� �� ������ ����� �� �����
        //        //    agent.SetDestination(pointPatrol); //��� � ���������  �����
        //    
        //}
        #endregion
    }

    #endregion

    #region Animation behaviour

    //TODO : �� ����� ������� ��� �������� agent.hasPath
    //[ClientCallback] //������� ��������� ��� �� ������� 
    private void Animation()
    {
        if (isDead || agent == null) return;

        e_anim.anim_Walk(agent.hasPath);
        e_anim.anim_Attack(Attacked, UnityEngine.Random.Range(1, 2));

    }

    #endregion

    enum TypeEnemy
    {
        LittleMeleeFighter, BigMeleeFighter, RangerBot
    }

    [Command(requiresAuthority = false)] //��������� ���������� ������������� �������� �������� ��� ������� �� ��������� ����� �������
    public void CmdSpawnBullet()
    {
        var bullet = Instantiate(_bullet.gameObject, _spawnPoint.position, _spawnPoint.rotation); //������� ��������� ������ ����
        NetworkServer.Spawn(bullet); //���������� ���������� � ������� ������� ���� �������.
        //RpcSpawnBullet();
    }
}