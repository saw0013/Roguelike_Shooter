using System.Xml.Serialization;
using UnityEngine;
using Mirror;
using Cosmo;

public class BulletPool : NetworkBehaviour
{
    [SerializeField] private GameObject _hitWallParticles;
    [SerializeField] private GameObject _hitPlayerParticles;
    [SerializeField] private GameObject _hitEnemyParticles;

    public Damage DamageToPlayer;
    public Damage DamageToEnemy;

    public float Size;

    public int ForceShoot = 1000;

    private Rigidbody _rigidbody;

    [SerializeField, Tooltip("Life time bullet")] private float _lifeBullet = 5f;

    [HideInInspector] public GameObject _owner;


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

    private void Start()
    {
        //ChangeSize();
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
        //if (collision.gameObject == _owner) return

        switch (collision.gameObject.tag)
        {
            case "Player":
                if (isClient)
                {
                    RpcParticles(_hitPlayerParticles);
                    break;
                }

                var _damageToPlayer = new Damage(DamageToPlayer);
                _damageToPlayer.receiver = collision.transform;

                
               if(_damageToPlayer.sender.CompareTag("Enemy"))
               {
                    var guard = (float)collision.gameObject.GetComponent<PlayerData>().guardPlayer / 100f;

                    Debug.LogWarning("Guard = " + guard);

                    float damage = _damageToPlayer.damageValue * (1 - guard);

                    Debug.LogWarning("Damage = " + damage);

                    _damageToPlayer.damageValue = (int)damage;
                }

                if (_owner) ClaimScore(_owner, -5);
                collision.gameObject.ApplyDamage(_damageToPlayer);

                break;

            case "Enemy":

                if (isClient)
                {
                    RpcParticles(_hitEnemyParticles);
                    return;
                }
               
                var _damageToEnemy = new Damage(DamageToEnemy);
                _damageToEnemy.sender = transform;
                _damageToEnemy.receiver = collision.transform;

                if (collision.gameObject.GetComponent<EnemyData>()._SyncHealth > 1)
                {
                    if(_owner) ClaimScore(_owner, 10);
                    collision.gameObject.ApplyDamage(_damageToEnemy);
                    
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
        Size = size;
        ChangeSize();
    }

    private void ChangeSize()
    {
        Debug.LogWarning("Size bullet = " + Size);
        transform.localScale = new Vector3(Size, Size, Size);
    }
   
    void RpcParticles(GameObject prefabParticle)
    {
        GameObject particle = Instantiate(prefabParticle, transform.position, transform.rotation);
        Destroy(particle, .7f);
    }

    public void ClaimScore(GameObject player, int score)
    {
        if (player != null)
            player.GetComponent<PlayerData>().ScorePlayer += score;
    }
}
