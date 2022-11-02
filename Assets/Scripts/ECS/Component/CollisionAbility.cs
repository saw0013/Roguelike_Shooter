using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CollisionAbility : MonoBehaviour, IConvertGameObjectToEntity, IAbility
{
    public Collider Colider;
    public List<MonoBehaviour> collisionsActions = new List<MonoBehaviour>();
    public List<IAbilityTarget> collisionsActionsIAbilities = new List<IAbilityTarget>();

    [HideInInspector]public List<Collider> collisions = new List<Collider>();

    private void Start()
    {               
        foreach (var action in collisionsActions)
        {
            if (action is IAbilityTarget abilities)
            {
                collisionsActionsIAbilities.Add(abilities);
            }
            else
            {
                Debug.LogError("[COLLISION ABILITY] Collision action must drive in Ability");
            }
        }
    }
   
    public void Execude()
    {
        foreach(var action in collisionsActionsIAbilities)
        {
            action.Target = new List<GameObject>();
            collisions.ForEach(c =>
            {
                if (c != null) action.Target.Add(c.gameObject);
            });
            action.Execude();
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        float3 position = gameObject.transform.position;
        switch (Colider)
        {
            case SphereCollider sphere:
                sphere.ToWorldSpaceSphere(out var sphepeCenter, out var sphereRadius);
                dstManager.AddComponentData(entity, new ColliderActorData
                {
                    ColliderType = ColliderType.Sphere,
                    SphereCenter = sphepeCenter - position,
                    SphereRadius = sphereRadius,
                    InitialTakeOff = true
                });
                break;
            case CapsuleCollider capsule:
                capsule.ToWorldSpaceCapsule(out var capsuleStart, out var capsuleEnd, out var capsuleRadius);
                dstManager.AddComponentData(entity, new ColliderActorData
                {
                    ColliderType = ColliderType.Capsule,
                    CapsuleStart = capsuleStart - position,
                    CapsuleEnd = capsuleEnd - position,
                    CapsuleRadius = capsuleRadius,
                    InitialTakeOff = true
                });
                break;
            case BoxCollider box:
                box.ToWorldSpaceBox(out var boxCenter, out var boxHalfExtens, out var boxOrentation);
                dstManager.AddComponentData(entity, new ColliderActorData
                {
                    ColliderType = ColliderType.Box,
                    BoxCenter = boxCenter - position,
                    BoxHalfExtens = boxHalfExtens,
                    BoxOrentation = boxOrentation,
                    InitialTakeOff = true
                });
                break;
        }
    }
}

    public enum ColliderType
    {
        Sphere = 0,
        Capsule = 1,
        Box = 2
    }

public struct ColliderActorData : IComponentData
{
    public ColliderType ColliderType;
    public float3 SphereCenter;
    public float SphereRadius;
    public float3 CapsuleStart;
    public float3 CapsuleEnd;
    public float CapsuleRadius;
    public float3 BoxCenter;
    public float3 BoxHalfExtens;
    public quaternion BoxOrentation;
    public bool InitialTakeOff;
}
