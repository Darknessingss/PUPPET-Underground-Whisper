using System.Collections;
using UnityEngine;

public class BossTentacleMechanic : MonoBehaviour
{
    [SerializeField] private BossData bossData;
    [SerializeField] private Transform player;
    [SerializeField] private EnemyController enemyController;

    private float _cooldownTimer;
    private float _sqrDetectionRadius;
    private WaitForSeconds _warningWait;
    private bool _playerInRange;

    private void Awake()
    {
        if (bossData == null && TryGetComponent<BossHealth>(out BossHealth bossHealth))
            bossData = bossHealth.BossData;

        if (enemyController == null) enemyController = GetComponent<EnemyController>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.transform;

        if (bossData != null)
        {
            _warningWait = new WaitForSeconds(bossData.TentacleWarningDelay);
            float radius = bossData.DetectionRadius;
            if (radius <= 0f) radius = 5f;
            _sqrDetectionRadius = radius * radius;
        }
    }

    private void Update()
    {
        if (bossData == null || player == null) return;

        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        bool inRange = sqrDistance <= _sqrDetectionRadius;

        if (!inRange)
        {
            _playerInRange = false;
            return;
        }

        if (!_playerInRange)
        {
            _playerInRange = true;
            _cooldownTimer = 0f;
        }

        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer <= 0f)
        {
            _cooldownTimer = bossData.TentacleCooldown;
            StartCoroutine(SpawnTentacles());
        }
    }

    private IEnumerator SpawnTentacles()
    {
        for (int index = 0; index < bossData.TentacleCount; index++)
        {
            if (player == null) yield break;

            float sqrDistance = (player.position - transform.position).sqrMagnitude;
            if (sqrDistance > _sqrDetectionRadius) yield break;

            Vector2 randomOffset = Random.insideUnitCircle * bossData.TentacleWarningRadius;
            Vector3 spawnPosition = player.position + (Vector3)randomOffset;

            if (bossData.TentacleWarningPrefab != null)
            {
                if (BulletPool.Instance != null)
                    BulletPool.Instance.Get(bossData.TentacleWarningPrefab, spawnPosition, Quaternion.identity);
                else
                    Instantiate(bossData.TentacleWarningPrefab, spawnPosition, Quaternion.identity);
            }

            StartCoroutine(SpawnSpikeAfterDelay(spawnPosition));
        }
        yield break;
    }

    private IEnumerator SpawnSpikeAfterDelay(Vector3 position)
    {
        yield return _warningWait;

        if (player == null) yield break;

        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        if (sqrDistance > _sqrDetectionRadius) yield break;

        if (bossData.TentacleSpikePrefab != null)
        {
            GameObject spike;

            if (BulletPool.Instance != null)
                spike = BulletPool.Instance.Get(bossData.TentacleSpikePrefab, position, Quaternion.identity);
            else
                spike = Instantiate(bossData.TentacleSpikePrefab, position, Quaternion.identity);

            if (spike.TryGetComponent<EnemyDamageDealer>(out EnemyDamageDealer damageDealer))
                damageDealer.SetDamage(bossData.TentacleDamage);
        }

        PlayExtra(bossData.TentacleSound);
    }

    private void PlayExtra(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = bossData.BossExtraVolume;
        audioSource.outputAudioMixerGroup = bossData.BossExtraMixer;
        audioSource.Play();
        Destroy(audioSource, clip.length + 0.1f);
    }
}