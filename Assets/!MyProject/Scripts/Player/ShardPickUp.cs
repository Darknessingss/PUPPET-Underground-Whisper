using UnityEngine;
using UnityEngine.InputSystem;

public class ShardPickup : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private AudioClip pickupSound;

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
        go.AddComponent<AudioSource>().PlayOneShot(pickupSound);
        Destroy(go, pickupSound.length);
    }
}