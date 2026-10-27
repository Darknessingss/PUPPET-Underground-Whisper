using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSounds : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioMixerGroup mixerGroup;

    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    [SerializeField] private Button button;

    private void Awake()
    {
        audioSource.playOnAwake = false;
        audioSource.outputAudioMixerGroup = mixerGroup;

        button.onClick.AddListener(PlayClickSound);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(PlayClickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(hoverSound);
    }

    private void PlayClickSound()
    {
        PlaySound(clickSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null)
            return;

        audioSource.PlayOneShot(clip, volume);
    }
}