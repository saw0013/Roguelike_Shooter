using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject _hitWallParticles;

    [SerializeField] private List<AudioClip> _audioClipImpactRandom;

    private AudioSource _audioSource;

    private Rigidbody _rigidbody;

    [SerializeField, Tooltip("Life time bullet")] private float _lifeBullet = 5f;

     
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }
   

    private void Update()
    {
        _rigidbody.MovePosition(transform.position + (transform.forward * 130 * Time.deltaTime));

        if(_lifeBullet < 0) Destroy(gameObject);
        else _lifeBullet -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var particle = Instantiate(_hitWallParticles, transform.position, transform.rotation);

        _audioSource.clip = _audioClipImpactRandom[Random.Range(0, _audioClipImpactRandom.Count)];
        _audioSource.Play();

        Destroy(particle, .7f);
    }
}
