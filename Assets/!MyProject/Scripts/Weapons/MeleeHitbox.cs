using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.15f;
    [SerializeField] private LayerMask hitLayers;

    private float damage;
    private float timer;
    private readonly HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>();

    private void OnEnable()
    {
        timer = lifetime;
        alreadyHit.Clear();
    }

    public void Init(float damageAmount)
    {
        damage = damageAmount;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hitLayers.value != 0 && ((1 << other.gameObject.layer) & hitLayers.value) == 0)
            return;

        if (!alreadyHit.Add(other))
            return;

        var dmgTarget = other.GetComponentInParent<IDamageable>();
        if (dmgTarget != null)
            dmgTarget.TakeDamage(damage);
    }

    private void ReturnToPool()
    {
        if (BulletPool.Instance != null)
            BulletPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }
}