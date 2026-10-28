using System;
using UnityEngine;
using UnityEngine.UI;

public class EkHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    [Header("Damage")]
    [SerializeField] private float damagePerSecondWhenNoSoul = 10f;

    [Header("Regeneration")]
    [SerializeField] private float soulThresholdForHeal = 50f;
    [SerializeField] private float healPerSecond = 3f;

    [SerializeField] private EkSoul soul;
    [SerializeField] private Image healthFill;
    [SerializeField] private PlayerDeath death;

    private float _currentHealth;

    public float CurrentHealth => _currentHealth;
    public float Normalized => _currentHealth / maxHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
        healthFill.fillAmount = 1f;
    }

    private void Update()
    {
        if (soul.CurrentSoul <= 0f)
            _currentHealth -= damagePerSecondWhenNoSoul * Time.deltaTime;

        else if (soul.CurrentSoul > soulThresholdForHeal)
            _currentHealth += healPerSecond * Time.deltaTime;

        _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);

        healthFill.fillAmount = Normalized;

        if (_currentHealth <= 0f)
            death.TryDie();
    }
}