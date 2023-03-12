using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mirror;
using Mono.CSharp;
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
    //public static void ApplyDamage(this GameObject receiver, Damage damage)
    //{
    //    var receivers = receiver.GetComponents<PlayerData>();
    //    if (receivers != null)
    //        for (int i = 0; i < receivers.Length; i++)
    //            receivers[i].TakeDamage(damage);
    //}
    #endregion

    public static void ChangeScorePlayer<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey player, TValue score)
    {
        if (dict == null) dict = new Dictionary<TKey, TValue>();

        if (!dict.ContainsKey(player))
            dict.Add(player, score);
        else dict[player] = score;
    }

    #region List


    #endregion

    #endregion

    #region Transform

    /// <summary>
    /// Метод расширения который будет искать в дочерних объектах нужный компонент
    /// <para>Если дочерних компонентов нет, будет уведомление ошибки и возврат NULL</para>
    /// </summary>
    /// <typeparam name="T">class</typeparam>
    /// <param name="t">Transform</param>
    /// <returns>class</returns>
    public static T FindChildObjectByType<T>(this Transform t) where T : class
    {
        if (t.childCount <= 0)
        {
            Debug.LogError(t + " не имеет дочерних объектов");
            return null;
        }

        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).TryGetComponent<T>(out T _component) && _component != null)
            {
                return _component;
            }
        }

        return null;
    }

    /// <summary>
    /// Получит <see cref="NetworkConnection"/> у игрового объекта
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static NetworkConnection ObjectNetworkConnection(this GameObject obj)
    {
        NetworkIdentity netId;
        if (obj.TryGetComponent<NetworkIdentity>(out netId))
            return netId.connectionToClient;
        else return null;

    }

    public static T GetComponentOrDefault<T>(this GameObject obj) where T : Component
    {

        T component = obj.GetComponent<T>();
        return component != null ? component : default(T);
    }

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

    #region Utils

    /// <summary>
    /// Сохраняет по-умолчанию в папку Replayes
    /// <para>Для указании нужной директории нужно указать необязательный параметр</para>
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetFilePath(string fileName, string folderName = "Replays")
    {
        string path = Application.dataPath;
        path = Path.Combine(Path.GetDirectoryName(path), folderName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return Path.Combine(path, fileName);
    }

    #endregion
}
