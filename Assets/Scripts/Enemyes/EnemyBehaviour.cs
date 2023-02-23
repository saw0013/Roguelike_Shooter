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

    [Tooltip("Скорость анимации ходьбы паука")]
    [SerializeField] private float SpeedWalkAnim;

    [Tooltip("Задержка в расстоянии, нужно чтобы НПЦ начанал атаку раньше")]
    public float DelayDistance;

    public float TimeToCharge;
    [Tooltip("Время, при исходе которого враг ищет слабую цель")]
    public float TimeFindWeak;

    [HideInInspector]
    public bool canSeePlayer;
    public bool Attacked;

    private bool chardge;
    private bool beginCharge;
    public bool enemyRun;
    public float TimeChardge;

    private bool canAttack = false;
    //private PlayerData weakPlayer;

    [Tooltip("Цель")]
    public Collider purpose; //Ну почему pupose а не Target?

    private EnemyAnimation e_anim;

    [SerializeField, Tooltip("Как в оффлайн так и в Runtime")] internal bool isShowGizmos = false;

    [SerializeField] private Damage damage;
    [SerializeField] private NetworkDamageTrigger damageTrigger;
    [SerializeField] private float _timeAttack;

    private float currentTimeAttack;

    private bool enable = true;

    private Renderer _renderer;
    private float alphaRenderer = 0.2f;

    [SerializeField] private NetworkAnimator netAnim;



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
        Debug.LogWarning("Враг LocalDead " + LocalDead);
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
                        SpawnBullet();
                        currentTimeAttack = 0;
                        Attacked = false;
                        agent.isStopped = false;
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

    #region Поведение Врага

    public Collider CheckAround()
    {

        //Мне кажИтся что немного дублируется код?
        if (purpose != null)
        {
            var _purpose = purpose?.GetComponent<PlayerData>();
            if (_purpose._SyncHealth < 0)
            {
                agent.isStopped = true;
                canAttack = false;
            }
        }

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, AttackLayer);
        if (rangeChecks.Length > 0)
        {
            for (int i = 0; i < rangeChecks.Length; i++)
            {
                if (rangeChecks[i].CompareTag("Player")) //Будем знать точно что перед нами игрок и Если у игрока больше 0 ХП значит он жив)
                {
                    var player = rangeChecks[i].GetComponent<PlayerData>();

                    if (player.GetComponent<PlayerData>()._SyncHealth > 0)
                    {
                        //Debug.LogWarning($"игрок {player.connectionToClient} currentHealth = {player.currentHealth}\r\n SyncHealth={player._SyncHealth}");
                        var distanceCheck = Vector3.Distance(gameObject.transform.position, rangeChecks[i].transform.position);

                        if (purpose == null && player._SyncHealth > 0) purpose = rangeChecks[i];

                        if (purpose != null && player._SyncHealth > 0)
                        {
                            var HealthPuproce = purpose.GetComponent<PlayerData>();

                            var distancePuproce = Vector3.Distance(gameObject.transform.position, purpose.transform.position); //Puproce - польский. Ты ФАШИСТ??? xD

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
        if (typeEnemy != TypeEnemy.RangerBot)
        {
            transform.LookAt(target);
        }
        else
        {
            Vector3 delayDistance = new Vector3(target.position.x + 0.5f, target.position.y, target.position.z + 0.5f);
            transform.LookAt(delayDistance);
        }
    }

    public virtual void FieldOfViewCheck()
    {
        //if (isDead) return;

        var checkPlayer = CheckAround();

        if (typeEnemy == TypeEnemy.RangerBot)
        {
            if (Vector3.Distance(transform.position, purpose.transform.position) < agent.stoppingDistance - 3 && !enemyRun) //Если дистанции до игрока меньше 2м и бот не бежит
            {
                enemyRun = true; //Беги

                Vector3 directionToTarget = (transform.position - purpose.transform.position);
                agent.stoppingDistance = 0.1f;

                NavMeshHit hit;
                // проверяем, можно ли пройти в этом направлении
                if (NavMesh.SamplePosition(transform.position + directionToTarget, out hit, 15.0f, NavMesh.AllAreas))
                {
                    // устанавливаем новую позицию для агента
                    agent.SetDestination(hit.position);
                }

            }
            else if (agent.remainingDistance <= agent.stoppingDistance && enemyRun) //
            {
                agent.stoppingDistance = 5f;
                enemyRun = false;
            }
        }

        //Мы постоянно смотрим по радиусу. Если в нашем обзоре есть коллайдеры с именем игрок идём по условию
        if (checkPlayer != null && !enemyRun)
        {
            //TODO : Проверить ХП каждого и выявить слабого
            
            Transform target = purpose.transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(Eyes.transform.forward, directionToTarget) < angle / 2 || purpose != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(Eyes.transform.position, directionToTarget, distanceToTarget, obstructionMask) || purpose != null)
                {
                    if (purpose != null)
                    {
                        agent.SetDestination(target.position);
                        canAttack = false;
                        if (distanceToTarget < agent.stoppingDistance + DelayDistance)
                        {
                            #region Условие атаки большого паука
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
                            #endregion

                            #region Условие атаки Маленького паука
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

                            #endregion

                            #region Условие атаки стреляющего 

                            else if (typeEnemy == TypeEnemy.RangerBot)
                            {
                                Debug.LogWarning("Зашли в условие атаки Бота");
                                if (!Attacked)
                                {
                                    LookTarget(target);
                                    Attacked = true;
                                    agent.isStopped = true;
                                }
                            }

                            #endregion
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
        if (isDead || agent == null) return;

        if (typeEnemy != TypeEnemy.RangerBot)
        {
            e_anim.anim_WalkSpider(agent.hasPath);
            e_anim.anim_Attack(Attacked, UnityEngine.Random.Range(1, 2));
        }
        else if (typeEnemy == TypeEnemy.RangerBot)
        {
            e_anim.anim_WalkSolider(agent.velocity);
        }

    }

    #endregion

    enum TypeEnemy
    {
        LittleMeleeFighter, BigMeleeFighter, RangerBot
    }


    public void SpawnBullet()
    {
        //var bullet = Instantiate(_bullet.gameObject, _spawnPoint.position, _spawnPoint.rotation); //Создаем локальный объект пули
        //bullet.GetComponent<BulletPool>().DamageToPlayer.damageValue = damage.damageValue;
        //bullet.GetComponent<NetworkMatch>().matchId = this.GetComponent<NetworkMatch>().matchId;
        //bullet.GetComponent<BulletPool>().Init(gameObject);
        //NetworkServer.Spawn(bullet); //отправляем информацию о сетевом объекте всем игрокам.
        RpcSpawnBullet();
    }

    [ClientRpc]
    void RpcSpawnBullet()
    {
        var bullet = Instantiate(_bullet.gameObject, _spawnPoint.position, _spawnPoint.rotation); //Создаем локальный объект пули
        bullet.GetComponent<BulletPool>().DamageToPlayer.damageValue = damage.damageValue;
        //bullet.GetComponent<NetworkMatch>().matchId = this.GetComponent<NetworkMatch>().matchId;
        //bullet.GetComponent<BulletPool>().Init(gameObject);
    }
}