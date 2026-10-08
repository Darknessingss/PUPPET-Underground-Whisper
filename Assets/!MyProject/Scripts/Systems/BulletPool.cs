using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int defaultCapacity = 5;
    [SerializeField] private int maxSize = 50;

    private IObjectPool<Bullet> _pool;

    public IObjectPool<Bullet> Pool => _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Bullet>(
            createFunc: CreateBullet,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyBullet,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    private Bullet CreateBullet()
    {
        var bullet = Instantiate(bulletPrefab, transform);
        bullet.SetPool(_pool);
        return bullet;
    }

    private void OnGet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void OnRelease(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void OnDestroyBullet(Bullet bullet)
    {
        if (bullet != null)
        {
            Destroy(bullet.gameObject);
        }
    }

    public Bullet Get() => _pool.Get();
    public void Return(Bullet bullet) => _pool.Release(bullet);
}