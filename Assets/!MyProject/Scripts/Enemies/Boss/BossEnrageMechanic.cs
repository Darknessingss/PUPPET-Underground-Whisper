using System.Collections;
using UnityEngine;

public class BossEnrageMechanic : MonoBehaviour
{
    [SerializeField] private BossData bossData;
    [SerializeField] private Transform player;
    [SerializeField] private EnemyController controller;

    private float _totalPassDamageDealt;
    private float _passTimer;
    private float _angle;
    private GameObject _trailVisual;
    private Collider2D[] _colliders;
    private bool[] _originalTriggerStates;

    private const float OrbitSpeed = 4f;
    private const float PassRadiusSqr = 1.44f;

    private void Awake()
    {
        if (bossData == null && TryGetComponent<BossHealth>(out BossHealth bossHealth))
            bossData = bossHealth.BossData;

        if (controller == null) controller = GetComponent<EnemyController>();

        _colliders = GetComponentsInChildren<Collider2D>();
        _originalTriggerStates = new bool[_colliders.Length];
        for (int index = 0; index < _colliders.Length; index++)
            _originalTriggerStates[index] = _colliders[index].isTrigger;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.transform;
    }

    public void Activate()
    {
        PlayExtra(bossData.EnrageRoarSound);
        StartCoroutine(EnrageRoutine());
    }

    private IEnumerator EnrageRoutine()
    {
        if (bossData.EnrageTrailPrefab != null)
            _trailVisual = Instantiate(bossData.EnrageTrailPrefab, transform.position, Quaternion.identity, transform);

        SetCollidersTrigger(true);

        if (controller != null) controller.SetMovementEnabled(false);

        float currentSpeed = bossData.MoveSpeed * bossData.EnrageSpeedMultiplier;
        float totalDamageLimit = bossData.EnrageMaxTotalDamage;
        float enrageTimer = bossData.EnrageDuration;

        while (enrageTimer > 0f && _totalPassDamageDealt < totalDamageLimit && player != null)
        {
            enrageTimer -= Time.deltaTime;

            _angle += OrbitSpeed * Time.deltaTime;
            Vector3 targetPosition = player.position +
                new Vector3(Mathf.Cos(_angle), Mathf.Sin(_angle), 0f) * bossData.EnrageCircleRadius;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

            _passTimer -= Time.deltaTime;
            if (_passTimer <= 0f)
            {
                float sqrDistance = (player.position - transform.position).sqrMagnitude;
                if (sqrDistance <= PassRadiusSqr &&
                    player.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    damageable.TakeDamage(bossData.EnragePassDamage);
                    _totalPassDamageDealt += bossData.EnragePassDamage;
                    _passTimer = bossData.EnragePassCooldown;
                }
            }

            yield return null;
        }

        SetCollidersTrigger(false);

        if (controller != null) controller.SetMovementEnabled(true);
        if (_trailVisual != null) _trailVisual.SetActive(false);
    }

    private void SetCollidersTrigger(bool value)
    {
        for (int index = 0; index < _colliders.Length; index++)
            _colliders[index].isTrigger = value;
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