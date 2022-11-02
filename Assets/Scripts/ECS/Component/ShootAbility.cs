using System;
using System.Text;
using UnityEngine;
using Zenject;

/// <summary>
/// Класс описывающий состояние пули
/// </summary>
public class ShootAbility : MonoBehaviour, IAbility
{
    private float shootTime = float.MinValue;
    public Vector3 bulletScale;
    public CharacterData data;
    public bool UseBulletAbility;

    
    [HideInInspector] public Settings _settings;

    [SerializeField]
    [FMODUnity.EventRef]
    private string aSound;

    private void PlayASound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(aSound);
    }


    [Inject]
    public void Construct(Settings settings)
    {
        _settings = settings;
    }

    public void Execude()
    {
        if (Time.time < shootTime + _settings._delayShoot) return;

        shootTime = Time.time;

        if(_settings._bullet != null)
        {
            var t = transform;
            
        }
        else
        {
            Debug.LogError("[SHOOT ABILITY] No bullet prefab link!");
        }

        PlayASound();
        data.Score(5);
    }

    [Serializable]
    public class Settings
    {
        public GameObject _bullet;
        public float _forceShoot;
        public float _delayShoot;
    }
}
