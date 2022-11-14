using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerProjectileSpawnerNetwork : NetworkBehaviour
{
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

    private int ñartridges;

    private bool reloading;

    [Header("Particles")]
    public ParticleSystem spawnParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource _shootAudio;
    [SerializeField] private AudioSource _reloadAudio;
    [SerializeField] private AudioSource _nullShootAudio;

    [SerializeField] private PlayerData playerData;

    private void Start()
    {
        ñartridges = MaxCartridges;
        _textCartridges.text = $"Cartridges: {ñartridges}";
    }

    void Update()
    {

        timer += Time.deltaTime;

        if (playerData.InputActive)
        {
            if (Input.GetKey(reloadKey) && !reloading && ñartridges != MaxCartridges)
            {
                reloading = true;

                if (_reloadAudio) _reloadAudio.Play();
            }

            if (Input.GetKey(spawnKey) && timer >= SpawnRate && !reloading)
            {
                if (ñartridges > 0)
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
                    ñartridges = MaxCartridges;
                    timerReload = 0;
                    reloading = false;
                    ReloadText();
                }
            }
        }

    }

    public void ReloadText() => _textCartridges.text = $"Cartridges: {ñartridges}";

   

    void SpawnProjectile()
    {
        timer = 0f;
        ñartridges--;
        ReloadText();

        var bullet = Instantiate(_bullet, _spawnPoint.position, _spawnPoint.rotation);
        //EZ_PoolManager.Spawn(_bullet, _spawnPoint.position, _spawnPoint.rotation);

        if (spawnParticles)
            spawnParticles.Play();

        if (_shootAudio)
            _shootAudio.Play();
    }
}
