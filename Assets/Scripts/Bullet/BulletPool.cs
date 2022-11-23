using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MeshRenderer = UnityEngine.MeshRenderer;

public class BulletPool : NetworkBehaviour
{
    [SerializeField] private GameObject _hitWallParticles;

    [SerializeField] private List<AudioClip> _audioClipImpactRandom;

    public int ForceShoot = 1000;

    private AudioSource _audioSource;

    private Rigidbody _rigidbody;

    [SerializeField, Tooltip("Life time bullet")] private float _lifeBullet = 5f;

    uint owner;
    bool inited;

    //TODO : Ѕудем отслеживать кто сделал выстрел, чтобы засчитать очки ему
    [Server]
    public void Init(uint owner)
    {
        this.owner = owner; //кто сделал выстрел
        inited = true;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        transform.GetComponent<MeshRenderer>().enabled = true; //¬ключим meshrenderer
    }


    private void Update()
    {
        // уда летит пул€ и скорость
        _rigidbody.AddForce(transform.forward * ForceShoot, ForceMode.VelocityChange);

        if (_lifeBullet < 0) Destroy(gameObject);
        else _lifeBullet -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var particle = Instantiate(_hitWallParticles, transform.position, transform.rotation);
        
        _audioSource.clip = _audioClipImpactRandom[Random.Range(0, _audioClipImpactRandom.Count)];
        _audioSource.Play();

        Destroy(particle, .7f);
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }

    public void OnSpawnBullet(int force) => ForceShoot = force; 
}
