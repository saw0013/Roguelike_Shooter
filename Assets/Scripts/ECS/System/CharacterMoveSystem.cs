using Unity.Entities;
using UnityEngine;

public class CharacterMoveSystem : ComponentSystem
{
    private EntityQuery _moveQuery;

    protected override void OnCreate()
    {
        _moveQuery = GetEntityQuery(ComponentType.ReadOnly<UserInputData>(),
            ComponentType.ReadOnly<MoveData>(), 
            ComponentType.ReadOnly<Transform>());
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities.With(_moveQuery).ForEach(
            (Entity entity, Transform transform,  ref InputData inputData, ref MoveData moveData) => 
            {
                var pos = transform.position;
                var distance = moveData.Speed * deltaTime;
                pos += new Vector3(inputData.Move.x * distance, 0, inputData.Move.y * distance);
                transform.position = pos;

                if (Mathf.Abs(inputData.Move.x) > 0.01f || Mathf.Abs(inputData.Move.y) > 0.01f)
                {
                    //TODO : Поменять поворот куда смотрит

                    Vector3 relativePos = new Vector3(0, 0f, 0);
                    Quaternion targetRot = Quaternion.LookRotation(relativePos);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.DeltaTime * 20);
                }              
            });
    }
}
