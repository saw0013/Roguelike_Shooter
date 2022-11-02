using Unity.Entities;

public class CharacterShootSystem : ComponentSystem
{
    private EntityQuery _shootQuery;

    protected override void OnCreate()
    {
        _shootQuery = GetEntityQuery(ComponentType.ReadOnly<UserInputData>(),
            ComponentType.ReadOnly<ShootData>(),
            ComponentType.ReadOnly<InputData>());
    }

    protected override void OnUpdate()
    {
        Entities.With(_shootQuery).ForEach(
            (Entity entity, UserInputData userInput, ref InputData inputdata) =>
            {
                if(inputdata.Shoot > 0f && userInput.ShootAction != null && userInput.ShootAction is IAbility ability)
                {
                    ability.Execude();
                }
            });
    }
}
