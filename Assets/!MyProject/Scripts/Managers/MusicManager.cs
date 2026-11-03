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

    private AudioSource CreateSource(string name, AudioClip clip)
    {
        var gameObject = new GameObject(name);
        gameObject.transform.SetParent(transform);

        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
        audioSource.outputAudioMixerGroup = musicMixerGroup;
        return audioSource;
    }

    public void PlayGameplayMusic() => Crossfade(menuSource, gameplaySource);

    public void PlayMenuMusic() => Crossfade(gameplaySource, menuSource);

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

    private IEnumerator CrossfadeRoutine(AudioSource fromSource, AudioSource toSource)
    {
        if (!toSource.isPlaying) toSource.Play();

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