using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Damage
{
    [Tooltip("Нанесите урон здоровью персонажа")]
    public int damageValue = 15;
    [Tooltip("Наносите урон, даже если персонаж блокирует")]
    public bool ignoreDefense;
    [Tooltip("Активировать Ragdoll при ударе по персонажу")]
    public bool activeRagdoll;
    [Tooltip("Время поддерживать активность Ragdoll")]
    public float senselessTime;
    [HideInInspector]
    public Transform sender;
    [HideInInspector]
    public Transform receiver;
    [HideInInspector]
    public Vector3 hitPosition;
    public bool hitReaction = true;
    [HideInInspector]
    public int recoil_id = 0;
    [HideInInspector]
    public int reaction_id = 0;
    public string damageType;
    [HideInInspector] public Vector3 force;

    public Damage()
    {
        this.damageValue = 15;
        this.hitReaction = true;
    }

    public Damage(int value)
    {
        this.damageValue = value;
        this.hitReaction = true;
    }

    public Damage(int value, bool ignoreReaction)
    {
        this.damageValue = value;
        this.hitReaction = !ignoreReaction;
        if (ignoreReaction)
        {
            this.recoil_id = -1;
            this.reaction_id = -1;
        }
    }

    public Damage(Damage damage)
    {
        this.damageValue = damage.damageValue;
        this.ignoreDefense = damage.ignoreDefense;
        this.activeRagdoll = damage.activeRagdoll;
        this.sender = damage.sender;
        this.receiver = damage.receiver;
        this.recoil_id = damage.recoil_id;
        this.reaction_id = damage.reaction_id;
        this.damageType = damage.damageType;
        this.hitPosition = damage.hitPosition;
        this.senselessTime = damage.senselessTime;
        this.force = damage.force;
    }

    /// <summary>
    /// Рассчитать процент снижения урона
    /// </summary>
    /// <param name="damageReduction"></param>
    public void ReduceDamage(float damageReduction)
    {
        int result = (int)(this.damageValue - ((this.damageValue * damageReduction) / 100));
        this.damageValue = result;
    }
}
