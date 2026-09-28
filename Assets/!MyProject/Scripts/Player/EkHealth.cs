using System;
using UnityEngine;
using UnityEngine.Assemblies;
using UnityEngine.UI;

public class EkHealth : MonoBehaviour
{

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [SerializeField] private Image _healthimage;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (_healthimage != null)
        {
            _healthimage.fillAmount = (float)currentHealth / maxHealth;
        }
    }


    private void Die()
    {
        if(currentHealth != 0)
        {
            isDead = true;
            Debug.Log("Ek has died.");
        }
    }
}
