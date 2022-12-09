using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Cosmo;
using Mirror;
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

    [Tooltip("Задержка в расстоянии, нужно чтобы НПЦ начанал атаку раньше")]
    public float DelayDistance;

    public float TimeToCharge;

    [HideInInspector]
    public bool canSeePlayer;
    public bool Attacked;

    private bool chardge;
    private bool beginCharge;
    public float TimeChardge;

    private bool canAttack = false;
    private PlayerData weakPlayer;

    private EnemyAnimation e_anim;

    [SerializeField, Tooltip("Как в оффлайн так и в Runtime")] internal bool isShowGizmos = false;

    [SerializeField] private Damage damage;
    [SerializeField] private NetworkDamageTrigger damageTrigger;
    [SerializeField] private float _timeAttack;

    private float currentTimeAttack;

    #endregion

    #region Base method. Start, Awake, Enable and too...

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override bool isDead { get; set; }

    public virtual void OnStart()
    {
        agent = GetComponent<NavMeshAgent>();
        e_anim = GetComponent<EnemyAnimation>();
        StartCoroutine(FOVRoutine());

    }

    public virtual void Update()
    {
        Animation();
        if (Attacked & !isDead)
        {
            if(typeEnemy != TypeEnemy.RangerBot)
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
                if(currentTimeAttack >= _timeAttack)
                {
                    currentTimeAttack = 0;
                    Attacked = false;
                    agent.isStopped = false;
                    CmdSpawnBullet();
                }
            }
        }

        if (beginCharge && !chardge)
        {
            agent.isStopped = true;
            TimeToCharge -= Time.deltaTime;

            if (TimeToCharge <= 0)
            {
                agent.isStopped = false;
                chardge = true;
                agent.speed += 3;
                beginCharge = false;
                DelayDistance = 0.5f;
                TimeChardge = 1.5f;
            }
        }
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

    #region Поведение Врага

    public virtual void FieldOfViewCheck()
    {
        if(isDead) return;
        
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, AttackLayer);

        //Мы постоянно смотрим по радиусу. Если в нашем обзоре есть коллайдеры с именем игрок идём по условию
        if (rangeChecks.Length != 0)
        {
            for(int i = 0; i <= rangeChecks.Length; i++)
            {
                if (weakPlayer == null) weakPlayer = rangeChecks[i + 1].GetComponent<PlayerData>();
                else
                {
                    if(weakPlayer.currentHealth > rangeChecks[i + 1].GetComponent<PlayerData>().currentHealth) weakPlayer = rangeChecks[i + 1].GetComponent<PlayerData>();
                }
            }

            //foreach (var collider in rangeChecks)
            //{
                //TODO : Проверить ХП каждого и выявить слабого

                Transform target = weakPlayer.transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(Eyes.transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(Eyes.transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        if (distanceToTarget >= agent.stoppingDistance + DelayDistance)
                        {
                            //agent.isStopped = false;
                            agent.SetDestination(weakPlayer.transform.position);
                            canAttack = false;
                        }
                        else
                        {
                            if(typeEnemy == TypeEnemy.BigMeleeFighter)
                            {
                                canAttack = true;
                                if (!Attacked)
                                {
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
        }
        else if (canSeePlayer)
            canSeePlayer = false;

        #region Заставить пауков ходить по карте SyncVar?
        //else //Если в обзоре нет ниодного игрока, просто следуем рандомной точке
        //{
        //    // Choose the next point in the array as the destination,
        //        // cycling to the start if necessary.
        //        //var pointPatrol = randomPoints[Random.Range(0, randomPoints.Count)]; //Выберем одну из точек
        //        //float distanceToTarget =
        //        //    Vector3.Distance(transform.position, pointPatrol); //Проверим дистанцию до рандомной точки
        //        //if (distanceToTarget > 1f) //Если до нужной точки не дошли
        //        //    agent.SetDestination(pointPatrol); //Идём к рандомной  точке
        //    
        //}
        #endregion
    }

    #endregion

    #region Animation behaviour

    //TODO : Всё равно ошибкка при анимации agent.hasPath
    //[ClientCallback] //Незачем выполнять это на сервере 
    private void Animation()
    {
        if(isDead) return;
        if (agent == null) return;

        e_anim.anim_Walk(agent.hasPath);
        e_anim.anim_Attack(Attacked, Random.Range(1, 2));

    }

    #endregion

    enum TypeEnemy
    {
        LittleMeleeFighter, BigMeleeFighter, RangerBot
    }

    [Command(requiresAuthority = false)] //позволяет локальному проигрывателю удаленно вызывать эту функцию на серверной копии объекта
    public void CmdSpawnBullet()
    {
        var bullet = Instantiate(_bullet.gameObject, _spawnPoint.position, _spawnPoint.rotation); //Создаем локальный объект пули
        NetworkServer.Spawn(bullet); //отправляем информацию о сетевом объекте всем игрокам.
        //RpcSpawnBullet();
    }
}