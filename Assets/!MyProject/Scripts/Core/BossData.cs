using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewBoss", menuName = "Enemies/Boss Data")]
public class BossData : EnemyData
{
    [Header("Boss - Tentacle Mechanic")]
    [SerializeField] private GameObject tentacleWarningPrefab;
    [SerializeField] private GameObject tentacleSpikePrefab;
    [SerializeField] private float tentacleDamage = 15f;
    [SerializeField] private float tentacleWarningRadius = 6f;
    [SerializeField] private float tentacleWarningDelay = 0.8f;
    [SerializeField] private int tentacleCount = 2;
    [SerializeField] private float tentacleCooldown = 4f;

    [Header("Boss - Stomp Mechanic")]
    [SerializeField] private GameObject stompPrefab;
    [SerializeField] private GameObject stompImpactPrefab;
    [SerializeField] private float stompDamage = 25f;
    [SerializeField] private float stompChargeTime = 2f;
    [SerializeField] private float stompRadius = 8f;
    [SerializeField] private float stompCooldown = 8f;

    [Header("Boss - Enrage Mechanic")]
    [SerializeField] private float enrageDuration = 10f;
    [SerializeField] private float enrageSpeedMultiplier = 2.5f;
    [SerializeField] private float enragePassDamage = 5f;
    [SerializeField] private float enrageMaxTotalDamage = 20f;
    [SerializeField] private float enrageCircleRadius = 3.5f;
    [SerializeField] private float enragePassCooldown = 0.6f;
    [SerializeField] private GameObject enrageTrailPrefab;

    [Header("Boss - Extra Audio")]
    [SerializeField] private AudioClip stompSound;
    [SerializeField] private AudioClip tentacleSound;
    [SerializeField] private AudioClip enrageRoarSound;
    [Range(0f, 1f)][SerializeField] private float bossExtraVolume = 1f;
    [SerializeField] private AudioMixerGroup bossExtraMixer;

    public GameObject TentacleWarningPrefab => tentacleWarningPrefab;
    public GameObject TentacleSpikePrefab => tentacleSpikePrefab;
    public float TentacleDamage => tentacleDamage;
    public float TentacleWarningRadius => tentacleWarningRadius;
    public float TentacleWarningDelay => tentacleWarningDelay;
    public int TentacleCount => tentacleCount;
    public float TentacleCooldown => tentacleCooldown;

    public GameObject StompPrefab => stompPrefab;
    public GameObject StompImpactPrefab => stompImpactPrefab;
    public float StompDamage => stompDamage;
    public float StompChargeTime => stompChargeTime;
    public float StompRadius => stompRadius;
    public float StompCooldown => stompCooldown;

    public float EnrageDuration => enrageDuration;
    public float EnrageSpeedMultiplier => enrageSpeedMultiplier;
    public float EnragePassDamage => enragePassDamage;
    public float EnrageMaxTotalDamage => enrageMaxTotalDamage;
    public float EnrageCircleRadius => enrageCircleRadius;
    public float EnragePassCooldown => enragePassCooldown;
    public GameObject EnrageTrailPrefab => enrageTrailPrefab;

    public AudioClip StompSound => stompSound;
    public AudioClip TentacleSound => tentacleSound;
    public AudioClip EnrageRoarSound => enrageRoarSound;
    public float BossExtraVolume => bossExtraVolume;
    public AudioMixerGroup BossExtraMixer => bossExtraMixer;
}