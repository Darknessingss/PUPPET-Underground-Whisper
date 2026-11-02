using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class ShardPickup : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private InputActionReference interactAction;

    [Header("Звук")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioMixerGroup mixerGroup;

    private bool playerInRange;

    private void OnEnable()
    {
        interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) playerInRange = false;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!playerInRange)
            return;

        LoreManager.Instance.CollectShard();
        PlayPickupSound();
        Destroy(gameObject);
    }

    private void PlayPickupSound()
    {
        if (pickupSound == null)
            return;

        var go = new GameObject("ShardPickupSFX");
        AudioSource source = go.AddComponent<AudioSource>();

        source.outputAudioMixerGroup = mixerGroup;
        source.PlayOneShot(pickupSound);

        Destroy(go, pickupSound.length);
    }
}