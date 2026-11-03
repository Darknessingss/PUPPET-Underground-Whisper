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
        Instance = this;

        menuSource = CreateSource("MenuMusic", menuClip);
        gameplaySource = CreateSource("GameplayMusic", gameplayClip);

        menuSource.volume = musicVolume;
        gameplaySource.volume = 0f;

        menuSource.Play();
        gameplaySource.Play();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetToMenu();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void ResetToMenu()
    {
        StopFade();

        menuSource.Stop();
        gameplaySource.Stop();

        menuSource.time = 0f;
        gameplaySource.time = 0f;

        menuSource.volume = musicVolume;
        gameplaySource.volume = 0f;

        menuSource.Play();
        gameplaySource.Play();
    }

    private AudioSource CreateSource(string name, AudioClip clip)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform);

        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = true;
        src.playOnAwake = false;
        src.outputAudioMixerGroup = musicMixerGroup;
        return src;
    }

    public void PlayGameplayMusic()
    {
        Crossfade(menuSource, gameplaySource);
    }

    public void PlayMenuMusic()
    {
        Crossfade(gameplaySource, menuSource);
    }

    private void Crossfade(AudioSource from, AudioSource to)
    {
        StopFade();
        fadeRoutine = StartCoroutine(CrossfadeRoutine(from, to));
    }

    private void StopFade()
    {
        if (fadeRoutine == null)
            return;

        StopCoroutine(fadeRoutine);
        fadeRoutine = null;
    }

    private IEnumerator CrossfadeRoutine(AudioSource from, AudioSource to)
    {
        float startFromVolume = from.volume;
        float startToVolume = to.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fadeDuration);

            from.volume = Mathf.Lerp(startFromVolume, 0f, progress);
            to.volume = Mathf.Lerp(startToVolume, musicVolume, progress);
            yield return null;
        }

        from.volume = 0f;
        to.volume = musicVolume;
        fadeRoutine = null;
    }
}