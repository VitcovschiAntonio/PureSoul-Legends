using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private GameObject _ThisObject;
    private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;

    void Start()
    {
        _currentHealth = _maxHealth;
    }



    public void TakeDamage(float damage)
    {
        if(_currentHealth - damage < 0)
        {
            Die();
        }
        else{
            _currentHealth -= damage;
        }
    }

    private void Heal(float ammount)
    {
        if(_currentHealth + ammount > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
        else{
            _currentHealth += ammount;
        }
    }

    private void Die()
    {
        if(_currentHealth < 0)
        {
           Debug.Log("destroyed");
        }
    }
}
