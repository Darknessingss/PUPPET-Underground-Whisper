using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private LayerMask hitLayers;

    private Rigidbody2D rb;
    private float damage;
    private float remainingLifetime;
    private bool returnedToPool;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        remainingLifetime = lifetime;
        returnedToPool = false;
    }

    public void Launch(Vector2 direction, float damageAmount)
    {
        damage = damageAmount;
        rb.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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
        if (returnedToPool) 
            return;
        if (!IsInHitLayer(other.gameObject.layer)) 
            return;

        if (other.TryGetComponent<IDamageable>(out var target))
            target.TakeDamage(damage);
        else
        {
            target = other.GetComponentInParent<IDamageable>();
            if (target != null)
                target.TakeDamage(damage);
        }

        ReturnToPool();
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