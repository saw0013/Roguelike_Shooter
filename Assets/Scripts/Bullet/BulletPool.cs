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

    [SyncVar] public float Size;

    public int ForceShoot = 1000;

    private Rigidbody _rigidbody;

    [SerializeField, Tooltip("Life time bullet")] private float _lifeBullet = 5f;

    [HideInInspector] public GameObject _owner;


    //[Server]
    public void Init(/*uint owner*/GameObject owner)
    {
        //this.owner = owner; //кто сделал выстрел
        _owner = owner;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        RpcChangeSize();
    }

    private void Update()
    {
        //Куда летит пуля и скорость
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
                _damageToPlayer.sender = transform;
                _damageToPlayer.receiver = collision.transform;

                //if (!_owner)
                //{
                //    var guard = collision.gameObject.GetComponent<PlayerData>().guardPlayer / 100;
                //    Debug.LogWarning("Guard = " + guard);
                //    var damage = _damageToPlayer.damageValue * (1 - guard);
                //    Debug.LogWarning("Damage = " + guard);
                //    _damageToPlayer.damageValue = damage;
                //}
                //else _damageToPlayer.damageValue = 5;

                var player = _owner.gameObject?.GetComponent<PlayerData>();

                if(player != null)
                {
                    var guard = collision.gameObject.GetComponent<PlayerData>().guardPlayer / 100;
                    Debug.LogWarning("Guard = " + guard);
                    int damage = _damageToPlayer.damageValue * (1 - guard);
                    Debug.LogWarning("Damage = " + damage);
                    _damageToPlayer.damageValue = damage;
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
    }

    [ClientRpc]
    private void RpcChangeSize()
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
