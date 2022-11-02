using FMODUnity;
using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class UserInputData : MonoBehaviour, IConvertGameObjectToEntity
{
    #region Animation
    [Header("Animation")]
    //public string MoveAnimHash;

    public string Forward;

    public string Strafe;

    //public string MoveSpeedAnimHash;

    //public string HitAnimHash;

    //public string DeadAnimHash;

    //public string ShootAnimHash;
    #endregion

    #region Sound
    [Header("Sound")]

    [SerializeField]
    [EventRef]
    //private string StepSound;

    public FMOD.Studio.EventInstance aSoundInstance;
    #endregion

    private ISettingPlayer _settingPlayer;
    [Header("Other")]
    public CharacterHealth health;

    public float speed = 50;
    public bool IsInvisibal;

    public MonoBehaviour ShootAction;
    public MonoBehaviour jerkAction;

    private void Awake()
    {
        //aSoundInstance = RuntimeManager.CreateInstance(StepSound);

        //health.Health = _settingPlayer.Health;
       // GetComponent<ShootAbility>()._settings._forceShoot = _settingPlayer.ForceShoot;
    }

    private void Update()
    {
        
    }

    public interface ISettingPlayer 
    {
        float Speed { get; }
        int Health { get; }
        float ForceShoot { get; }
    }

   

    public class Game : ISettingPlayer
    {
        public float Speed => 100;

        public int Health => 100;

        public float ForceShoot => 20;
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //if (PhotonView.Get(this.gameObject).IsMine)
        //{
            dstManager.AddComponentData(entity, new InputData());
        //}

        dstManager.AddComponentData(entity, new MoveData()
        {
            Speed = speed / 10
        });
        if (ShootAction != null && ShootAction is IAbility)
        {
            dstManager.AddComponentData(entity, new ShootData());
        }
        if(Forward != String.Empty && Strafe != String.Empty/* && DeadAnimHash != String.Empty*/)
        {
            dstManager.AddComponentData(entity, new AnimData());
        }
    }
}

public struct InputData : IComponentData
{
    public float2 Move;
    public float Shoot;
    public float Jerk;
    public float Invisibal;
}

public struct ShootData : IComponentData
{

}

public struct MoveData : IComponentData
{
    public float Speed;
    public float JerkSpeed;
}

public struct AnimData : IComponentData
{

}

