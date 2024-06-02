using System;
using UnityEngine;

public class AIFOV : MonoBehaviour
{
    [SerializeField] private Transform _playerPosition;
    [SerializeField] private Transform _peakFOVPosition;
    [SerializeField] private LayerMask _layerMask;

    private bool _playerInFov;
    private bool _playerSpotted;

    public event Action<bool> OnPlayerSpotted;

    private void Start()
    {
        _layerMask = LayerMask.GetMask("Player", "Obstacle");
    }

    private void Update()
    {
        CheckPlayerVisibility();
        OnPlayerSpotted?.Invoke(_playerSpotted); // Invoke the event to notify listeners
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MyPlayer"))
            _playerInFov = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MyPlayer"))
            _playerInFov = false;
    }

    private void CheckPlayerVisibility()
    {
        if (_playerInFov)
        {
            Ray ray = new Ray(_peakFOVPosition.position, _playerPosition.position - _peakFOVPosition.position);
            Debug.DrawRay(_peakFOVPosition.position, _playerPosition.position - _peakFOVPosition.position, Color.red);
            
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _layerMask))
            {
                _playerSpotted = hitInfo.collider.CompareTag("MyPlayer");
            }
            else
            {
                _playerSpotted = false;
            }
        }
        else
        {
            _playerSpotted = false;
        }
    }
}
