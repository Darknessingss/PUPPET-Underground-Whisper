using UnityEngine;
using UnityEngine.Audio;

public class EnemyAudioService : MonoBehaviour
{
    public static EnemyAudioService Instance { get; private set; }

    [SerializeField] private int poolSize = 8;

    private AudioSource[] _sources;
    private int _nextIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        transform.SetParent(null, worldPositionStays: true);
        DontDestroyOnLoad(gameObject);

        _sources = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"EnemyAudio_{i}");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            _sources[i] = src;
        }
    }

    public void Play(AudioClip clip, AudioMixerGroup group, float volume)
    {
        if (clip == null) 
            return;

        var src = _sources[_nextIndex];
        _nextIndex = (_nextIndex + 1) % _sources.Length;

        src.Stop();
        src.clip = clip;
        src.outputAudioMixerGroup = group;
        src.volume = Mathf.Clamp01(volume);
        src.pitch = 1f;
        src.Play();
    }

    public void PlayAttack(EnemyData data) => Play(data.AttackSound, data.AudioMixerGroup, data.Volume);
    public void PlayDetection(EnemyData data) => Play(data.DetectionSound, data.AudioMixerGroup, data.Volume);
    public void PlayDeath(EnemyData data) => Play(data.DeathSound, data.AudioMixerGroup, data.Volume);
    public void PlayHit(EnemyData data) => Play(data.HitSound, data.AudioMixerGroup, data.Volume);
}