using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CharacterAnimSystem : ComponentSystem
{
    private EntityQuery AnimQuery;

    protected override void OnCreate()
    {
        AnimQuery = GetEntityQuery(ComponentType.ReadOnly<Animator>(),
            ComponentType.ReadOnly<AnimData>());
    }

    protected override void OnUpdate()
    {
        Entities.With(AnimQuery).ForEach(
            (Entity entity, ref InputData input, Animator anim, UserInputData userInput) =>
            {
               //anim.SetBool(userInput.MoveAnimHash, math.distance(0, Mathf.Abs(input.Move.x) + Mathf.Abs(input.Move.y)) > 0.01f);
               //
               //anim.SetBool(userInput.ShootAnimHash, input.Shoot > 0f);

                //if (userInput.health.Health <= 0) anim.SetTrigger(userInput.DeadAnimHash);

               // if (userInput.health.isDamage)
               // {
               //     anim.SetTrigger(userInput.HitAnimHash);
               //     userInput.health.isDamage = false;
               // }

               // if (userInput.MoveSpeedAnimHash == string.Empty) return;

                float speed = userInput.speed * math.distance(0, Mathf.Abs(input.Move.x) + Mathf.Abs(input.Move.y));

                if (speed > 50) speed = 50;

                if (input.Move.x > .5f) input.Move.x = .5f;
                if (input.Move.x < -.5f) input.Move.x = -.5f;
                
                if (input.Move.y > .5f) input.Move.y = .5f;
                if (input.Move.y < -.5f) input.Move.y = -.5f;

                anim.SetFloat(userInput.Strafe, input.Move.x);
                anim.SetFloat(userInput.Forward, input.Move.y);


                //  anim.SetFloat(userInput.MoveSpeedAnimHash, speed);
            });
    }
}