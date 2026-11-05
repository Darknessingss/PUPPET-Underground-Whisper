using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Transform player;
    [SerializeField] private EnemyCombat combat;
    public EnemyData Data => enemyData;

    private float _sqrDetectionRadius;
    private float _sqrMeleeAttackRadius;
    private float _sqrKeepDistance;
    private bool _playerDetected;
    private Vector2 _lastLookDirection = Vector2.right;

    private void Awake()
    {
        if (enemyData != null)
        {
            _sqrDetectionRadius = enemyData.DetectionRadius * enemyData.DetectionRadius;
            _sqrMeleeAttackRadius = enemyData.MeleeAttackRadius * enemyData.MeleeAttackRadius;
            _sqrKeepDistance = enemyData.KeepDistance * enemyData.KeepDistance;
        }

        if (combat == null) combat = GetComponent<EnemyCombat>();
    }

    private void Start()
    {
        if (player == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (enemyData == null || player == null)
            return;

        Vector2 toPlayer = player.position - transform.position;
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
        _lastLookDirection = direction;

        if (combat.IsMovementLocked)
        {
            combat.ProcessAttack(direction, _lastLookDirection, true);
            return;
        }

        if (enemyData.IsMelee)
        {
            bool inAttackRange = sqrDistance <= _sqrMeleeAttackRadius;
            if (!inAttackRange)
                transform.position += (Vector3)(direction * (enemyData.MoveSpeed * Time.deltaTime));

            combat.ProcessAttack(direction, _lastLookDirection, inAttackRange);
            return;
        }

        MoveRange(direction, sqrDistance);
        combat.ProcessAttack(direction, _lastLookDirection, true);
    }

    private void MoveRange(Vector2 direction, float sqrDistance)
    {
        if (sqrDistance > _sqrKeepDistance)
            transform.position += (Vector3)(direction * (enemyData.MoveSpeed * Time.deltaTime));
        else if (sqrDistance < _sqrKeepDistance * 0.8f)
            transform.position += (Vector3)(-direction * (enemyData.MoveSpeed * Time.deltaTime));
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.DetectionRadius);

        if (enemyData.IsMelee)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyData.MeleeAttackRadius);
        }

        if (enemyData.IsRange)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, enemyData.KeepDistance);
        }
    }
#endif
}