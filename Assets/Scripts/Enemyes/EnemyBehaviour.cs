using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    [Header("Draw Eye")]
    [SerializeField] private bool showGizmos;
    [SerializeField, Range(0, 25)] private float radius = 5f;
    [SerializeField, Range(1, 100)]
    public float headDetectHeight = 5f;

    [Header("Layers")]
    [SerializeField] LayerMask AttackLayer;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Atack()) Debug.Log("ATAAAACK!!!");
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        transform.EnemyAttackToPlayer(radius, AttackLayer, headDetectHeight);
    }

    #region Поведение МОБА

    /// <summary>
    /// Проверяет можно ли атаковать? Если да то идём к Player
    /// <para>Копия для Debug <see cref="Extensions.EnemyAttackToPlayer(Transform, float, LayerMask, float)"/></para>
    /// </summary>
    /// <returns></returns>
    bool Atack()
    {
        Matrix4x4 oldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(1, 0.02f, 1));
        Vector3 pos = transform.position + Vector3.up * 0.4f;
        Ray ray2 = new Ray(pos, Vector3.up);

        if (Physics.SphereCast(ray2, radius,(headDetectHeight + (radius)), AttackLayer))
        {
            var colliders = Physics.OverlapSphere(transform.position, radius, AttackLayer);
            Debug.Log(colliders.Length);
            //foreach(var collider in colliders)
            //{
                //TODO : СДелать проеврку у кого меньше ХП
                //var Health = collider.GetComponent<PlayerData>()._SyncHealth;
                //if(Health > -1)
                //{
                Debug.Log(transform.name);
                    agent.SetDestination(transform.position);
                //}
            //}
            return true; 
        }
        else
            return false;
    }



    #endregion
}