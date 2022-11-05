using EZ_Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject _hitWallParticles;

    [SerializeField] private List<AudioClip> _audioClipImpactRandom;

    private AudioSource _audioSource;

    private Rigidbody _rigidbody;

    void OnSpawned() => _rigidbody = GetComponent<Rigidbody>();
    private void Awake() => _audioSource = GetComponent<AudioSource>();

    private void Update()
    {
        _rigidbody.MovePosition(transform.position + (transform.forward * 130 * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        var particle = Instantiate(_hitWallParticles, transform.position, transform.rotation);

        _audioSource.clip = _audioClipImpactRandom[Random.Range(0, _audioClipImpactRandom.Count)];
        _audioSource.Play();

        EZ_PoolManager.Despawn(transform);
        Destroy(particle, .7f);
    }
}
