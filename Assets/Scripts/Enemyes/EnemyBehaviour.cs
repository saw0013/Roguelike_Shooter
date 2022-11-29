using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
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
    public bool Attacked;

    private bool canAttack;

    private EnemyAnimation e_anim;

    [SerializeField, Tooltip("Как в оффлайн так и в Runtime")] internal bool isShowGizmos = false;

    [SerializeField] private Damage damage;
    [SerializeField] private NetworkDamageTrigger damageTrigger;
    [SerializeField] private float _timeAttack;

    private float currentTimeAttack;

    #endregion

    #region Base method. Start, Awake, Enable and too...


    public virtual void OnStart()
    {
        agent = GetComponent<NavMeshAgent>();
        e_anim = GetComponent<EnemyAnimation>();
        StartCoroutine(FOVRoutine());

    }

    public virtual void Update()
    {
        Animation();
        if (Attacked)
        {
            currentTimeAttack += Time.deltaTime;
            if (currentTimeAttack >= _timeAttack)
            {
                currentTimeAttack = 0;
                damageTrigger.AttackNum = 0;
                Attacked = false;
                agent.isStopped = false;
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
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, AttackLayer);

        //Мы постоянно смотрим по радиусу. Если в нашем обзоре есть коллайдеры с именем игрок идём по условию
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
                        if (distanceToTarget >= agent.stoppingDistance + 0.5f)
                        {
                            //agent.isStopped = false;
                            agent.SetDestination(collider.transform.position);
                            canAttack = false;
                        }
                        else
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

    private void Animation()
    {
        e_anim.anim_Walk(agent.hasPath);
        e_anim.anim_Attack(Attacked, Random.Range(1, 2));
    }

    #endregion
}