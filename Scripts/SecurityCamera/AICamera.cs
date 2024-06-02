using System;
using System.Collections;
using UnityEngine;

public class AICamera : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private AIFOV _FOV;
    [SerializeField] private bool _playerSpotted;
    
    [Header("Camera Sound")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _movingSound;
    [SerializeField] private AudioClip _playerFoundSound;

    [Header("Camera Settings")]
    [SerializeField] private float _searchRotationSpeed = 45f;
    [SerializeField] private float _lockOnPlayerSpeed = 150f;
    [SerializeField] private float _angleRotation;

    [Header("Camera functionalities")]
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isDisabled = false;
 
    [SerializeField] private bool _alarmTriggered = false;
 

    public event Action<bool> OnPlayerSpotted;

    private float _leftLimit = -60f;
    private float _rightLimit = 60f;
    private bool _isRotatingRight = true;
    private bool _isWaiting = false;

    void Start()
    {
        _FOV.OnPlayerSpotted += HandleIfPlayerSpotted;
    }

    void Update()
    {
        Search();
        //Lock();
      //  TriggerAlarm();
       // HandleSound();
       Debug.Log(_playerSpotted);
        
    }

    private void HandleSound()
    { 
        if (_playerSpotted)
        {
            if (_audioSource.clip != _playerFoundSound)
            {
                _audioSource.Stop();
                _audioSource.clip = _playerFoundSound;
                _audioSource.Play();
            }
        }
        else if (!_isWaiting)
        {
            if (_audioSource.clip != _movingSound)
            {
                _audioSource.Stop();
                _audioSource.clip = _movingSound;
                _audioSource.loop = true;
                _audioSource.Play();
            }
            else if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
        else
        {
            _audioSource.Stop();
        }
    }

    private void Search()
    {
        if (_isMoving && !_isDisabled && !_playerSpotted)
        {
            if (!_isWaiting)
            {
                Vector3 currentRotation = transform.localEulerAngles;
                float yAngle = currentRotation.y > 180 ? currentRotation.y - 360 : currentRotation.y;

                if (_isRotatingRight)
                {
                    yAngle += _searchRotationSpeed * Time.deltaTime;
                    if (yAngle >= _rightLimit)
                    {
                        yAngle = _rightLimit;
                        _isRotatingRight = false;
                        StartCoroutine(CameraWaitingTime(2f));
                    }
                }
                else
                {
                    yAngle -= _searchRotationSpeed * Time.deltaTime;
                    if (yAngle <= _leftLimit)
                    {
                        yAngle = _leftLimit;
                        _isRotatingRight = true;
                        StartCoroutine(CameraWaitingTime(2f));
                    }
                }

                currentRotation.y = yAngle < 0 ? yAngle + 360 : yAngle;
                transform.localEulerAngles = currentRotation;
            }
        }
    }

    private IEnumerator CameraWaitingTime(float delay)
    {
        _isWaiting = true;
        _audioSource.Stop();
        yield return new WaitForSeconds(delay);
        _isWaiting = false;
    }

    private void Lock()
    {
        if (_playerSpotted)
        {
            // //StartCoroutine(DelayedVisibilityCheck(3));
            // Vector3 direction = _playerPosition.position - transform.position;
            // float angleToPlayer = Vector3.Angle(transform.forward, direction);
      
            // if (angleToPlayer <= _angleRotation)
            // {
            //     Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            //     transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _lockOnPlayerSpeed * Time.deltaTime);
            // }
            Vector3 direction = _player.position - transform.position;
            

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _lockOnPlayerSpeed * Time.deltaTime); 
        }
    }

    private void TriggerAlarm()
    {
        if (_alarmTriggered)
        {
           
        }
        if (_alarmTriggered && !_playerSpotted)
        {
            _alarmTriggered = false;
           
        }
         OnPlayerSpotted?.Invoke(_alarmTriggered);
    }

    private IEnumerator DelayedVisibilityCheck(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_playerSpotted && !_alarmTriggered)
        {
            _alarmTriggered = true;
            yield break;
        }
        else
        {
            _alarmTriggered = false;
        }
    }

    private void HandleIfPlayerSpotted(bool value)
    {
        _playerSpotted = value;
    }
}
