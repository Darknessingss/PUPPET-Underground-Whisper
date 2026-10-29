using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 1500f;
    [SerializeField] private Slider healthSlider;

    [Header("Плавность слайдера")]
    [SerializeField] private float sliderLerpSpeed = 1.5f;

    private float _currentHealth;
    private float _targetSliderValue;
    private float _displayedSliderValue;
    private bool _isDead;

    private void Awake()
    {
        _currentHealth = maxHealth;
        _targetSliderValue = 1f;
        _displayedSliderValue = 1f;

        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
            healthSlider.value = 1f;
        }
    }

    private void Update()
    {
        if (healthSlider != null && !Mathf.Approximately(_displayedSliderValue, _targetSliderValue))
        {
            _displayedSliderValue = Mathf.MoveTowards(
                _displayedSliderValue,
                _targetSliderValue,
                sliderLerpSpeed * Time.deltaTime);

            healthSlider.value = _displayedSliderValue;
        }

        if (_isDead && Mathf.Approximately(_displayedSliderValue, 0f))
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        if (_isDead) return;

        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);

        _targetSliderValue = _currentHealth / maxHealth;

        if (_currentHealth <= 0f)
        {
            _isDead = true;
        }
    }
}
