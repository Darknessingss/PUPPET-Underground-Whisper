using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Transform player;
    [SerializeField] private Collider2D attackOriginCollider;

    private float _attackCooldown;
    private float _sqrDetectionRadius;
    private float _attackRadius;
    private Vector3 _colliderCenterOffset;
    private bool _playerDetected;

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

        if (!_playerDetected && sqrDistance <= _sqrDetectionRadius)
        {
            _playerDetected = true;
            EnemyAudioService.Instance.PlayDetection(enemyData);
        }
        else if (_playerDetected && sqrDistance > _sqrDetectionRadius)
        {
            _playerDetected = false;
        }

        if (!_playerDetected)
            return;

        Vector2 direction = toPlayer.normalized;
        transform.position += (Vector3)(direction * (enemyData.MoveSpeed * Time.deltaTime));

        _attackCooldown -= Time.deltaTime;

        if (_attackCooldown <= 0f)
        {
            Attack(direction);
            _attackCooldown = 1f / Mathf.Max(enemyData.AttackSpeed, 0.01f);
        }
    }

    private void Attack(Vector2 direction)
    {
        EnemyAudioService.Instance.PlayAttack(enemyData);

        if (!enemyData.IsMelee || enemyData.MeleeAttackPrefab == null)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Vector3 spawnPosition = transform.position + _colliderCenterOffset + (Vector3)(direction * _attackRadius);

        var slash = Instantiate(enemyData.MeleeAttackPrefab, spawnPosition, rotation);

        var dealer = slash.GetComponent<EnemyDamageDealer>();
        if (dealer != null)
            dealer.SetDamage(enemyData.Damage);
    }

    private void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.DetectionRadius);
    }
}