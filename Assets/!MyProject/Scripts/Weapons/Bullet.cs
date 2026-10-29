using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private LayerMask hitLayers;

    private float damage;
    private float timer;

    private void OnEnable()
    {
        timer = lifetime;
    }

    public void Launch(Vector2 direction, float damageAmount)
    {
        damage = damageAmount;

        var rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
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

        var dmgTarget = other.GetComponentInParent<IDamageable>();
        if (dmgTarget != null)
            dmgTarget.TakeDamage(damage);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (BulletPool.Instance != null)
            BulletPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }
}