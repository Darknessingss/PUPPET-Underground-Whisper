using UnityEngine;
using UnityEngine.InputSystem;

public class PickupUI : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private GameObject pickUp;

    private bool playerInRange;

    private void Start()
    {
        pickUp.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;
        playerInRange = true;
        pickUp.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;
        playerInRange = false;
        pickUp.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange)
            return;
    }
}
