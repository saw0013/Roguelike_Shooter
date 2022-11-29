using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class GetPointPatrool : NetworkBehaviour
{
    public static GetPointPatrool Instance;
    public float Range;
    [SerializeField] private Color color;

    private void Awake()
    {
        Instance = this;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;

        return false;
    }

    /// <summary>
    /// Vector3 ��������� ����� �� NavMesh. ����� ������ ������ ��� <see cref="RandomPoint"/>
    /// </summary>
    /// <param name="point">�������� �����</param>
    /// <returns>���������� transform</returns>
    public Vector3 GetRandomPoint(Transform point = null, float radius = 0)
    {
        Vector3 _point;

        if (RandomPoint(point == null ? transform.position : point.position, radius == 0 ? Range : radius, out _point))
        {
            Debug.DrawRay(_point, Vector3.up, Color.white, 5);

            return _point;
        }

        return point == null ? Vector3.zero : point.position;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, Range);
    }

#endif

}