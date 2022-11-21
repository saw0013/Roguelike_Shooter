using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Extensions
{
    #region Vector3

    public static Vector3 AngleFormOtherDirection(this Vector3 directionA, Vector3 directionB)
    {
        return Quaternion.LookRotation(directionA).eulerAngles.AngleFormOtherEuler(Quaternion.LookRotation(directionB).eulerAngles);
    }

    public static Vector3 AngleFormOtherEuler(this Vector3 eulerA, Vector3 eulerB)
    {
        Vector3 angles = eulerA.NormalizeAngle().Difference(eulerB.NormalizeAngle()).NormalizeAngle();
        return angles;
    }

    /// <summary>
    /// Нормализация угла. от -180 до 180 градусов
    /// </summary>
    /// <param Name="eulerAngle">Угол Эулера.</param>
    public static Vector3 NormalizeAngle(this Vector3 eulerAngle)
    {
        var delta = eulerAngle;

        if (delta.x > 180)
        {
            delta.x -= 360;
        }
        else if (delta.x < -180)
        {
            delta.x += 360;
        }

        if (delta.y > 180)
        {
            delta.y -= 360;
        }
        else if (delta.y < -180)
        {
            delta.y += 360;
        }

        if (delta.z > 180)
        {
            delta.z -= 360;
        }
        else if (delta.z < -180)
        {
            delta.z += 360;
        }

        return new Vector3(delta.x, delta.y, delta.z);//round values to angle;
    }

    public static Vector3 Difference(this Vector3 vector, Vector3 otherVector)
    {
        return otherVector - vector;
    }

    #endregion

    #region GameObject

    #region TakeDamage
    public static void ApplyDamage(this GameObject receiver, Damage damage)
    {
        var receivers = receiver.GetComponents<PlayerData>();
        if (receivers != null)
            for (int i = 0; i < receivers.Length; i++)
                receivers[i].TakeDamage(damage);
    }
    #endregion

    #endregion

    #region Transform

    private const float GIZMO_DISK_THICKNESS = 0.02f;

    public static void EnemyAttackToPlayer(this Transform t, float radius, LayerMask AttackLayer, float headDetectHeight)
    {
        var color = Color.green;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, new Vector3(1, GIZMO_DISK_THICKNESS, 1));
        Vector3 pos = t.position + Vector3.forward * 0.4f;
        Ray ray2 = new Ray(pos, Vector3.up);

        if (Physics.SphereCast(ray2, radius, (headDetectHeight + (radius)), AttackLayer))
            color = Color.red;
        else
            color = Color.green;

        color.a = 0.4f;
        Gizmos.color = color;
        //Gizmos.DrawWireSphere(pos + Vector3.up * ((headDetectHeight + (radius))), radius);
        Gizmos.DrawSphere(Vector3.up * (headDetectHeight + (radius)), radius);
        Gizmos.matrix = oldMatrix;
    }

    #if UNITY_EDITOR 
    public static void ShowGizmos(this EnemyBehaviour eb)
    {
        EnemyBehaviour fov = eb;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.Eyes.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);
        

        Vector3 viewAngle01 = DirectionFromAngle(fov.Eyes.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.Eyes.transform.eulerAngles.y, fov.angle / 2);

        Handles.color = Color.red;
        Handles.DrawLine(fov.Eyes.transform.position, fov.Eyes.transform.position + viewAngle01 * fov.radius);
        Handles.DrawLine(fov.Eyes.transform.position, fov.Eyes.transform.position + viewAngle02 * fov.radius);

        if (fov.canSeePlayer)
        {
            Handles.color = Color.green;
            //TODO : Draw to player
            //Handles.DrawLine(fov.Eyes.transform.position, fov.playerRef.transform.position);
        }
    }
    private static Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endif

#endregion
}
