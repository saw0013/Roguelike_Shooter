using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmoground
{
    [System.Serializable]
    public class OnReceiveDamage : UnityEngine.Events.UnityEvent<Damage> { }

    public interface IDamageReceiver
    {
        OnReceiveDamage onStartReceiveDamage { get; }
        OnReceiveDamage onReceiveDamage { get; }
        Transform transform { get; }
        GameObject gameObject { get; }
        void TakeDamage(Damage damage);
    }

    public static class DamageHelper
    {
        /// <summary>
        /// Нанесите урон игровому объекту, если <see cref="CanReceiveDamage(GameObject)"/>
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="damage"></param>
        public static void ApplyDamage(this GameObject receiver, Damage damage)
        {
            var receivers = receiver.GetComponents<IDamageReceiver>();
            if (receivers != null)
                for (int i = 0; i < receivers.Length; i++)
                    receivers[i].TakeDamage(damage);
        }

        /// <summary>
        /// проверить, может ли gameObject получить урон
        /// </summary>
        /// <param name="receiver"></param>
        /// <returns>Вернёт true если gameObject имеет <see cref="IDamageReceiver"/></returns>
        public static bool CanReceiveDamage(this GameObject receiver)
        {
            return receiver.GetComponent<IDamageReceiver>() != null;
        }

        /// <summary>
        /// Получить угол между transform и hitpoint
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="hitpoint"></param>
        /// <param name="normalized"></param>
        /// <returns></returns>
        public static float HitAngle(this Transform transform, Vector3 hitpoint, bool normalized = true)
        {
            var localTarget = transform.InverseTransformPoint(hitpoint);
            var _angle = (int)(Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg);

            if (!normalized) return _angle;

            if (_angle <= 45 && _angle >= -45)
                _angle = 0;
            else if (_angle > 45 && _angle < 135)
                _angle = 90;
            else if (_angle >= 135 || _angle <= -135)
                _angle = 180;
            else if (_angle < -45 && _angle > -135)
                _angle = -90;

            return _angle;
        }
    }
}