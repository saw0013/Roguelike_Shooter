using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Mirror;
using Mono.CSharp;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : NetworkBehaviour

{

    #region  Variables

    [Header("Draw Eye")]
    [SerializeField, Range(0, 25)] internal float radius = 5f;
    [Range(0, 360)] public float angle;
    [SerializeField] internal GameObject Eyes;

    [Header("Layers")]
    [SerializeField] LayerMask AttackLayer;
    public LayerMask obstructionMask;

    private NavMeshAgent agent;

    [HideInInspector]
    public bool canSeePlayer;

    private bool canAttack;

    private EnemyAnimation e_anim;

    [SerializeField, Tooltip("Как в оффлайн так и в Runtime")] internal bool isShowGizmos = false;

    [SerializeField] private Damage damage;

    [HideInInspector]
    public Transform patroolPoints;

    private bool isPatrool;

    #endregion

    #region Base method. Start, Awake, Enable and too...

    public virtual void OnStart()
    {
        agent = GetComponent<NavMeshAgent>();
        e_anim = GetComponent<EnemyAnimation>();
        StartCoroutine(FOVRoutine());
        StartCoroutine(Patrool(isPatrool));
    }
    private void Start()
    {

    }

    public virtual void Update()
    {
        isPatrool = canSeePlayer;
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

    private IEnumerator Patrool(bool isPatrooling)
    {
        WaitForSeconds wait = new WaitForSeconds(1.0f);
        while (isPatrooling)
        {
            yield return wait;
            Patrool();
        }
    }

    private void Patrool()
    {
        var point = patroolPoints.GetChild(Random.Range(0, patroolPoints.childCount)); //Получаю рандомную парульную точку
        var distance = Vector3.Distance(transform.position , point.position); //Проверим дистанцию до выбранной точки

        //Если мы не видим игрока и не атакуем, есть путь к точке и расстояние до неё меньше 1м
        if (!canSeePlayer & !canAttack & distance < 1f & !agent.hasPath)
        {
            transform.LookAt(point.position);
            agent.SetDestination(point.position); //Перемещаемся к точку
        }
    }

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
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, AttackLayer);

        if (rangeChecks.Length != 0)
        {
            foreach (var collider in rangeChecks)
            {
                //TODO : Проверить ХП каждого и выявить слабого

                Transform target = collider.transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(Eyes.transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(Eyes.transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        if (distanceToTarget > 2.5f)
                        {
                            agent.isStopped = false;
                            agent.SetDestination(collider.transform.position);
                            canAttack = false;
                        }
                        else
                        {
                            canAttack = true;
                            agent.isStopped = true;
                            StartCoroutine(damagePlayer(collider));
                            //collider.GetComponent<PlayerData>().TakeDamage(damage);
                        }
                        canSeePlayer = true;
                    }
                    else
                        canSeePlayer = false;
                }
                else
                    canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }

    private IEnumerator damagePlayer(Collider collider)
    {
        yield return new WaitForEndOfFrame();

        while (canAttack)
        {
            var _damage = new Damage(damage);
            _damage.sender = transform;
            _damage.receiver = collider.transform;
            collider.gameObject.ApplyDamage(_damage);
            yield return new WaitForSeconds(3f);
        }
    }
    #endregion

    #region Animation behaviour

    private void Animation()
    {
        e_anim.anim_Walk(agent.hasPath);
        e_anim.anim_Attack(canAttack, Random.Range(1, 2));
    }

    #endregion
}