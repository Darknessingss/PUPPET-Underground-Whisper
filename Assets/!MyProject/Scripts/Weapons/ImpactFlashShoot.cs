using System.Collections;
using UnityEngine;

public class ImpactFlashShoot : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.05f;

    private Coroutine lifetimeRoutine;
    private bool returnedToPool;

    private void OnEnable()
    {
        returnedToPool = false;
        RestartLifetime(lifetime);
    }

    private void OnDisable()
    {
        if (lifetimeRoutine != null)
        {
            StopCoroutine(lifetimeRoutine);
            lifetimeRoutine = null;
        }
    }

    public void Init(float overrideLifetime)
    {
        if (overrideLifetime > 0f)
            RestartLifetime(overrideLifetime);
    }

    private void RestartLifetime(float duration)
    {
        if (lifetimeRoutine != null)
            StopCoroutine(lifetimeRoutine);

        lifetimeRoutine = StartCoroutine(LifetimeRoutine(duration));
    }

    private IEnumerator LifetimeRoutine(float duration)
    {
        if (duration > 0f)
            yield return new WaitForSeconds(duration);

        lifetimeRoutine = null;
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (returnedToPool)
            return;
        returnedToPool = true;

        if (BulletPool.Instance != null)
            BulletPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }
}