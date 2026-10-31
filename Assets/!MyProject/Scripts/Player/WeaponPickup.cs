using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private WeaponItem item;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private WeaponInventory inventory;
    [SerializeField] private InputActionReference interactAction;

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
        if (other.CompareTag(playerTag))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            playerInRange = false;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange && inventory.TryAddWeapon(item))
            Destroy(gameObject);
    }
}