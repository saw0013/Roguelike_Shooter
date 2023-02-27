using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmo;

public class NetworkDamageTrigger : MonoBehaviour
{
    [SerializeField] private Damage damage;

    public int AttackNum = 0;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && AttackNum != 0)
        {
            var Damage = new Damage(damage);
            Damage.sender = transform;
            Damage.receiver = collider.transform;

            var guard = (float)collider.gameObject.GetComponent<PlayerData>().guardPlayer / 100f;
            float _damage = Damage.damageValue * (1 - guard);
            Damage.damageValue = (int)_damage;

            collider.gameObject.ApplyDamage(Damage);
            AttackNum--;
        }
    }
}
