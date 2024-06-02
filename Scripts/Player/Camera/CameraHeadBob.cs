using System.Collections;
using UnityEngine;

public class CameraHeadBob : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private PlayerMovement _movement;

    [Header("Settings")]
    [SerializeField] private float _walkBobSpeed = 14f;
    [SerializeField] [Range(0, 1)] private float _walkBobAmount = 0.05f;

    [SerializeField] private float _sprintBobSpeed = 18f;
    [SerializeField] [Range(0, 1)] private float _sprintBobAmount = 0.11f;

    [SerializeField] private float _crouchBobSpeed = 8f;
    [SerializeField] [Range(0, 1)] private float _crouchBobAmount = 0.025f;

    private float _defaultYPos;
    private float _timer;
    private bool _isGrounded;
    private bool _isMoving;
    private bool _isCrouching;
    private bool _isSprinting;

    private float _currentBobSpeed;
    private float _currentBobAmount;
    private float _targetBobSpeed;
    private float _targetBobAmount;

    void Awake()
    {
        _defaultYPos = _playerCamera.transform.localPosition.y;
    }

    void Start()
    {
        _movement.OnPlayerIsGrounded += HandleIsGrounded;
        _movement.OnPlayerMove += HandleMoveDirection;
        _movement.OnPlayerCrouch += HandleIsCrouching;
        _movement.OnPlayerSprint += HandleIsSprinting;

        _currentBobSpeed = _walkBobSpeed;
        _currentBobAmount = _walkBobAmount;
        _targetBobSpeed = _walkBobSpeed;
        _targetBobAmount = _walkBobAmount;
    }

    void Update()
    {
        HandleCurrentBobbing();
        HandleHeadBob();
       
    }

    private void HandleHeadBob()
    {
        if (!_isGrounded) { return; }

        if (_isMoving)
        {
            _timer += Time.deltaTime * _currentBobSpeed;
            float bobOffset = Mathf.Sin(_timer) * _currentBobAmount;
            _playerCamera.transform.localPosition = new Vector3(
                _playerCamera.transform.localPosition.x,
                _defaultYPos + bobOffset,
                _playerCamera.transform.localPosition.z
            );
        }
        else
        {
            _timer = 0f;  // Reset the timer when not moving
            _playerCamera.transform.localPosition = new Vector3(
                _playerCamera.transform.localPosition.x,
                Mathf.Lerp(_playerCamera.transform.localPosition.y, _defaultYPos, Time.deltaTime * _currentBobSpeed),
                _playerCamera.transform.localPosition.z
            );
        }
    }

    private void HandleCurrentBobbing()
    {
        if (_isSprinting)
        {
            _targetBobAmount = _sprintBobAmount;
            _targetBobSpeed = _sprintBobSpeed;
        }
        else if (_isCrouching)
        {
            _targetBobAmount = _crouchBobAmount;
            _targetBobSpeed = _crouchBobSpeed;
        }
        else
        {
            _targetBobAmount = _walkBobAmount;
            _targetBobSpeed = _walkBobSpeed;
        }

        _currentBobAmount = Mathf.Lerp(_currentBobAmount, _targetBobAmount, Time.deltaTime * 3f); // Smooth transition
        _currentBobSpeed = Mathf.Lerp(_currentBobSpeed, _targetBobSpeed, Time.deltaTime * 3f); // Smooth transition
    }

    private void HandleIsGrounded(bool value)
    {
        _isGrounded = value;
    }

    private void HandleMoveDirection(Vector2 value)
    {
        _isMoving = value.magnitude > 0;
    }

    private void HandleIsCrouching(bool value)
    {
        
        _isCrouching = value;
    }

    private void HandleIsSprinting(bool value)
    {
        _isSprinting = value;
    }
}
