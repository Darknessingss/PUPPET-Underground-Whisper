using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Transform player;
    [SerializeField] private Collider2D attackOriginCollider;

    [Header("Range Settings")]
    [SerializeField] private float viewAngle = 90f;

    private float _attackCooldown;
    private float _sqrDetectionRadius;
    private float _attackRadius;
    private Vector3 _colliderCenterOffset;
    private bool _playerDetected;
    private Vector2 _lastLookDirection = Vector2.right;

    private void Awake()
    {
        if (enemyData != null)
            _sqrDetectionRadius = enemyData.DetectionRadius * enemyData.DetectionRadius;

        if (attackOriginCollider == null)
            attackOriginCollider = GetComponent<Collider2D>();

        CacheColliderData();
    }

    private void Start()
    {
        if (player == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void CacheColliderData()
    {
        if (attackOriginCollider == null) return;

        _colliderCenterOffset = attackOriginCollider.bounds.center - transform.position;
        float scale = Mathf.Abs(transform.lossyScale.x);

        if (attackOriginCollider is CapsuleCollider2D capsule)
            _attackRadius = Mathf.Max(capsule.size.x, capsule.size.y) * 0.5f * scale;
        else if (attackOriginCollider is CircleCollider2D circle)
            _attackRadius = circle.radius * scale;
        else if (attackOriginCollider is BoxCollider2D box)
            _attackRadius = Mathf.Max(box.size.x, box.size.y) * 0.5f * scale;
        else
            _attackRadius = Mathf.Max(attackOriginCollider.bounds.extents.x, attackOriginCollider.bounds.extents.y);
    }

    private void Update()
    {
        if (enemyData == null || player == null)
            return;

        Vector2 toPlayer = (Vector2)(player.position - transform.position);
        float sqrDistance = toPlayer.sqrMagnitude;

        if (!_playerDetected)
        {
            if (sqrDistance > _sqrDetectionRadius) 
                return;

            _playerDetected = true;
            EnemyAudioService.Instance.PlayDetection(enemyData);
        }
        else if (sqrDistance > _sqrDetectionRadius)
        {
            _playerDetected = false;
            return;
        }

        Vector2 direction = toPlayer.normalized;

        if (enemyData.IsMelee)
        {
            MoveMelee(direction);
            _lastLookDirection = direction;
        }
        else
        {
            MoveRange(direction, sqrDistance);
        }


        _attackCooldown -= Time.deltaTime;

        if (_attackCooldown > 0f) 
            return;
        if (enemyData.IsRange && Vector2.Angle(_lastLookDirection, direction) > viewAngle * 0.5f)
            return;

        Attack(direction);
        _attackCooldown = 1f / Mathf.Max(enemyData.AttackSpeed, 0.01f);
    }

    private void MoveMelee(Vector2 direction)
    {
        transform.position += (Vector3)(direction * (enemyData.MoveSpeed * Time.deltaTime));
    }

    private void MoveRange(Vector2 direction, float sqrDistance)
    {
        float keepDistSqr = enemyData.KeepDistance * enemyData.KeepDistance;

        if (sqrDistance > keepDistSqr)
        {
            transform.position += (Vector3)(direction * (enemyData.MoveSpeed * Time.deltaTime));
            _lastLookDirection = direction;
        }
        else if (sqrDistance < keepDistSqr * 0.8f)
        {
            Vector2 backDirection = -direction;
            transform.position += (Vector3)(backDirection * (enemyData.MoveSpeed * Time.deltaTime));
            _lastLookDirection = backDirection;
        }
        else
        {
            _lastLookDirection = direction;
        }
    }

    private void Attack(Vector2 direction)
    {
        EnemyAudioService.Instance.PlayAttack(enemyData);

        if (enemyData.IsMelee && enemyData.MeleeAttackPrefab != null)
            AttackMelee(direction);
        else if (enemyData.IsRange && enemyData.BulletPrefab != null)
            AttackRange(direction);
    }

    private void AttackMelee(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Vector3 spawnPosition = transform.position + _colliderCenterOffset + (Vector3)(direction * _attackRadius);
        var slash = Instantiate(enemyData.MeleeAttackPrefab, spawnPosition, rotation);

        var dealer = slash.GetComponent<EnemyDamageDealer>();
        if (dealer != null)
            dealer.SetDamage(enemyData.Damage);
    }

    private void AttackRange(Vector2 direction)
    {
        if (BulletPool.Instance == null)
        {
            Debug.LogError("BulletPool.Instance is missing on the scene!");
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Vector3 spawnPosition = transform.position + _colliderCenterOffset + (Vector3)(direction * (_attackRadius + 0.2f));

        var bulletInstance = BulletPool.Instance.Get(enemyData.BulletPrefab, spawnPosition, rotation);
        if (bulletInstance != null && bulletInstance.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.Launch(direction, enemyData.Damage);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.DetectionRadius);

        if (enemyData.IsRange)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, enemyData.KeepDistance);
        }
    }
#endif
}