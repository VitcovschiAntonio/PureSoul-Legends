using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
     [SerializeField] private Transform _player;
    [SerializeField] private AIFOV _FOV;


    [Header("Camera functionalities")]
    [SerializeField] private bool _playerSpotted;
    [SerializeField] private bool _triggerAlarm;


    [Header("Camera Sound")]
    [SerializeField] private AudioClip _moveSound;
    [SerializeField] private AudioClip _playerSpottedSound;
    private AudioSource _audioSource;


    
    [Header("Camera Details")]
    [SerializeField] private bool _isDisabled;
    [SerializeField] private float _followTargetSpeed = 150f;
    [SerializeField] private float _searchSpeed = 25f;
    [SerializeField] private bool _isStatic;
    private bool _isWaiting = false;
        private float _leftLimit = -60f;
    private float _rightLimit = 60f;
    private bool _isRotatingRight = true;

    public event Action<bool> OnPlayerSpottedAllarm;
    public event Action<bool> OnPlayerSpotted;



    void Start()
    {
        _FOV.OnPlayerSpotted += HandleIfPlayerSpotted;
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isDisabled)
        {
            HandleSound();
            FollowPlayer();
            Search();

            
        }
        
    }
     private IEnumerator DelayedVisibilityCheck(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_playerSpotted)
        {
            _triggerAlarm = true;
             OnPlayerSpottedAllarm?.Invoke(_triggerAlarm);

            yield break;
        }
        else
        {
            _triggerAlarm = false;
             OnPlayerSpottedAllarm?.Invoke(_triggerAlarm);

        }
    }
    private IEnumerator CameraWaitingTime(float delay)
    {
        _isWaiting = true;
        _audioSource.Stop();
        yield return new WaitForSeconds(delay);
        _isWaiting = false;
    }

  
    private void Search()
    {
        if (!_isStatic &&!_playerSpotted)
        {
            if (!_isWaiting)
            {
                Vector3 currentRotation = transform.localEulerAngles;
                float yAngle = currentRotation.y > 180 ? currentRotation.y - 360 : currentRotation.y;

                if (_isRotatingRight)
                {
                    yAngle += _searchSpeed * Time.deltaTime;
                    if (yAngle >= _rightLimit)
                    {
                        yAngle = _rightLimit;
                        _isRotatingRight = false;
                        StartCoroutine(CameraWaitingTime(2f));

                    }
                }
                else
                {
                    yAngle -= _searchSpeed * Time.deltaTime;
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
     
    private void FollowPlayer()
    {
        if(_playerSpotted)
        {
            StartCoroutine(DelayedVisibilityCheck(3));
            Vector3 direction = _player.position - transform.position;
            
        
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _followTargetSpeed * Time.deltaTime);
        }
        OnPlayerSpotted?.Invoke(_playerSpotted);
    }
     private void HandleSound()
    { 
        if (_playerSpotted)
        {
            if (_audioSource.clip != _playerSpottedSound)
            {
                _audioSource.Stop();
                _audioSource.clip = _playerSpottedSound;
                _audioSource.Play();
            }
        }
        else if (!_isWaiting)
        {
            if (_audioSource.clip != _moveSound)
            {
                _audioSource.Stop();
                _audioSource.clip = _moveSound;
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

     private void HandleIfPlayerSpotted(bool value)
    {
        _playerSpotted = value;
    }
}
