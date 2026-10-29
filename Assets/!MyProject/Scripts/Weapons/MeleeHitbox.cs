using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.15f;
    [SerializeField] private LayerMask hitLayers;

    private float damage;
    private float remainingLifetime;
    private readonly HashSet<Collider2D> alreadyHitColliders = new HashSet<Collider2D>();

    private void OnEnable()
    {
        remainingLifetime = lifetime;
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

        var damageableTarget = other.GetComponentInParent<IDamageable>();
        if (damageableTarget != null)
            damageableTarget.TakeDamage(damage);
    }

    private bool IsInHitLayer(int layer)
    {
        return hitLayers.value == 0 || (hitLayers.value & (1 << layer)) != 0;
    }

    private void ReturnToPool()
    {
        if (BulletPool.Instance != null)
            BulletPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }
}