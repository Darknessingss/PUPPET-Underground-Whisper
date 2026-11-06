using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class DeathScreenManager : MonoBehaviour
{
    [Header("Корневой объект смерти")]
    [SerializeField] private GameObject deathRoot;

    [Header("Микшер")]
    [SerializeField] private AudioMixerGroup mixerGroup;

    [Header("Чёрный фон")]
    [SerializeField] private Image blackScreen;
    [SerializeField] private float blackFadeDuration = 0.8f;
    [SerializeField] private AnimationCurve blackFadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Сердце")]
    [SerializeField] private Image heartImage;
    [SerializeField] private float appearDuration = 0.6f;
    [SerializeField] private AnimationCurve appearCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float startScale = 10f;
    [SerializeField] private float endScale = 450f;

    [Header("Звуки")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip shakeSound;
    [SerializeField] private AudioClip explosionSound;

    [Header("Тряска")]
    [SerializeField] private float shakeStart = 1f;
    [SerializeField] private float shakeEnd = 5f;
    [SerializeField] private float shakeStrength = 18f;
    [SerializeField] private float shakeFrequency = 32f;

    [Header("Взрыв (Анимация)")]
    [SerializeField] private Animator explosionAnimator;
    [SerializeField] private string explosionStateName = "heart_animation_death";
    [SerializeField] private float explosionDuration = 0.35f;

    [Header("Плашка смерти")]
    [SerializeField] private RectTransform deathPanelRect;
    [SerializeField] private float slideStartTime = 4f;
    [SerializeField] private float slideDuration = 5f;
    [SerializeField] private float slideOffsetY = -900f;
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Контент панели")]
    [SerializeField] private GameObject deathContent;

    [Header("Блокировка других UI")]
    [SerializeField] private ManagerInterface mainUiManager;

    [Header("Музыка")]
    [SerializeField] private MusicManager musicManager;

    public bool IsCasting { get; private set; }
    public event Action OnDeathSequenceComplete;

    private Vector2 _heartBasePosition;
    private Vector2 _panelTargetPosition;

    private AudioSource _deathSource;
    private AudioSource _shakeSource;
    private AudioSource _explosionSource;

    private void Awake()
    {
        _deathSource = CreateAudioSource("DeathSource");
        _shakeSource = CreateAudioSource("ShakeSource");
        _explosionSource = CreateAudioSource("ExplosionSource");

        if (deathRoot != null) deathRoot.SetActive(false);
        if (deathContent != null) deathContent.SetActive(false);

        _heartBasePosition = ((RectTransform)heartImage.transform).anchoredPosition;

        if (deathPanelRect != null)
        {
            _panelTargetPosition = deathPanelRect.anchoredPosition;
            deathPanelRect.gameObject.SetActive(false);
        }

        if (blackScreen != null)
        {
            Color blackColor = blackScreen.color;
            blackColor.a = 0f;
            blackScreen.color = blackColor;
            blackScreen.gameObject.SetActive(false);
        }
    }

    private AudioSource CreateAudioSource(string sourceName)
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.name = sourceName;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.ignoreListenerPause = true;

        if (mixerGroup != null)
            audioSource.outputAudioMixerGroup = mixerGroup;

        return audioSource;
    }

    public void PlayDeathSequence()
    {
        if (IsCasting)
            return;
        IsCasting = true;

        if (mainUiManager != null) mainUiManager.enabled = false;
        if (musicManager != null) musicManager.StopAllMusic();

        if (deathRoot != null) deathRoot.SetActive(true);
        if (blackScreen != null) blackScreen.gameObject.SetActive(true);
        if (deathPanelRect != null) deathPanelRect.gameObject.SetActive(false);
        if (deathContent != null) deathContent.SetActive(false);

        heartImage.gameObject.SetActive(false);

        _deathSource.Stop();
        _shakeSource.Stop();
        _explosionSource.Stop();

        StartCoroutine(SequenceRoutine());
    }

    private IEnumerator SequenceRoutine()
    {
        float sequenceStartTime = Time.unscaledTime;

        PlaySound(_deathSource, deathSound);

        yield return FadeBlackRoutine(0f, 1f, blackFadeDuration, blackFadeCurve);

        RectTransform heartRect = (RectTransform)heartImage.transform;
        heartImage.gameObject.SetActive(true);
        heartRect.anchoredPosition = _heartBasePosition;
        heartRect.localScale = Vector3.one * startScale;

        yield return AppearHeartRoutine(heartRect);

        StartCoroutine(ExplosionRoutine());

        float elapsedTime = Time.unscaledTime - sequenceStartTime;
        if (elapsedTime < shakeStart)
            yield return new WaitForSecondsRealtime(shakeStart - elapsedTime);

        if (shakeSound != null)
        {
            _shakeSource.clip = shakeSound;
            _shakeSource.loop = true;
            _shakeSource.Play();
        }

        float shakeDuration = Mathf.Max(0f, shakeEnd - shakeStart);
        yield return ShakeHeartRoutine(heartRect, shakeDuration);

        if (_shakeSource.isPlaying)
            _shakeSource.Stop();

        PlaySound(_explosionSource, explosionSound);

        elapsedTime = Time.unscaledTime - sequenceStartTime;
        if (elapsedTime < slideStartTime)
            yield return new WaitForSecondsRealtime(slideStartTime - elapsedTime);

        if (deathPanelRect != null)
        {
            deathPanelRect.gameObject.SetActive(true);
            yield return SlideInRoutine(deathPanelRect);
        }

        if (deathContent != null)
            deathContent.SetActive(true);

        IsCasting = false;
        OnDeathSequenceComplete?.Invoke();

        if (mainUiManager != null)
            mainUiManager.enabled = true;
    }

    private void PlaySound(AudioSource source, AudioClip clip)
    {
        if (clip == null)
            return;
        source.clip = clip;
        source.Play();
    }

    private IEnumerator FadeBlackRoutine(float fromAlpha, float toAlpha, float duration, AnimationCurve curve)
    {
        if (blackScreen == null)
            yield break;

        Color blackColor = blackScreen.color;
        blackColor.a = fromAlpha;
        blackScreen.color = blackColor;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = curve.Evaluate(Mathf.Clamp01(elapsedTime / duration));
            blackColor.a = Mathf.LerpUnclamped(fromAlpha, toAlpha, progress);
            blackScreen.color = blackColor;
            yield return null;
        }

        blackColor.a = toAlpha;
        blackScreen.color = blackColor;
    }

    private IEnumerator AppearHeartRoutine(RectTransform heartRect)
    {
        Vector3 startVector = Vector3.one * startScale;
        Vector3 endVector = Vector3.one * endScale;

        float elapsedTime = 0f;
        while (elapsedTime < appearDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = appearCurve.Evaluate(Mathf.Clamp01(elapsedTime / appearDuration));
            heartRect.localScale = Vector3.LerpUnclamped(startVector, endVector, progress);
            yield return null;
        }

        heartRect.localScale = endVector;
    }

    private IEnumerator ShakeHeartRoutine(RectTransform heartRect, float duration)
    {
        if (duration <= 0f)
            yield break;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float offsetX = Mathf.PerlinNoise(Time.unscaledTime * shakeFrequency, 0f) - 0.5f;
            float offsetY = Mathf.PerlinNoise(0f, Time.unscaledTime * shakeFrequency) - 0.5f;

            heartRect.anchoredPosition = _heartBasePosition + new Vector2(offsetX, offsetY) * 2f * shakeStrength;
            yield return null;
        }

        heartRect.anchoredPosition = _heartBasePosition;
    }

    private IEnumerator ExplosionRoutine()
    {
        if (explosionAnimator != null)
        {
            explosionAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            explosionAnimator.speed = 1f;
            explosionAnimator.Play(explosionStateName, 0, 0f);
            explosionAnimator.Update(0f);
        }

        yield return new WaitForSecondsRealtime(explosionDuration);
    }

    private IEnumerator SlideInRoutine(RectTransform panelRect)
    {
        Vector2 startPosition = _panelTargetPosition + new Vector2(0f, slideOffsetY);
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = slideCurve.Evaluate(Mathf.Clamp01(elapsedTime / slideDuration));
            panelRect.anchoredPosition = Vector2.LerpUnclamped(startPosition, _panelTargetPosition, progress);
            yield return null;
        }

        panelRect.anchoredPosition = _panelTargetPosition;
    }
}