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

    public Damage DamageToPlayer;
    public Damage DamageToEnemy;

    public int ForceShoot = 1000;

    private Rigidbody _rigidbody;

    [SerializeField, Tooltip("Life time bullet")] private float _lifeBullet = 5f;

    internal uint owner;
    private GameObject _owner;

    //[Server]
    public void Init(/*uint owner*/GameObject owner)
    {
        //this.owner = owner; //��� ������ �������
        _owner = owner;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //���� ����� ���� � ��������
        _rigidbody.AddForce(transform.forward * ForceShoot, ForceMode.VelocityChange);

        if (_lifeBullet < 0) Destroy(gameObject);
        else _lifeBullet -= Time.deltaTime;
    }

    //[ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        var _damageToPlayer = new Damage(DamageToPlayer);
        _damageToPlayer.sender = transform;
        _damageToPlayer.receiver = collision.transform;

        var _damageToEnemy = new Damage(DamageToEnemy);
        _damageToEnemy.sender = transform;
        _damageToEnemy.receiver = collision.transform;

        if (collision.gameObject == _owner) return;

        switch (collision.gameObject.tag)
        {
            case "Player":
                RpcParticles(_hitPlayerParticles);
                ClaimScore(_owner, -5);
                collision.gameObject.ApplyDamage(_damageToPlayer);
                break;

            case "Enemy":
                if (!collision.gameObject.GetComponent<EnemyData>().LocalDead)
                {
                    ClaimScore(_owner, 10);
                    RpcParticles(_hitEnemyParticles);
                    collision.gameObject.ApplyDamage(_damageToPlayer);
                }

                if (collision.gameObject.GetComponent<EnemyData>().LocalDead)
                {
                    ClaimScore(_owner, 50);
                    _owner.GetComponent<PlayerData>().EnemyKilled++;
                }
                break;

            default:
                RpcParticles(_hitWallParticles);
                break;
        }

        Destroy(gameObject);
    }


    public void OnSpawnBullet(int force, float size)
    {
        ForceShoot = force;
        transform.localScale = new Vector3(size, size, size);
    }

    [ClientCallback]
    void RpcParticles(GameObject prefabParticle)
    {
        GameObject particle = Instantiate(prefabParticle, transform.position, transform.rotation);
        Destroy(particle, .7f);
    }

    public void ClaimScore(GameObject player, int score)
    {
        if(player != null)
            player.GetComponent<PlayerData>().ScorePlayer += score;
    }
}
