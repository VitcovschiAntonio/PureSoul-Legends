using System;
using System.Collections;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    [SerializeField] private AudioClip _alarmSoundClip;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private SecurityCamera[] _cameraList;
    private bool _alarmRinging = false;
    private bool _alarmTriggered = false;
    private Coroutine _stopAlarmCoroutine;
    private bool _isDisabled;

    public Action<bool> OnAlarmStatus;

    void Start()
    {
        // For situations when multiple cameras are linked to one alarm
        foreach (SecurityCamera camera in _cameraList)
        {
            camera.OnPlayerSpottedAllarm += HandleAlarmTriggeredValue;
        }
    }

    void Update()
    {
        // 
        if(!_isDisabled)
        {
             HandleAlarmSound();

        }
        
    }

    private void HandleAlarmSound()
    {
        if (_alarmTriggered)
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = _alarmSoundClip;
                _audioSource.loop = true;
                _audioSource.Play();
                _alarmRinging = true;
                OnAlarmStatus?.Invoke(_alarmRinging);
            }

            // If the alarm is triggered, stop any existing coroutine to stop the alarm
            if (_stopAlarmCoroutine != null)
            {
                StopCoroutine(_stopAlarmCoroutine);
                _stopAlarmCoroutine = null;
            }
        }
        else
        {
            // Start the coroutine to stop the alarm if not already running
            if (_stopAlarmCoroutine == null)
            {
                
                _stopAlarmCoroutine = StartCoroutine(StopAlarmAfterDelay(10f));
            }
        }
    }

    private IEnumerator StopAlarmAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _alarmRinging = false;
        OnAlarmStatus?.Invoke(_alarmRinging);
        _audioSource.Stop();
        _stopAlarmCoroutine = null;
    }

    private void HandleAlarmTriggeredValue(bool value)
    {
        _alarmTriggered = value;
    }
}
