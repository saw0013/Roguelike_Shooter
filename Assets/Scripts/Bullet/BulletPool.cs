using System.Linq;
using UnityEngine;
using Mirror;
using Cosmo;
using Mirror.Experimental;
using MirrorBasics;
using static UnityEngine.ParticleSystem;

public class BulletPool : NetworkBehaviour
{
    [SerializeField] private GameObject _hitWallParticles;
    [SerializeField] private GameObject _hitPlayerParticles;
    [SerializeField] private GameObject _hitEnemyParticles;

    [SerializeField] private Damage damage;

    public int ForceShoot = 1000;

    private Rigidbody _rigidbody;

    [SerializeField, Tooltip("Life time bullet")] private float _lifeBullet = 5f;

    internal uint owner;
    private PlayerMovementAndLookNetwork _owner;


    //TODO : Будем отслеживать кто сделал выстрел, чтобы засчитать очки ему
    [Server]
    public void Init(/*uint owner*/PlayerMovementAndLookNetwork owner)
    {
        //this.owner = owner; //кто сделал выстрел
        _owner = owner;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
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
        var _damageToPlayer = new Damage(damage);
        _damageToPlayer.sender = transform;
        _damageToPlayer.receiver = collision.transform;
        collision.gameObject.ApplyDamage(_damageToPlayer);

        GameObject particle = null;

        switch (collision.gameObject.tag)
        {
            case "Player":
                particle = Instantiate(_hitPlayerParticles, transform.position, transform.rotation);
                _owner.ScorePlayerUpdate(-5);
                break;

            case "Enemy":
                particle = Instantiate(_hitEnemyParticles, transform.position, transform.rotation);
                _owner.ScorePlayerUpdate(10);
                break;

            default:  particle = Instantiate(_hitWallParticles, transform.position, transform.rotation);
                 break;
        }

        Destroy(particle, .7f);
        Destroy(gameObject);
    }

    public void OnSpawnBullet(int force, float size)
    {
        ForceShoot = force;
        transform.localScale = new Vector3(size, size, size);
    }
}
