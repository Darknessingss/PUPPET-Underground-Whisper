using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask interactiveMask;

    private float _damage;
    private float _speed;
    private float _spawnTime;
    private Vector2 _direction;
    private IObjectPool<Bullet> _pool;
    private int _combinedMask;

    private void Awake()
    {
        _combinedMask = enemyMask.value | interactiveMask.value;
    }

    public void SetPool(IObjectPool<Bullet> pool) => _pool = pool;

    public void Init(Vector2 position, Vector2 direction, float speed, float damage)
    {
        _damage = damage;
        _speed = speed;
        _direction = direction.normalized;
        _spawnTime = Time.time;

        transform.position = position;
        transform.right = _direction;
    }

    private void Update()
    {
        transform.position += (Vector3)(_direction * _speed * Time.deltaTime);

        if (Time.time - _spawnTime >= lifetime)
            ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_combinedMask & (1 << collision.gameObject.layer)) == 0) return;

        if (collision.TryGetComponent<IDamageable>(out var target))
            target.TakeDamage(_damage);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_pool != null) _pool.Release(this);
        else gameObject.SetActive(false);
    }
}