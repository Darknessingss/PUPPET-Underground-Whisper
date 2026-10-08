using Unity.Cinemachine;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask interactiveMask;


    private float _damage;
    private float _speed;
    private float _spawnTime;
    private Vector2 _direction;
    private BulletPool _pool;
    private int _combinedMask;

    private void Awake()
    {
        _combinedMask = enemyMask.value | interactiveMask.value;
    }

    public void Init(BulletPool pool, Vector2 position, Vector2 direction,
                     float speed, float damage)
    {
        _pool = pool;
        _damage = damage;
        _speed = speed;
        _direction = direction.normalized;
        _spawnTime = Time.time;

        transform.position = position;
        transform.right = _direction;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += (Vector3)(_direction * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_combinedMask & (1 << collision.gameObject.layer)) == 0) return;
        {
            if (collision.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(_damage);

            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (_pool != null) _pool.Return(this);
        else gameObject.SetActive(false);
    }
}
