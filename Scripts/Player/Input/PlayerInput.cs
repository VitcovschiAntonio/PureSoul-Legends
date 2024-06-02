using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Doing
/// </summary>
public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    
    public event Action<Vector2> OnPlayerMove;
    public event Action<float> OnPlayerJump;
    public event Action<float> OnPlayerCrouch;
    public event Action<float> OnPlayerSprint;
    public event Action<float> OnPlayerAttack;
    public event Action<float> OnPlayerReload;
    public event Action<float> OnPlayerAim;

    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        _playerInputActions.Enable();
    //Movement
        _playerInputActions.Movement.Move.performed += InputMovePerformed;
        _playerInputActions.Movement.Move.canceled += InputMovePerformed;

       

        _playerInputActions.Movement.Crouch.started += InputCrouchPerformed;
        _playerInputActions.Movement.Crouch.canceled += InputCrouchPerformed;

        _playerInputActions.Movement.Sprint.performed += InputSprintPerformed;
        _playerInputActions.Movement.Sprint.canceled += InputSprintPerformed;

        _playerInputActions.Movement.Jump.performed += InputJumpPerformed;
        _playerInputActions.Movement.Jump.canceled += InputJumpPerformed;
    
    //Weapon
        _playerInputActions.Weapon.Reload.performed += InputReloadPerformed;
        _playerInputActions.Weapon.Reload.canceled += InputReloadPerformed;

        _playerInputActions.Weapon.Aim.performed += InputAimPerformed;
        _playerInputActions.Weapon.Aim.canceled += InputAimPerformed;

        _playerInputActions.Weapon.Attack.performed += InputAttackPerformed;
        _playerInputActions.Weapon.Attack.canceled += InputAttackPerformed;




    
        
    }
    void OnDisable()
    {
        _playerInputActions.Disable();
    //Movement
        _playerInputActions.Movement.Move.performed -= InputMovePerformed;
        _playerInputActions.Movement.Move.canceled -= InputMovePerformed;

       

        _playerInputActions.Movement.Crouch.performed -= InputCrouchPerformed;
        _playerInputActions.Movement.Crouch.canceled -= InputCrouchPerformed;

        _playerInputActions.Movement.Sprint.performed -= InputSprintPerformed;
        _playerInputActions.Movement.Sprint.canceled -= InputSprintPerformed;

        _playerInputActions.Movement.Jump.started -= InputJumpPerformed;
        _playerInputActions.Movement.Jump.canceled -= InputJumpPerformed;
    }

    private void InputMovePerformed(InputAction.CallbackContext ctx)
    {
        OnPlayerMove?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void InputJumpPerformed(InputAction.CallbackContext ctx)
    {
        
        OnPlayerJump?.Invoke(ctx.ReadValue<float>());
    }
    private void InputCrouchPerformed(InputAction.CallbackContext ctx)
    {
        OnPlayerCrouch?.Invoke(ctx.ReadValue<float>());
    }
    
    private void InputSprintPerformed(InputAction.CallbackContext ctx)
    {
        OnPlayerSprint?.Invoke(ctx.ReadValue<float>());
    }

    private void InputAttackPerformed(InputAction.CallbackContext ctx)
    {
        OnPlayerAttack?.Invoke(ctx.ReadValue<float>());
    }
    private void InputReloadPerformed(InputAction.CallbackContext ctx)
    {
        OnPlayerReload?.Invoke(ctx.ReadValue<float>());
    }
    private void InputAimPerformed(InputAction.CallbackContext ctx)
    {
        OnPlayerAim?.Invoke(ctx.ReadValue<float>());
    }

}
