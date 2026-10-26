using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private WeaponItem item;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private WeaponInventory inventory;

    private bool playerInRange;

    private void Start()
    {
        if (inventory != null) 
            return;
        var player = GameObject.FindGameObjectWithTag(playerTag);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) playerInRange = false;
    }

    private void Update()
    {
        if (!playerInRange) 
            return;

        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.eKey.wasPressedThisFrame)
        {
            if (inventory != null && inventory.TryAddWeapon(item))
                Destroy(gameObject);
        }
    }
}