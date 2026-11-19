using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private ManagerInterface manager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            manager.ActivateBossBar();
            gameObject.SetActive(false);
        }
    }
}