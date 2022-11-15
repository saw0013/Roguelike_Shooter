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

    private int �artridges;

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
        �artridges = MaxCartridges;
        _textCartridges.text = $"Cartridges: {�artridges}";
    }

    void Update()
    {
        if (hasAuthority)
        {
            timer += Time.deltaTime;

            if (playerData.InputActive)
            {
                if (Input.GetKey(reloadKey) && !reloading && �artridges != MaxCartridges)
                {
                    reloading = true;

                    if (_reloadAudio) _reloadAudio.Play();
                }

                if (Input.GetKey(spawnKey) && timer >= SpawnRate && !reloading)
                {
                    if (�artridges > 0)
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
                        �artridges = MaxCartridges;
                        timerReload = 0;
                        reloading = false;
                        ReloadText();
                    }
                }
            }
        }

    }

    public void ReloadText() => _textCartridges.text = $"Cartridges: {�artridges}";

   

    void SpawnProjectile()
    {
        timer = 0f;
        �artridges--;
        ReloadText();
        CmdFire(netId);
        //CmdSpawnBullet(netId);
        //EZ_PoolManager.Spawn(_bullet, _spawnPoint.position, _spawnPoint.rotation);

        if (spawnParticles)
            spawnParticles.Play();

        if (_shootAudio)
            _shootAudio.Play();
    }

    #region ����� ��� �������. ���� ����� �������� ��������

    [Server]
    public void SpawnBullet(uint owner)
    {
        GameObject bulletGo = Instantiate(_bullet.gameObject, _spawnPoint.position, _spawnPoint.rotation); //������� ��������� ������ ���� �� �������
        NetworkServer.Spawn(bulletGo); //���������� ���������� � ������� ������� ���� �������.
        
        bulletGo.GetComponent<BulletPool>().Init(owner); //�������������� ��������� ����
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
