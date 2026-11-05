using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Collider2D attackOriginCollider;

    [Header("Range Settings")]
    [SerializeField] private float viewAngle = 90f;

    [Header("Movement Lock")]
    [SerializeField] private float attackMovementLockDuration = 0.5f;

    private float _attackCooldown;
    private float _attackRadius;
    private Vector3 _colliderCenterOffset;
    private float _attackLockEndTime;
    private float _cosHalfViewAngle;

    public bool IsMovementLocked => Time.time < _attackLockEndTime;

    private void Awake()
    {
        if (enemyData == null && TryGetComponent<EnemyController>(out var controller))
            enemyData = controller.Data;

        if (attackOriginCollider == null)
            attackOriginCollider = GetComponent<Collider2D>();

        _cosHalfViewAngle = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);

        CacheColliderData();
    }

    private void CacheColliderData()
    {
        if (attackOriginCollider == null)
            return;

        _colliderCenterOffset = attackOriginCollider.bounds.center - transform.position;
        float scale = Mathf.Abs(transform.lossyScale.x);

        _attackRadius = attackOriginCollider 
            switch
        {
            CapsuleCollider2D capsule => Mathf.Max(capsule.size.x, capsule.size.y) * 0.5f * scale,
            CircleCollider2D circle => circle.radius * scale,
            BoxCollider2D box => Mathf.Max(box.size.x, box.size.y) * 0.5f * scale,
            _ => Mathf.Max(attackOriginCollider.bounds.extents.x, attackOriginCollider.bounds.extents.y)
        };
    }

    public void ProcessAttack(Vector2 directionToPlayer, Vector2 lookDirection, bool canAttack)
    {
        if (_attackCooldown > 0f)
        {
            _attackCooldown -= Time.deltaTime;
            return;
        }

        if (!canAttack)
            return;

        if (enemyData.IsRange && Vector2.Dot(lookDirection, directionToPlayer) < _cosHalfViewAngle)
            return;

        Attack(directionToPlayer);
        _attackCooldown = 1f / Mathf.Max(enemyData.AttackSpeed, 0.01f);
        _attackLockEndTime = Time.time + attackMovementLockDuration;
    }

    private void Attack(Vector2 direction)
    {
        EnemyAudioService.Instance.PlayAttack(enemyData);

        if (enemyData.IsMelee)
            AttackMelee(direction);
        else
            AttackRange(direction);
    }

    private void AttackMelee(Vector2 direction)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        Vector3 spawnPosition = transform.position + _colliderCenterOffset + (Vector3)(direction * _attackRadius);

        GameObject slashInstance = BulletPool.Instance != null
            ? BulletPool.Instance.Get(enemyData.MeleeAttackPrefab, spawnPosition, rotation)
            : Instantiate(enemyData.MeleeAttackPrefab, spawnPosition, rotation);

        if (slashInstance.TryGetComponent<EnemyDamageDealer>(out var dealer))
            dealer.SetDamage(enemyData.Damage);
    }

    private void AttackRange(Vector2 direction)
    {
        if (BulletPool.Instance == null)
        {
            Debug.LogError("BulletPool.Instance is missing on the scene!");
            return;
        }

        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        Vector3 spawnPosition = transform.position + _colliderCenterOffset + (Vector3)(direction * (_attackRadius + 0.2f));

        if (enemyData.MuzzleFlashPrefab != null)
            BulletPool.Instance.Get(enemyData.MuzzleFlashPrefab, spawnPosition, rotation);

        var bulletInstance = BulletPool.Instance.Get(enemyData.BulletPrefab, spawnPosition, rotation);
        if (bulletInstance != null && bulletInstance.TryGetComponent<Bullet>(out var bullet))
            bullet.Launch(direction, enemyData.Damage);
    }
}