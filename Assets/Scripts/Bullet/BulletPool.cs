using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MeshRenderer = UnityEngine.MeshRenderer;

public class BulletPool : NetworkBehaviour
{
    [SerializeField] private GameObject _hitWallParticles;

    [SerializeField] private Damage damage;

    [SerializeField] private List<AudioClip> _audioClipImpactRandom = new List<AudioClip>();

    public int ForceShoot = 1000;

    private AudioSource _audioSource;

    private Rigidbody _rigidbody;

    [SerializeField, Tooltip("Life time bullet")] private float _lifeBullet = 5f;

    uint owner;
    bool inited;

    //TODO : Будем отслеживать кто сделал выстрел, чтобы засчитать очки ему
    [Server]
    public void Init(uint owner)
    {
        this.owner = owner; //кто сделал выстрел
        inited = true;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        //TODO : Метод появления изменён. Проверить!!!
        if(!transform.GetComponent<MeshRenderer>().enabled)
            transform.GetComponent<MeshRenderer>().enabled = true; //Включим meshrenderer
        _audioSource = GetComponent<AudioSource>();
    }


    private void Update()
    {
        //Куда летит пуля и скорость
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
        Destroy(gameObject);
    }

    public void OnSpawnBullet(int force) => ForceShoot = force; 
}
