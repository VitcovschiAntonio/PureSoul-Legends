using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _cc;
    private Vector2 _inputDirection;

    [SerializeField] private bool _isGrounded;

    [Header("Movement Speeds")]
    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _targetSpeed;
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _crouchSpeed = 2f;
    [SerializeField] private float _sprintSpeed = 7.5f;
    [SerializeField] private float _movementSpeedRatio = 3f;
    private Vector3 _moveDirection;
    private Vector2 _currentInput;

    [Header("Movement Logic")]
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isSprinting;
    [SerializeField] private bool _isCrouching;
    [SerializeField] private bool _isJumping;
    [Header("Inputs")]
    [SerializeField] private KeyCode crouchButton = KeyCode.C;

    private bool _crouchBtnPressed => Input.GetKeyDown(crouchButton);

    [Header("Crouch Data")]
    [SerializeField] private Transform _uncrouchCheck;
    [SerializeField] [Range(0, 1)] private float _crouchHeight = 0.5f;
    [SerializeField] private float _standingHeight = 2f;
    [SerializeField] [Range(0, 1)] private float _timeToCrouch = 0.25f;
    [SerializeField] private Vector3 _crouchingCenter = new Vector3(0, 0.35f, 0);
    [SerializeField] private Vector3 _standingCenter = new Vector3(0, 1f, 0);

    private bool _isCrouched = false;
    private bool _isTransitioning = false;

    [Header("Jump Data")]
    [SerializeField] private bool _canJump;
    [SerializeField] private bool _jumpBtnPressed;
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _gravity = 12.81f;

    [Header("Sprint Data")]
    [SerializeField] private bool _sprintBtnPressed;
    [SerializeField] private bool _canSprint;

    public event Action<Vector2> OnPlayerMove;
    public event Action<bool> OnPlayerIsGrounded;
    public event Action<bool> OnPlayerCrouch;
    public event Action<bool> OnPlayerSprint;

    void Start()
    {
        _cc = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();

        _playerInput.OnPlayerMove += HandleMoveInput;
        _playerInput.OnPlayerJump += HandleJumpInput;
        _playerInput.OnPlayerSprint += HandleRunInput;

        _standingHeight = _cc.height;

        _canSprint = true;
        _isCrouching = false;
    }

    void FixedUpdate()
    {
        _isGrounded = _cc.isGrounded;
        Move();
        ChangeCurrentSpeed();
    }

    void Update()
    {
        OnPlayerIsGrounded?.Invoke(_isGrounded);


        Sprint();
        NormalJump();
        Crouch();
    }

    private void Crouch()
    {
        if (_crouchBtnPressed)
        {
            StartCoroutine(CrouchStand());
        }
        OnPlayerCrouch?.Invoke(_isCrouched);


    }

    private IEnumerator CrouchStand()
    {
        if (_isCrouched && Physics.Raycast(_uncrouchCheck.position, Vector3.up, 1f))
        {
            yield break;
        }

        _isTransitioning = true;

        float timeElapsed = 0f;
        float currentHeight = _cc.height;
        Vector3 currentCenter = _cc.center;

        float targetHeight = _isCrouched ? _standingHeight : _crouchHeight;
        Vector3 targetCenter = _isCrouched ? _standingCenter : _crouchingCenter;

        while (timeElapsed < _timeToCrouch)
        {
            _cc.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / _timeToCrouch);
            _cc.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / _timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _cc.height = targetHeight;
        _cc.center = targetCenter;

        _isCrouched = !_isCrouched;
        _isTransitioning = false;
    }

    private void NormalJump()
    {
        if (_isGrounded) { _canJump = true; }
        else { _canJump = false; }

        if (_canJump && _jumpBtnPressed)
        {
            _moveDirection.y = _jumpForce;
            _jumpBtnPressed = false;
        }
    }

    private void DoubleJump()
    {
        if (_isGrounded) { _canJump = true; }

        if (_canJump && _jumpBtnPressed)
        {
            _moveDirection.y = _jumpForce;
            _canJump = false;
            _jumpBtnPressed = false;
        }
    }

    private void Sprint()
    {
        if (_sprintBtnPressed && _canSprint && !_isCrouched)
        {
            _isSprinting = true;
        }
        else
        {
            _isSprinting = false;
        }
        OnPlayerSprint?.Invoke(_isSprinting);
    }

    private void ChangeCurrentSpeed()
    {
        if (_isCrouched)
        {
            _targetSpeed = _crouchSpeed;
        }
        else if (_isSprinting)
        {
            _targetSpeed = _sprintSpeed;
        }
        else
        {
            _targetSpeed = _walkSpeed;
        }
        _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, 3f * Time.deltaTime);
    }

    private void Move()
    {
        OnPlayerMove?.Invoke(_inputDirection);
        _currentInput = new Vector2(_currentSpeed * _inputDirection.x, _currentSpeed * _inputDirection.y);

        float moveDirectionY = _moveDirection.y;
        _moveDirection = (transform.TransformDirection(Vector3.forward) * _currentInput.y) + (transform.TransformDirection(Vector3.right) * _currentInput.x);
        _moveDirection.y = moveDirectionY;

        if (!_isGrounded)
        {
            _moveDirection.y -= _gravity * Time.deltaTime;
        }

        _cc.Move(_moveDirection * Time.deltaTime);
    }

    private void HandleMoveInput(Vector2 value)
    {
        _inputDirection = value;
        _inputDirection.Normalize();
    }

    private void HandleJumpInput(float value)
    {
        if (value > 0) { _jumpBtnPressed = true; }
        else { _jumpBtnPressed = false; }
    }

    private void HandleRunInput(float value)
    {
        if (value > 0) { _sprintBtnPressed = true; }
        else { _sprintBtnPressed = false; }
    }

    void OnDisable()
    {
        _playerInput.OnPlayerMove -= HandleMoveInput;
        _playerInput.OnPlayerJump -= HandleJumpInput;
        _playerInput.OnPlayerSprint -= HandleRunInput;
    }
}
