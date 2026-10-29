using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private LayerMask hitLayers;

    private float damage;
    private float remainingLifetime;

    private void OnEnable()
    {
        remainingLifetime = lifetime;
    }

    public void Launch(Vector2 direction, float damageAmount)
    {
        damage = damageAmount;

        var rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
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

        var damageableTarget = other.GetComponentInParent<IDamageable>();
        if (damageableTarget != null)
            damageableTarget.TakeDamage(damage);

        ReturnToPool();
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