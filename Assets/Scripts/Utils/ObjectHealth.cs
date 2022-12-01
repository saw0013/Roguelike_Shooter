using Cosmoground;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectHealth : MonoBehaviour
{
    [SerializeField][ReadOnly] protected bool _isDead;
    [SerializeField] protected float _currentHealth;

    public bool isImmortal = false;

    [Tooltip("Если вы хотите начать с другого значения, снимите этот флажок и убедитесь, что текущее значение здоровья имеет значение больше нуля.")]
    public bool fillHealthOnStart = true;
    public int maxHealth = 100;

    [SerializeField] protected ValueChangedEvent onChangeHealth;

    [SerializeField] protected OnReceiveDamage _onReceiveDamage = new OnReceiveDamage();

    [SerializeField] protected OnDead _onDead = new OnDead();

    public OnReceiveDamage onReceiveDamage { get { return _onReceiveDamage; } protected set { _onReceiveDamage = value; } }
    public OnDead onDead { get { return _onDead; } protected set { _onDead = value; } }

    public List<string> acceptableAttacks = new List<string>() { "explosion", "projectile" };

    void Start()
    {
        if (fillHealthOnStart)
            _currentHealth = maxHealth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Damage d = new Damage { damageValue = UnityEngine.Random.Range(5, 25), damageType = "projectile" }; //Получить дамаг от объекта
        TakeDamage(d);
    }


    public virtual void TakeDamage(Damage damage)
    {
        if (acceptableAttacks.Contains(damage.damageType))
        {
            if (damage != null)
            {
                onReceiveDamage.Invoke(damage);

                if (_currentHealth > 0 && !isImmortal)
                {
                    _currentHealth -= damage.damageValue;
                }

                if (damage.damageValue > 0)
                    onReceiveDamage.Invoke(damage);

                if (!_isDead && _currentHealth <= 0)
                {
                    _isDead = true;
                    onDead.Invoke(gameObject);
                }
            }
        }
    }

    public virtual void ChangeHealth(int value)
    {
        _currentHealth = value;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
        if (!_isDead && _currentHealth <= 0)
        {
            _isDead = true;
            onDead.Invoke(gameObject);
        }
    }
}

[Serializable]
public class ValueChangedEvent : UnityEvent<float> { }
