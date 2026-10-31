using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Slider healthSlider;

    [Header("Плавность слайдера")]
    [SerializeField] private float sliderLerpSpeed = 1.5f;

    [Header("Эффект получения урона")]
    [SerializeField] private Color hitFlashColor = Color.red;
    [SerializeField] private float hitFlashDuration = 0.15f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float _currentHealth;
    private float _targetSliderValue;
    private float _displayedSliderValue;
    private bool _isDead;

    private Color _originalColor;
    private Coroutine _flashCoroutine;

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

            _originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (healthSlider != null && _displayedSliderValue != _targetSliderValue)
        {
            _displayedSliderValue = Mathf.MoveTowards(
                _displayedSliderValue,
                _targetSliderValue,
                sliderLerpSpeed * Time.deltaTime);

            healthSlider.value = _displayedSliderValue;
        }

        if (_isDead && _displayedSliderValue <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        if (_isDead) 
            return;

        _currentHealth = Mathf.Max(_currentHealth - amount, 0f);
        _targetSliderValue = _currentHealth / maxHealth;

        if (spriteRenderer != null)
        {
            if (_flashCoroutine != null)
                StopCoroutine(_flashCoroutine);
            _flashCoroutine = StartCoroutine(HitFlashRoutine());
        }

        if (_currentHealth <= 0f)
            _isDead = true;
    }

    private IEnumerator HitFlashRoutine()
    {
        spriteRenderer.color = hitFlashColor;

        yield return new WaitForSeconds(hitFlashDuration);

        spriteRenderer.color = _originalColor;
        _flashCoroutine = null;
    }
}