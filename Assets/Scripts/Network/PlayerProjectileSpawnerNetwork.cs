using UnityEngine;
using UnityEngine.UI;
using Mirror;
using static UnityEngine.UI.GridLayoutGroup;
using Zenject.SpaceFighter;

public class PlayerProjectileSpawnerNetwork : NetworkBehaviour
{
    #region Variables

    [Header("UI")]
    [SerializeField] private Text _textCartridges;

    [Header("Input")]
    public KeyCode spawnKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.Mouse1;

    [Header("Spawner Settings")]
    [SerializeField] private Transform _bullet;
    [SerializeField] private Transform _spawnPoint;

    public float SpawnRate;
    public float ReloadTime;
    public int MaxCartridges;

    private float timer;
    private float timerReload;

    private int сartridges;

    private bool reloading;

    [Header("Particles")]
    public ParticleSystem spawnParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource _shootAudio;
    [SerializeField] private AudioSource _reloadAudio;
    [SerializeField] private AudioSource _nullShootAudio;

    [SerializeField] private PlayerData playerData;

    #endregion

    private void Start()
    {
        сartridges = MaxCartridges;
        _textCartridges.text = $"Cartridges: {сartridges}";
    }

    void Update()
    {
        if (hasAuthority)
        {
            timer += Time.deltaTime;

            if (playerData.InputActive)
            {
                if (Input.GetKey(reloadKey) && !reloading && сartridges != MaxCartridges)
                {
                    reloading = true;

                    if (_reloadAudio) _reloadAudio.Play();
                }

                if (Input.GetKey(spawnKey) && timer >= SpawnRate && !reloading)
                {
                    if (сartridges > 0)
                    {
                        SpawnProjectile();
                    }
                    else
                    {
                        _nullShootAudio.Play();
                    }
                }

                if (reloading)
                {
                    timerReload += Time.deltaTime;
                    if (timerReload >= ReloadTime)
                    {
                        сartridges = MaxCartridges;
                        timerReload = 0;
                        reloading = false;
                        ReloadText();
                    }
                }
            }
        }

    }

    public void ReloadText() => _textCartridges.text = $"Cartridges: {сartridges}";

   

    void SpawnProjectile()
    {
        timer = 0f;
        сartridges--;
        ReloadText();
        CmdFire(netId);
        //CmdSpawnBullet(netId);
        //EZ_PoolManager.Spawn(_bullet, _spawnPoint.position, _spawnPoint.rotation);

        if (spawnParticles)
            spawnParticles.Play();

        if (_shootAudio)
            _shootAudio.Play();
    }

    #region Метод для сервера. Если игрок является сервером

    [Server]
    public void SpawnBullet(uint owner)
    {
        GameObject bulletGo = Instantiate(_bullet.gameObject, _spawnPoint.position, _spawnPoint.rotation); //Создаем локальный объект пули на сервере
        NetworkServer.Spawn(bulletGo); //отправляем информацию о сетевом объекте всем игрокам.
        
        bulletGo.GetComponent<BulletPool>().Init(owner); //инициализируем поведение пули
    }

    [Command]
    public void CmdSpawnBullet(uint owner) => SpawnBullet(owner);

    #endregion

    [ClientRpc]
    void RpcClientShot(uint owner)
    {
        var __bullet = Instantiate(_bullet.gameObject, _spawnPoint.position, _spawnPoint.rotation);
        __bullet.GetComponent<BulletPool>().Init(owner);
    }


    [Command]
    void CmdFire(uint owner) => RpcClientShot(owner);

}
