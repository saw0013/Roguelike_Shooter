using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CollisisonSystem : ComponentSystem
{
    private EntityQuery _collisionQuery;

    private Collider[] _results = new Collider[50];
    private List<Collider> _resultsList = new List<Collider>();

    protected override void OnCreate()
    {
        _collisionQuery = GetEntityQuery(ComponentType.ReadOnly<ColliderActorData>(), ComponentType.ReadOnly<Transform>());
    }

    protected override void OnUpdate()
    {
        var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entities.With(_collisionQuery).ForEach(
            (Entity entity, CollisionAbility collisionAbility, ref ColliderActorData colliderData) =>
            {
                if (collisionAbility == null) return;

                var gamaObject = collisionAbility.gameObject;
                float3 position = gamaObject.transform.position;
                Quaternion rotation = gamaObject.transform.rotation;

                collisionAbility?.collisions.Clear();

                int size = 0;

                switch (colliderData.ColliderType)
                {
                    case ColliderType.Sphere:
                        size = Physics.OverlapSphereNonAlloc(colliderData.SphereCenter + position, colliderData.SphereRadius, _results);
                        break;

                    case ColliderType.Capsule:
                        var center = ((colliderData.CapsuleStart + position) + (colliderData.CapsuleEnd + position)) / 2f;
                        var point1 = colliderData.CapsuleStart + position;
                        var point2 = colliderData.CapsuleEnd + position;
                        point1 = (float3) (rotation * (point1 - center)) + center;
                        point2 = (float3)(rotation * (point2 - center)) + center;
                        size = Physics.OverlapCapsuleNonAlloc(point1, point2, colliderData.CapsuleRadius, _results);
                        break;

                    case ColliderType.Box:
                        size = Physics.OverlapBoxNonAlloc(colliderData.BoxCenter + position, colliderData.BoxHalfExtens, _results ,colliderData.BoxOrentation * rotation);
                        break;
                }

                if(size > 0)
                {
                    for(int i = 0; i < size - 1; i++)
                    {
                        if (_results[i] == collisionAbility.Colider) i++;
                        collisionAbility.collisions.Add(_results[i]);
                        collisionAbility.Execude();
                    }
                }
            });
    }
}
