using System.Collections;
using UnityEngine;

public class BossStompMechanic : MonoBehaviour
{
    [SerializeField] private BossData bossData;
    [SerializeField] private Transform player;

    private bool _isCharging;
    private float _cooldownTimer;

    private void Awake()
    {
        if (bossData == null && TryGetComponent<BossHealth>(out BossHealth bossHealth))
            bossData = bossHealth.BossData;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.transform;
    }

    private void Update()
    {
        if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;
    }

    public void StartCharge()
    {
        if (_isCharging || _cooldownTimer > 0f) return;
        StartCoroutine(StompRoutine());
    }

    private IEnumerator StompRoutine()
    {
        _isCharging = true;
        _cooldownTimer = bossData.StompCooldown;

        GameObject chargeVisual = null;
        if (bossData.StompPrefab != null)
            chargeVisual = Instantiate(bossData.StompPrefab, transform.position, Quaternion.identity, transform);

        yield return new WaitForSeconds(bossData.StompChargeTime);

        if (chargeVisual != null) Destroy(chargeVisual);

        if (bossData.StompImpactPrefab != null)
        {
            GameObject impact = Instantiate(bossData.StompImpactPrefab, transform.position, Quaternion.identity);
            Destroy(impact, 1.5f);

            if (impact.TryGetComponent<EnemyDamageDealer>(out EnemyDamageDealer damageDealer))
                damageDealer.SetDamage(bossData.StompDamage);
        }

        PlayExtra(bossData.StompSound);

        if (player != null)
        {
            float sqrDistance = (player.position - transform.position).sqrMagnitude;
            if (sqrDistance <= bossData.StompRadius * bossData.StompRadius &&
                player.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(bossData.StompDamage);
            }
        }

        _isCharging = false;
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