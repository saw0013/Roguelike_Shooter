using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions 
{
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
}
