using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Experimental;
using MirrorBasics;
using TMPro;
using static UnityEngine.UI.GridLayoutGroup;
using Zenject.SpaceFighter;
using static Cinemachine.DocumentationSortingAttribute;

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
    // public float ReloadTime;
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

    private PlayerMovementAndLookNetwork playerNetwork;

    #endregion

    private void Start()
    {
        playerNetwork = GetComponent<PlayerMovementAndLookNetwork>();
        �artridges = MaxCartridges;
        _textCartridges.text = $"AMMO: {�artridges}";
    }

    void Update()
    {
        if (hasAuthority)
        {
            timer += Time.deltaTime;

            if (playerData.GetInputActive())
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
                    if (timerReload >= playerData.AmmoReload)
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

    public void ReloadText() => _textCartridges.text = $"AMMO: {�artridges}";



    void SpawnProjectile()
    {
        timer = 0f;
        �artridges--;
        ReloadText();
        CmdSpawnBullet();
    }


    [Command(requiresAuthority = false)] //��������� ���������� ������������� �������� �������� ��� ������� �� ��������� ����� �������
    public void CmdSpawnBullet()
    {
        var bullet = Instantiate(_bullet.gameObject, _spawnPoint.position, _spawnPoint.rotation); 
        bullet.GetComponent<NetworkMatch>().matchId = /*playerNetwork.networkMatch.matchId*/playerNetwork.matchID.ToGuid();
        bullet.GetComponent<BulletPool>().OnSpawnBullet(playerData.BuletForce, playerData.SizeBullet);
        bullet.GetComponent<BulletPool>().Init(playerNetwork);
        NetworkServer.Spawn(bullet); //���������� ���������� � ������� ������� ���� �������.

        MatchMaker.managerSessionSavedFromCollection(playerNetwork.matchID.ToGuid())
            .AddCountShoot(playerNetwork);

        RpcSpawnBullet();
    }

    [ClientRpc] //��������� ������� �������� �������� ��� ������� ��� ���� ���������� ����� �������
    public void RpcSpawnBullet()
    {
        //var bullet = Instantiate(_bullet.gameObject, _spawnPoint.position, _spawnPoint.rotation); //������� ��������� ������ ����
        //bullet.GetComponent<BulletPool>().OnSpawnBullet(playerData.BuletForce);
        //bullet.GetComponent<BulletPool>().owner = netId;
        if (spawnParticles)
            spawnParticles.Play();

        if (_shootAudio)
            _shootAudio.Play();
    }

  

}
