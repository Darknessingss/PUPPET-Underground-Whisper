using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSounds : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(PlayClickSound);
    }

    private void OnDestroy()
    {
            button.onClick.RemoveListener(PlayClickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIAudioService.Instance?.Play(hoverSound, volume);
    }

    private void PlayClickSound()
    {
        UIAudioService.Instance?.Play(clickSound, volume);
    }
}