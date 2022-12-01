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
        /// Event ���������� ����� <seealso cref="currentHealth"/>  ����� ���� ��� ������
        /// </summary>
        OnDead onDead { get; }
        /// <summary>
        /// ������� �������� ��������
        /// </summary>
        float currentHealth { get; }
        /// <summary>
        /// ������������ �������� ��������
        /// </summary>
        int MaxHealth { get; }
        /// <summary>
        /// ��������� ����  <seealso cref="currentHealth"/>  0 � ������
        /// </summary>
        bool isDead { get; set; }
        /// <summary>
        /// ��������� ��� ��������� <seealso cref="currentHealth"/>  � ������ the <seealso cref="MaxHealth"/>
        /// </summary>
        /// <param name="value">value</param>
        void AddHealth(int value);
        /// <summary>
        /// ������� <seealso cref="currentHealth"/> � ������ <seealso cref="MaxHealth"/>
        /// </summary>
        /// <param name="value">value</param>
        void ChangeHealth(int value);
        /// <summary>
        /// �������� �������� ������������ ��������
        /// </summary>
        /// <param name="value">value</param>
        void ChangeMaxHealth(int value);
        /// <summary>
        /// �������� ������� �������� �� ������������� ��������
        /// </summary>
        /// <param name="health">target health</param>
        void ResetHealth(float health);
        /// <summary>
        /// �������� ������� �������� �� �������������
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
        /// ��������� ��� ��������� <seealso cref="currentHealth"/>  � ������ <seealso cref="MaxHealth"/>
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
        /// �������� <seealso cref="currentHealth"/> � ������ <seealso cref="MaxHealth"/>
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
        /// �������� ������������ ���������� ��������
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
        /// ���������, ���� �� � GameObject IHealthController
        /// </summary>
        /// <param name="gameObject">Target to GetComponent <seealso cref="vIHealthController"/> </param>
        /// <returns></returns>
        public static bool HasHealth(this GameObject gameObject)
        {
            return gameObject.GetHealthController() != null;
        }

        /// <summary>
        /// ��������� ���� �� �����
        /// </summary>
        /// <param name="gameObject">Target to GetComponent <seealso cref="vIHealthController"/> </param>
        /// <returns>return true if GameObject does not has a vIHealthController or currentHealth is less or equals zero </returns>
        public static bool IsDead(this GameObject gameObject)
        {
            var health = gameObject.GetHealthController();
            return health == null || health.isDead;
        }

        /// <summary>
        /// �������� ������� �������� �� ������������� ��������
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
        /// �������� ������� �������� �� �������������
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
