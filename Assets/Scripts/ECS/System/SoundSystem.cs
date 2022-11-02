using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SoundSystem : ComponentSystem
{
    private bool _walk;

    private EntityQuery AnimQuery;

    protected override void OnCreate()
    {
        AnimQuery = GetEntityQuery(ComponentType.ReadOnly<StudioEventEmitter>(),
            ComponentType.ReadOnly<UserInputData>());
    }

    protected override void OnUpdate()
    {
        Entities.With(AnimQuery).ForEach(
            (Entity entity, ref InputData input, StudioEventEmitter sound, UserInputData userInput) =>
            {
                if (math.distance(0, Mathf.Abs(input.Move.x) + Mathf.Abs(input.Move.y)) > 0.01f && _walk == false)
                {
                    _walk = true;
                    userInput.aSoundInstance.start();
                }
                else if(math.distance(0, Mathf.Abs(input.Move.x) + Mathf.Abs(input.Move.y)) < 0.1f)
                {
                    _walk = false;
                    userInput.aSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    //userInput.aSoundInstance.release();
                }
            });
    }

}
