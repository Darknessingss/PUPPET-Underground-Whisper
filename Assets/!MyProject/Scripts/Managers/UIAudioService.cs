using UnityEngine;
using UnityEngine.Audio;

public class UIAudioService : MonoBehaviour
{
    public static UIAudioService Instance { get; private set; }

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioMixerGroup mixerGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        source.playOnAwake = false;
        source.spatialBlend = 0f;
        if (mixerGroup != null)
            source.outputAudioMixerGroup = mixerGroup;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null || source == null) 
            return;
        source.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}