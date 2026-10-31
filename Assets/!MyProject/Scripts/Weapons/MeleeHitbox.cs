using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.15f;
    [SerializeField] private LayerMask hitLayers;

    private float damage;
    private float remainingLifetime;
    private bool returnedToPool;
    private readonly HashSet<Collider2D> alreadyHitColliders = new();

    private void OnEnable()
    {
        remainingLifetime = lifetime;
        returnedToPool = false;
        alreadyHitColliders.Clear();
    }

    public void Init(float damageAmount)
    {
        damage = damageAmount;
    }

    private void Update()
    {
        remainingLifetime -= Time.deltaTime;
        if (remainingLifetime <= 0f)
            ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsInHitLayer(other.gameObject.layer)) 
            return;
        if (!alreadyHitColliders.Add(other)) 
            return;

        if (other.TryGetComponent<IDamageable>(out var target))
            target.TakeDamage(damage);
        else
        {
            target = other.GetComponentInParent<IDamageable>();
            if (target != null)
                target.TakeDamage(damage);
        }
    }

    private bool IsInHitLayer(int layer)
    {
        return hitLayers.value == 0 || (hitLayers.value & (1 << layer)) != 0;
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