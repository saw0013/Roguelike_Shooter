using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class GetPointPatrool : MonoBehaviour
{
    public static GetPointPatrool Instance;
    public float Range;
    [SerializeField]
    private Color color;

    [SerializeField] internal List<Vector3> RandomPoints = new List<Vector3>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AddRandomPointState1();
    }

    void AddRandomPointState1()
    {
        for (int i = 0; i < 50;)
        {
            var p = GetRandomPoint();
            if (p != Vector3.zero)
            {
                RandomPoints.Add(p);
                i++;
            }
        }
    }

    public Vector3 GetPointToRunOnPlayer(Vector3 point = new Vector3())
    {
        bool pointSucsses = false;
       
        while (!pointSucsses)
        {
            point = GetRandomPoint();

            if(point != Vector3.zero)
            {
                var sphereDistance = Physics.OverlapSphere(point, 4, LayerMask.NameToLayer("Player"));

                if(sphereDistance.Length == 0)
                {
                    pointSucsses = true;
                }       
            }
        }

        return point;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        for (int i = 0; i < 40; i++)
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
    /// Vector3 рандомная точка на NavMesh. Метод поиска смотри тут <see cref="RandomPoint"/>
    /// </summary>
    /// <param name="point">Входящая точка</param>
    /// <returns>Возвращаем transform</returns>
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