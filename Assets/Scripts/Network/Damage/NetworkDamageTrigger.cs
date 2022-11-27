using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDamageTrigger : MonoBehaviour
{
    [SerializeField] private Damage damage;

    public int AttackNum = 0;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && AttackNum != 0)
        {
            var _damage = new Damage(damage);
            _damage.sender = transform;
            _damage.receiver = collider.transform;
            collider.gameObject.ApplyDamage(_damage);
            AttackNum--;
        }
    }
}
