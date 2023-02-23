using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class GetPointPatrool : MonoBehaviour
{
    public static GetPointPatrool Instance;
    public float Range;
    [SerializeField]
    private UnityEngine.Color color;

    [SerializeField] internal List<Vector3> RandomPoints  = new List<Vector3>();
    [SerializeField] internal List<Vector3> PointToPlayer = new List<Vector3>();

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

    public Vector3 GetPointToRunOnPlayer(Transform enemy)
    {
        if(PointToPlayer.Count > 20) PointToPlayer.Clear();

        for (int i = 0; i < 20;)
        {
            var p = GetRandomPoint(enemy, 15f);
            if (p != Vector3.zero)
            {
                PointToPlayer.Add(p);
                i++;
            }
        }

        //Просто оставлю это тут https://www.youtube.com/watch?v=u4m61AWxKmE&ab_channel=GGJNEXT

        List<Collider> findPointWhereStayPlayer;
        int indexPoint = 0;

        for (int i = 0; i < PointToPlayer.Count; i++)
        {
            indexPoint = i; //Будем знать какие координаты мы проверяем

            Debug.LogWarning("Координата=" + PointToPlayer[i] + " мы её проверяем");

            findPointWhereStayPlayer = Physics.OverlapSphere(PointToPlayer[i], 10, LayerMask.NameToLayer("Player")).ToList();

            if (findPointWhereStayPlayer.Count == 0) break; //Прервём цикл если в радиусе нашей точки не найден ниодин игрок
        }

        return PointToPlayer[indexPoint];

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