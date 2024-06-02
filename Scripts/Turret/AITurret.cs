using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITurret : MonoBehaviour
{
    [SerializeField] private Alarm _alarm;
    [SerializeField] private SecurityCamera[] _cameras;
    [SerializeField] private AIFOV _FOV;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _gunRotator;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _turnedOnSound;
    [SerializeField] private AudioClip _turnedOffSound;
    [SerializeField] private AudioClip _rotatingSound;
    [SerializeField] private AudioClip _shootingSound;

    [SerializeField] private bool _isDisabled;
    [SerializeField] private bool _activated;
    [SerializeField] private float _followTargetSpeed;
    [SerializeField] private float _searchSpeed;
    [SerializeField] private float _rightLimit;
    [SerializeField] private float _leftLimit;

    private bool _alarmTriggered;
    private bool _isRotatingRight;
    private bool _isPlayerSpottedByCamera;
    private bool _isPlayerSpottedByThis;
    private bool _playerLocationKnown;
    private bool _isRotating;

    private Quaternion _previousRotation;

    private const float rotationEpsilon = 0.01f;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _alarm.OnAlarmStatus += GetAlarmStatus;
        _FOV.OnPlayerSpotted += GetPlayerSpottedThis;
        foreach (SecurityCamera camera in _cameras)
        {
            camera.OnPlayerSpotted += GetPlayerSpottedCamera;
        }

        _previousRotation = _gunRotator.rotation;
    }

    void Update()
    {
        if (!_isDisabled)
        {
            Activate();
            LockAndFire();
            CheckRotation();
        }
        else
        {
            StopAllSounds();
        }
    }

    private void Activate()
    {
        if (_alarmTriggered && !_playerLocationKnown)
        {
            if (!_activated)
            {
                _activated = true;
                _audioSource.PlayOneShot(_turnedOnSound);
                Debug.Log("Activated and searching");
            }

            SearchForPlayer();
        }
        else if (!_alarmTriggered)
        {
            if (_activated)
            {
                _activated = false;
                _audioSource.PlayOneShot(_turnedOffSound);
                Debug.Log("Deactivating");
                StopAllSounds();
            }
        }
    }

    private void SearchForPlayer()
    {
        if (!_audioSource.isPlaying || _audioSource.clip != _rotatingSound)
        {
            _audioSource.clip = _rotatingSound;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        Vector3 currentRotation = transform.localEulerAngles;
        float yAngle = currentRotation.y > 180 ? currentRotation.y - 360 : currentRotation.y;

        if (_isRotatingRight)
        {
            yAngle += _searchSpeed * Time.deltaTime;
            if (yAngle >= _rightLimit)
            {
                yAngle = _rightLimit;
                _isRotatingRight = false;
            }
        }
        else
        {
            yAngle -= _searchSpeed * Time.deltaTime;
            if (yAngle <= _leftLimit)
            {
                yAngle = _leftLimit;
                _isRotatingRight = true;
            }
        }

        currentRotation.y = yAngle < 0 ? yAngle + 360 : yAngle;
        _gunRotator.transform.localEulerAngles = currentRotation;
    }

    private void LockAndFire()
    {
        if (_activated && (_isPlayerSpottedByThis || _isPlayerSpottedByCamera))
        {
            _playerLocationKnown = true;

            Vector3 direction = _player.position - _gunRotator.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            _gunRotator.rotation = Quaternion.RotateTowards(_gunRotator.rotation, targetRotation, _followTargetSpeed * Time.deltaTime);

            Debug.Log("Looking at Player");

            if (_isPlayerSpottedByThis && (!_audioSource.isPlaying || _audioSource.clip != _shootingSound))
            {
                _audioSource.Stop();
                _audioSource.clip = _shootingSound;
                _audioSource.loop = false;
                _audioSource.Play();
            }
        }
        else
        {
            _playerLocationKnown = false;
            if (_audioSource.isPlaying && _audioSource.clip == _shootingSound)
            {
                _audioSource.Stop();
            }
        }
    }

    private void CheckRotation()
    {
        bool wasRotating = _isRotating;
        _isRotating = Quaternion.Angle(_gunRotator.rotation, _previousRotation) > rotationEpsilon;

        if (!_isRotating && wasRotating && _audioSource.isPlaying && _audioSource.clip == _rotatingSound)
        {
            _audioSource.Stop();
        }

        _previousRotation = _gunRotator.rotation;
    }

    private void StopAllSounds()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }

    private void GetAlarmStatus(bool value)
    {
        _alarmTriggered = value;
    }

    private void GetPlayerSpottedCamera(bool value)
    {
        _isPlayerSpottedByCamera = value;
    }

    private void GetPlayerSpottedThis(bool value)
    {
        _isPlayerSpottedByThis = value;
    }
}
