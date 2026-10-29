using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float _currentHealth;

    private void Awake() => _currentHealth = maxHealth;

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0f) Destroy(gameObject);
    }
}