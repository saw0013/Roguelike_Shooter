using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletPoolNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject _hitWallParticles;

    [SerializeField] private List<AudioClip> _audioClipImpactRandom;

    private AudioSource _audioSource;

    private Rigidbody _rigidbody;

    //uint owner;
    //bool inited;
    //Vector3 target;
    //
    //[Server]
    //public void Init(uint owner, Vector3 target)
    //{
    //    this.owner = owner; //кто сделал выстрел
    //    this.target = target; //куда должна лететь пуля
    //    inited = true;
    //}

    void OnSpawned() => _rigidbody = GetComponent<Rigidbody>();
    private void Awake() => _audioSource = GetComponent<AudioSource>();

    private void Update()
    {
        _rigidbody.MovePosition(transform.position + (transform.forward * 130 * Time.deltaTime));
        
        //if (inited && isServer)
    }

    private void OnCollisionEnter(Collision collision)
    {
        var particle = Instantiate(_hitWallParticles, transform.position, transform.rotation);

        _audioSource.clip = _audioClipImpactRandom[Random.Range(0, _audioClipImpactRandom.Count)];
        _audioSource.Play();

        //EZ_PoolManager.Despawn(transform);
        Destroy(particle, .7f);
    }
}
