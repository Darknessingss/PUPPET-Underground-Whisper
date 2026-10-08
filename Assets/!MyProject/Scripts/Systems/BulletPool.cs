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
        var PrefabSpawn = Instantiate(bulletPrefab, transform);
        PrefabSpawn.gameObject.SetActive(false);
        return PrefabSpawn;
    }

    public Bullet Get()
    {
        var PrefabSpawn = _pool.Count > 0 ? _pool.Dequeue() : CreateBullet();
        return PrefabSpawn;
    }

    public void Return(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        _pool.Enqueue(bullet);
    }
}