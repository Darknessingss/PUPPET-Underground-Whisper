using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int sizePool = 5;

    private readonly Queue<Bullet> _pool = new();

    private void Awake()
    {
        for (int i = 0; i < sizePool; i++)
            _pool.Enqueue(CreateBullet());
    }

    private Bullet CreateBullet()
    {
        var b = Instantiate(bulletPrefab, transform);
        b.gameObject.SetActive(false);
        return b;
    }

    public Bullet Get()
    {
        var b = _pool.Count > 0 ? _pool.Dequeue() : CreateBullet();
        return b;
    }

    public void Return(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        _pool.Enqueue(bullet);
    }
}