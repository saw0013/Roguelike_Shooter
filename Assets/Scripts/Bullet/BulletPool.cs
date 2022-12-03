using UnityEngine;
using Mirror;
using Cosmo;

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

    //TODO : Будем отслеживать кто сделал выстрел, чтобы засчитать очки ему
    [Server]
    public void Init(uint owner)
    {
        this.owner = owner; //кто сделал выстрел
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {

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
        switch (collision.gameObject.tag)
        {
            case "Player":
                var _damageToPlayer = new Damage(damage);
                _damageToPlayer.sender = transform;
                _damageToPlayer.receiver = collision.transform;
                collision.gameObject.ApplyDamage(_damageToPlayer);

                var particlePlayer = Instantiate(_hitPlayerParticles, transform.position, transform.rotation);
                Destroy(particlePlayer, .7f);
                Destroy(gameObject);
                break;

            case "Enemy":
                var _damageToEnemy = new Damage(damage);
                _damageToEnemy.sender = transform;
                _damageToEnemy.receiver = collision.transform;
                collision.gameObject.ApplyDamage(_damageToEnemy);

                var particleEnemy = Instantiate(_hitEnemyParticles, transform.position, transform.rotation);
                Destroy(particleEnemy, .7f);
                Destroy(gameObject);
                break;

            default:
                var particle = Instantiate(_hitWallParticles, transform.position, transform.rotation);
                Destroy(particle, .7f);
                Destroy(gameObject);
                break;
        }
    }

    public void OnSpawnBullet(int force, float size)
    {
        ForceShoot = force;
        transform.localScale = new Vector3(size, size, size);
    }
}
