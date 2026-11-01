using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyDamageDealer : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 0.3f;
    [SerializeField] private LayerMask hitLayers;

    private bool _hasHit;
    private float _remainingLifetime;
    private bool _returnedToPool;
    private readonly HashSet<Collider2D> _alreadyHitColliders = new();

    private void OnEnable()
    {
        _remainingLifetime = lifetime;
        _returnedToPool = false;
        _hasHit = false;
        _alreadyHitColliders.Clear();
    }

    public void SetDamage(float value) => damage = value;

    private void Update()
    {
        _remainingLifetime -= Time.deltaTime;
        if (_remainingLifetime <= 0f)
            ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit || !IsInHitLayer(other.gameObject.layer) || !_alreadyHitColliders.Add(other))
            return;

        if (other.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(damage);
            _hasHit = true;
        }
        else if (other.GetComponentInParent<IDamageable>() is { } parentTarget)
        {
            parentTarget.TakeDamage(damage);
            _hasHit = true;
        }
    }

    private bool IsInHitLayer(int layer) => hitLayers.value == 0 || (hitLayers.value & (1 << layer)) != 0;

    private void ReturnToPool()
    {
        if (_returnedToPool)
            return;
        _returnedToPool = true;

        if (BulletPool.Instance != null)
            BulletPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }
}