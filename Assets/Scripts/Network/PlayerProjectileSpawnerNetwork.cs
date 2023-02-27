using UnityEngine;
using UnityEngine.UI;
using Mirror;
using FMODUnity;
using DG.Tweening;
using MirrorBasics;
using System.Drawing;

public class PlayerProjectileSpawnerNetwork : NetworkBehaviour
{
    #region Variables

    [Header("UI")]
    [SerializeField] private Text _textCartridges;
    [SerializeField] private Slider _sliderCatridges;
    [SerializeField] private Image _imageSliderCartridges;

    private UnityEngine.Color deffualtColorSlider;

    [Header("Input")]
    public KeyCode spawnKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.Mouse1;

    [Header("Spawner Settings")]
    [SerializeField] private Transform _bullet;
    [SerializeField] private Transform _spawnPoint;

    public float SpawnRate;
    // public float ReloadTime;

    private float timerShoot;
    private float timerReloadDelay;

    /// <summary>
    /// Время перезарядки
    /// </summary>
    private float timerReload;

    /// <summary>
    /// Патроны
    /// </summary>
    private int сartridges;

    /// <summary>
    /// Перезаряжается?
    /// </summary>
    private bool reloading;

    private bool startDoTween;

    [Header("Particles")]
    public ParticleSystem spawnParticles;

    [Header("Audio")]
    [SerializeField] private StudioEventEmitter _shootAudio;
    [SerializeField] private StudioEventEmitter _reloadAudio;
    [SerializeField] private StudioEventEmitter _nullShootAudio;

    [SerializeField] private PlayerData playerData;

    private PlayerMovementAndLookNetwork playerNetwork;

    #endregion

    private void Start()
    {
        playerNetwork = GetComponent<PlayerMovementAndLookNetwork>();
        deffualtColorSlider = _imageSliderCartridges.color;
    }
    void Update()
    {
        if (isOwned)
        {
            if (playerData.GetInputActive())
            {
                timerShoot += Time.deltaTime;
                timerReloadDelay += Time.deltaTime;

                if (Input.GetKey(reloadKey) && !reloading && сartridges != playerData.MaxCartridges && timerReloadDelay >= 5)
                {
                    timerReload = 0;
                    reloading = true;

                    if (_reloadAudio) _reloadAudio.Play();
                }

                if (Input.GetKey(spawnKey) && timerShoot >= playerData.BuletRate && !reloading)
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

                    var _tweenReload = _imageSliderCartridges.DOColor(UnityEngine.Color.black, 0.1f);
                    _tweenReload.OnComplete(() => _imageSliderCartridges.DOColor(deffualtColorSlider, 0.1f));

                    SliderAction(0);

                    if (timerReload >= GetTimeReload())
                    {
                        сartridges = playerData.MaxCartridges;
                        timerReload = 0;
                        reloading = false;
                        ReloadText();
                        startDoTween = false;
                    }
                }
            }
        }

    }

    private void SliderAction(int action)
    {
        switch (action)
        {
            case 0:
                if (!startDoTween)
                {
                    startDoTween = true;
                    float cartridges = (float)playerData.MaxCartridges / 100;
                    Debug.LogWarning("Cartridges " + cartridges);

                    var timeReload = GetTimeReload();

                    Debug.LogWarning("TimeReload " + timeReload);

                    _sliderCatridges.DOValue(cartridges, timeReload);
                }
                break;

            case 1:

                var _tweenOn = _imageSliderCartridges.DOColor(UnityEngine.Color.red, 0.1f);
                _tweenOn.OnComplete(() => _imageSliderCartridges.DOColor(deffualtColorSlider, 0.1f));

                break;
        }

    }

    private float GetTimeReload()
    {
        float procent = ProcentReloadTime();
        var ammoReload = playerData.AmmoReload;
        var TimeReload = ammoReload * procent;       
        return TimeReload;
    }

    private float ProcentReloadTime()
    {
        var maxCartridges = playerData.MaxCartridges;
        float procentReload = сartridges * 100 / maxCartridges;
        procentReload /= 100;
        return 1 - procentReload;
    }

    public void ReloadText() => _textCartridges.text = $"AMMO: {сartridges}";

    public void GetCatridges() 
    {
        сartridges = playerData.MaxCartridges;
        _sliderCatridges.maxValue = (float)сartridges / 100;
        _sliderCatridges.value = _sliderCatridges.maxValue;
        _textCartridges.text = $"AMMO: {сartridges}";
    } 

    void SpawnProjectile()
    {
        timerShoot = 0f;
        сartridges--;
        _sliderCatridges.value -= 0.01f;
        SliderAction(1);
        ReloadText();

        var sizeBulletLocal = GetComponentInParent<PlayerData>().SizeBullet;

        var damagePlayerLocal = GetComponentInParent<PlayerData>().DamagePlayer;
        CmdSpawnBullet(damagePlayerLocal, sizeBulletLocal);
    }


    [Command] //позволяет локальному проигрывателю удаленно вызывать эту функцию на серверной копии объекта
    public void CmdSpawnBullet(int damage, float size)
    {
        var bullet = Instantiate(_bullet.gameObject, _spawnPoint.position, _spawnPoint.rotation);
        bullet.GetComponent<NetworkMatch>().matchId = /*playerNetwork.networkMatch.matchId*/playerNetwork.matchID.ToGuid();

        bullet.GetComponent<BulletPool>().DamageToPlayer.damageValue = 5;

        bullet.GetComponent<BulletPool>().DamageToEnemy.damageValue = damage;

        bullet.GetComponent<BulletPool>().DamageToPlayer.sender = transform;

        bullet.GetComponent<BulletPool>().OnSpawnBullet(2, size);
        bullet.GetComponent<BulletPool>().Init(gameObject);
        playerData.AmmoWasted++;
        //NetworkServer.Spawn(bullet); //отправляем информацию о сетевом объекте всем игрокам.

        RpcSpawnBullet(size);
    }

    [ClientRpc] //позволяет серверу удаленно вызывать эту функцию для всех клиентских копий объекта
    public void RpcSpawnBullet(float size)
    {
        var bullet = Instantiate(_bullet.gameObject, _spawnPoint.position, _spawnPoint.rotation);
        bullet.GetComponent<BulletPool>().OnSpawnBullet(2, size);
        bullet.GetComponent<BulletPool>().DamageToPlayer.damageValue = 0;
        bullet.GetComponent<BulletPool>().DamageToPlayer.sender = transform;
        //bullet.GetComponent<BulletPool>().DamageToEnemy.damageValue = playerData.DamagePlayer;
        //bullet.GetComponent<NetworkMatch>().matchId = /*playerNetwork.networkMatch.matchId*/playerNetwork.matchID.ToGuid();
        //bullet.GetComponent<BulletPool>().OnSpawnBullet(2, playerData.SizeBullet);
        //bullet.GetComponent<BulletPool>().Init(gameObject);
        //playerData.AmmoWasted++;

        if (spawnParticles)
            spawnParticles.Play();

        if (_shootAudio)
            _shootAudio.Play();
    }
}
