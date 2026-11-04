using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Аудио клипы")]
    [SerializeField] private AudioClip menuClip;
    [SerializeField] private AudioClip gameplayClip;

    [Header("Микшер")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;

    [Header("Настройки")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.6f;
    [SerializeField] private float fadeDuration = 1f;

    private AudioSource menuSource;
    private AudioSource gameplaySource;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        menuSource = CreateSource("MenuMusic", menuClip);
        gameplaySource = CreateSource("GameplayMusic", gameplayClip);

        ResetToMenuMusic();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ResetToMenuMusic();

    private void ResetToMenuMusic()
    {
        StopFade();

        gameplaySource.Stop();
        gameplaySource.volume = 0f;

        menuSource.Stop();
        menuSource.volume = musicVolume;
        menuSource.Play();
    }

    private AudioSource CreateSource(string sourceName, AudioClip clip)
    {
        var sourceObject = new GameObject(sourceName);
        sourceObject.transform.SetParent(transform);

        var audioSource = sourceObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;

        if (musicMixerGroup != null)
            audioSource.outputAudioMixerGroup = musicMixerGroup;

        return audioSource;
    }

    public void PlayGameplayMusic() => Crossfade(menuSource, gameplaySource);
    public void PlayMenuMusic() => Crossfade(gameplaySource, menuSource);

    public void StopAllMusic()
    {
        StopFade();
        fadeRoutine = StartCoroutine(FadeOutAllRoutine());
    }

    private void Crossfade(AudioSource fromSource, AudioSource toSource)
    {
        StopFade();
        fadeRoutine = StartCoroutine(CrossfadeRoutine(fromSource, toSource));
    }

    private void StopFade()
    {
        if (fadeRoutine == null) 
            return;
        StopCoroutine(fadeRoutine);
        fadeRoutine = null;
    }

    private IEnumerator FadeOutAllRoutine()
    {
        float startMenuVolume = menuSource.volume;
        float startGameplayVolume = gameplaySource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fadeDuration);

            menuSource.volume = Mathf.Lerp(startMenuVolume, 0f, progress);
            gameplaySource.volume = Mathf.Lerp(startGameplayVolume, 0f, progress);
            yield return null;
        }

        menuSource.Stop();
        gameplaySource.Stop();
        menuSource.volume = 0f;
        gameplaySource.volume = 0f;
        fadeRoutine = null;
    }

    private IEnumerator CrossfadeRoutine(AudioSource fromSource, AudioSource toSource)
    {
        if (!toSource.isPlaying)
            toSource.Play();

        float startFromVolume = fromSource.volume;
        float startToVolume = toSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fadeDuration);

            fromSource.volume = Mathf.Lerp(startFromVolume, 0f, progress);
            toSource.volume = Mathf.Lerp(startToVolume, musicVolume, progress);
            yield return null;
        }

        fromSource.volume = 0f;
        fromSource.Stop();
        toSource.volume = musicVolume;
        fadeRoutine = null;
    }
}