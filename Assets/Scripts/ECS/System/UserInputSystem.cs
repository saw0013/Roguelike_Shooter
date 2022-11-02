using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputSystem : ComponentSystem
{
    private EntityQuery _inputQuery;

    private InputAction moveAction;

    private InputAction shootAction;
    private InputAction jerkAction;
    private InputAction invisibalAction;

    private float2 _moveInput;

    private float _shootInput;
    private float _jerkInput;
    private float _invisibalInput;

    protected override void OnCreate()
    {
        _inputQuery = GetEntityQuery(ComponentType.ReadOnly<InputData>());
    }

    protected override void OnStartRunning()
    {
        moveAction = new InputAction("move", binding: "<Gamepad>/rightStick");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.performed += context => { _moveInput = context.ReadValue<Vector2>(); };
        moveAction.started += context => { _moveInput = context.ReadValue<Vector2>(); };
        moveAction.canceled += context => { _moveInput = context.ReadValue<Vector2>(); };
        moveAction.Enable();

        shootAction = new InputAction("shoot", binding: "<Keyboard>/Space");
        shootAction.performed += context => { _shootInput = context.ReadValue<float>(); };
        shootAction.started += context => { _shootInput = context.ReadValue<float>(); };
        shootAction.canceled += context => { _shootInput = context.ReadValue<float>(); };
        shootAction.Enable();

        jerkAction = new InputAction("jerk", binding: "<Keyboard>/Enter");
        jerkAction.performed += context => { _jerkInput = context.ReadValue<float>(); };
        jerkAction.started += context => { _jerkInput = context.ReadValue<float>(); };
        jerkAction.canceled += context => { _jerkInput = context.ReadValue<float>(); };
        jerkAction.Enable();

        invisibalAction = new InputAction("ivisivals", binding: "<Keyboard>/Tab");
        invisibalAction.performed += context => { _invisibalInput = context.ReadValue<float>(); };
        invisibalAction.started += context => { _invisibalInput = context.ReadValue<float>(); };
        invisibalAction.canceled += context => { _invisibalInput = context.ReadValue<float>(); };
        invisibalAction.Enable();
    }

    protected override void OnStopRunning()
    {
        moveAction.Disable();
        shootAction.Disable();
        jerkAction.Disable();
        invisibalAction.Disable();
    }

    protected override void OnUpdate()
    {
        Entities.With(_inputQuery).ForEach(
            (Entity entity, ref InputData inputData) =>
            {
                inputData.Jerk = _jerkInput;
                inputData.Move = _moveInput;
                inputData.Shoot = _shootInput;
                inputData.Invisibal = _invisibalInput;
            });
    }
}
