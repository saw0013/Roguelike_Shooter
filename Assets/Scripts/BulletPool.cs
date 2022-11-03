using EZ_Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject _hitWallParticles;

    private Rigidbody _rigidbody;

    void OnSpawned() =>_rigidbody = GetComponent<Rigidbody>();

    private void Update()
    {
        _rigidbody.MovePosition(transform.position + (transform.forward * 130 * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        var particle = Instantiate(_hitWallParticles, transform.position, transform.rotation);
        EZ_PoolManager.Despawn(transform);
        Destroy(particle, .7f);
    }
}
