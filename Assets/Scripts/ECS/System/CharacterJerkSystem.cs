using Unity.Entities;

public class CharacterJerkSystem : ComponentSystem
{
    private EntityQuery jerkQuery;

    protected override void OnCreate()
    {
        jerkQuery = GetEntityQuery(ComponentType.ReadOnly<InputData>(),
            ComponentType.ReadOnly<UserInputData>());
    }

    protected override void OnUpdate()
    {
        Entities.With(jerkQuery).ForEach( 
            (Entity entity, UserInputData userInput, ref InputData inputData) =>
            {
                if(inputData.Jerk > 0f && userInput.jerkAction != null && userInput.jerkAction is IAbility ability)
                {
                    ability.Execude();
                }
            });
    }
}
