using UnityEngine;
using UnityEngine.Audio;

public class WeaponAudioService : MonoBehaviour
{
    public static WeaponAudioService Instance { get; private set; }

    [SerializeField] private int poolSize = 8;

    private AudioSource[] sources;
    private int nextIndex;

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

        sources = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"WeaponAudio_{i}");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            sources[i] = src;
        }
    }

    public void Play(AudioClip clip, AudioMixerGroup group, float volume)
    {
        if (clip == null) 
            return;

        var src = sources[nextIndex];
        nextIndex = (nextIndex + 1) % sources.Length;

        src.Stop();
        src.clip = clip;
        src.outputAudioMixerGroup = group;
        src.volume = Mathf.Clamp01(volume);
        src.pitch = 1f;
        src.Play();
    }

    public void PlayAttack(WeaponItem weapon)
    {
        if (weapon == null) 
            return;
        Play(weapon.AttackSound, weapon.AudioMixerGroup, weapon.Volume);
    }

    public void PlayReload(WeaponItem weapon)
    {
        if (weapon == null || !weapon.HasReloadSound) 
            return;
        Play(weapon.ReloadSound, weapon.AudioMixerGroup, weapon.Volume);
    }
}