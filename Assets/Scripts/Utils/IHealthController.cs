using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmoground
{
    [System.Serializable]
    public class OnDead : UnityEngine.Events.UnityEvent<GameObject> { }

    public interface IHealthController : IDamageReceiver
    {
        /// <summary>
        /// Event вызывается когда <seealso cref="currentHealth"/>  равно нулю или меньше
        /// </summary>
        OnDead onDead { get; }
        /// <summary>
        /// Текущее значение здоровья
        /// </summary>
        float currentHealth { get; }
        /// <summary>
        /// Максимальное значение здоровья
        /// </summary>
        int MaxHealth { get; }
        /// <summary>
        /// Проверить если  <seealso cref="currentHealth"/>  0 и меньше
        /// </summary>
        bool isDead { get; set; }
        /// <summary>
        /// Увеличить или уменьшить <seealso cref="currentHealth"/>  с учётом the <seealso cref="MaxHealth"/>
        /// </summary>
        /// <param name="value">value</param>
        void AddHealth(int value);
        /// <summary>
        /// Сменить <seealso cref="currentHealth"/> с учётом <seealso cref="MaxHealth"/>
        /// </summary>
        /// <param name="value">value</param>
        void ChangeHealth(int value);
        /// <summary>
        /// Изменить значение максимальное здоровье
        /// </summary>
        /// <param name="value">value</param>
        void ChangeMaxHealth(int value);
        /// <summary>
        /// Сбросить текущее здоровье до определенного значения
        /// </summary>
        /// <param name="health">target health</param>
        void ResetHealth(float health);
        /// <summary>
        /// Сбросить текущее здоровье до максимального
        /// </summary>
        void ResetHealth();

    }

    public static class HealthControllerHelper
    {
        static IHealthController GetHealthController(this GameObject gameObject)
        {
            return gameObject.GetComponent<IHealthController>();
        }

        /// <summary>
        /// Увеличить или уменьшить <seealso cref="currentHealth"/>  с учётом <seealso cref="MaxHealth"/>
        /// </summary>
        /// <param name="receiver">Target to GetComponent <seealso cref="vIHealthController"/> </param>
        /// <param name="health"></param>
        public static void AddHealth(this GameObject receiver, int health)
        {
            var healthController = receiver.GetHealthController();
            if (healthController != null)
            {
                healthController.AddHealth(health);
            }
        }
        /// <summary>
        /// Изменить <seealso cref="currentHealth"/> с учётом <seealso cref="MaxHealth"/>
        /// </summary>
        /// <param name="receiver">Target to GetComponent <seealso cref="vIHealthController"/> </param>
        /// <param name="health"></param>
        public static void ChangeHealth(this GameObject receiver, int health)
        {
            var healthController = receiver.GetHealthController();
            if (healthController != null)
            {
                healthController.ChangeHealth(health);
            }
        }

        /// <summary>
        /// Изменить максимальное количество здоровья
        /// </summary>
        /// <param name="receiver">Target to GetComponent <seealso cref="vIHealthController"/> </param>
        /// <param name="health"></param>
        public static void ChangeMaxHealth(this GameObject receiver, int health)
        {
            var healthController = receiver.GetHealthController();
            if (healthController != null)
            {
                healthController.ChangeMaxHealth(health);
            }
        }

        /// <summary>
        /// Проверьте, есть ли у GameObject IHealthController
        /// </summary>
        /// <param name="gameObject">Target to GetComponent <seealso cref="vIHealthController"/> </param>
        /// <returns></returns>
        public static bool HasHealth(this GameObject gameObject)
        {
            return gameObject.GetHealthController() != null;
        }

        /// <summary>
        /// Проверить умер ли игрок
        /// </summary>
        /// <param name="gameObject">Target to GetComponent <seealso cref="vIHealthController"/> </param>
        /// <returns>return true if GameObject does not has a vIHealthController or currentHealth is less or equals zero </returns>
        public static bool IsDead(this GameObject gameObject)
        {
            var health = gameObject.GetHealthController();
            return health == null || health.isDead;
        }

        /// <summary>
        /// Сбросить текущее здоровье до определенного значения
        /// </summary>
        /// <param name="receiver">Target to GetComponent <seealso cref="vIHealthController"/> </param>
        /// <param name="health">target health</param>
        public static void ResetHealth(this GameObject receiver, float health)
        {
            var healthController = receiver.GetHealthController();
            if (healthController != null)
            {
                healthController.ResetHealth(health);
            }
        }
        /// <summary>
        /// Сбросить текущее здоровье до максимального
        /// </summary>
        /// <param name="receiver">Target to GetComponent <seealso cref="vIHealthController"/> </param>
        public static void ResetHealth(this GameObject receiver)
        {
            var healthController = receiver.GetHealthController();
            if (healthController != null)
            {
                healthController.ResetHealth();
            }
        }
    }
}
