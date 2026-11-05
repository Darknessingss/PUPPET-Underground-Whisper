using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Slider Smoothness")]
    [SerializeField] private float sliderLerpSpeed = 1.5f;

    [Header("Hit Flash Effect")]
    [SerializeField] private Color hitFlashColor = Color.red;
    [SerializeField] private float hitFlashDuration = 0.15f;

    private float _currentHealth;
    private float _targetSliderValue;
    private float _displayedSliderValue;
    private bool _isDead;
    private bool _hasSlider;
    private bool _hasRenderer;

    private Color _originalColor;
    private Coroutine _flashCoroutine;
    private WaitForSeconds _flashWait;

    private EkSoul _playerSoul;
    private bool _soulResolved;

    private void Awake()
    {
        if (enemyData != null)
            maxHealth = enemyData.MaxHealth;

        _currentHealth = maxHealth;
        _targetSliderValue = 1f;
        _displayedSliderValue = 1f;

        _hasSlider = healthSlider != null;
        if (_hasSlider)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
            healthSlider.value = 1f;
        }

        _hasRenderer = spriteRenderer != null;
        if (_hasRenderer)
            _originalColor = spriteRenderer.color;

        _flashWait = new WaitForSeconds(hitFlashDuration);
    }

    private void Update()
    {
        if (!_hasSlider)
            return;

        if (_displayedSliderValue != _targetSliderValue)
        {
            _displayedSliderValue = Mathf.MoveTowards(
                _displayedSliderValue,
                _targetSliderValue,
                sliderLerpSpeed * Time.deltaTime);
            healthSlider.value = _displayedSliderValue;
        }

        if (_isDead && _displayedSliderValue <= 0f)
            Destroy(gameObject);
    }

    public void TakeDamage(float amount)
    {
        if (_isDead || amount <= 0f)
            return;

        _currentHealth = Mathf.Max(_currentHealth - amount, 0f);
        _targetSliderValue = _currentHealth / maxHealth;

        EnemyAudioService.Instance.PlayHit(enemyData);

        if (_hasRenderer)
        {
            if (_flashCoroutine != null)
                StopCoroutine(_flashCoroutine);
            _flashCoroutine = StartCoroutine(HitFlashRoutine());
        }

        if (_currentHealth <= 0f)
        {
            _isDead = true;
            EnemyAudioService.Instance.PlayDeath(enemyData);

            var soul = ResolvePlayerSoul();
            if (soul != null && enemyData != null)
                soul.RestoreSoul(enemyData.SoulReward);

            if (!_hasSlider)
                Destroy(gameObject);
        }
    }

    private EkSoul ResolvePlayerSoul()
    {
        if (_soulResolved && _playerSoul != null)
            return _playerSoul;

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        _playerSoul = playerObj != null ? playerObj.GetComponent<EkSoul>() : null;
        _soulResolved = true;
        return _playerSoul;
    }

    private IEnumerator HitFlashRoutine()
    {
        spriteRenderer.color = hitFlashColor;
        yield return _flashWait;
        spriteRenderer.color = _originalColor;
        _flashCoroutine = null;
    }
}