using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Transform player;
    [SerializeField] private EnemyCombat combat;
    public EnemyData Data => enemyData;

    private float _sqrDetectionRadius;
    private bool _playerDetected;
    private Vector2 _lastLookDirection = Vector2.right;

    private void Awake()
    {
        if (enemyData != null)
            _sqrDetectionRadius = enemyData.DetectionRadius * enemyData.DetectionRadius;

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
            transform.position += (Vector3)(direction * (enemyData.MoveSpeed * Time.deltaTime));
            _lastLookDirection = direction;
        }
        else
        {
            MoveRange(direction, sqrDistance);
        }

        combat?.ProcessAttack(direction, _lastLookDirection);
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