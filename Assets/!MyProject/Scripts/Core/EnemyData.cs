using UnityEngine;
using UnityEngine.Audio;

public enum EnemyCategory { Melee, Range }

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic")]
    [SerializeField] private string enemyID;
    [SerializeField] private string displayName;
    [SerializeField] private GameObject enemyPrefab;

    [Header("Category")]
    [SerializeField] private EnemyCategory category;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Combat")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private GameObject meleeAttackPrefab;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip detectionSound;
    [SerializeField] private AudioClip attackSound;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;
    [SerializeField] private AudioMixerGroup audioMixerGroup;

    public string EnemyID => enemyID;
    public string DisplayName => displayName;
    public GameObject EnemyPrefab => enemyPrefab;
    public EnemyCategory Category => category;
    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public float Damage => damage;
    public float AttackSpeed => attackSpeed;
    public GameObject MeleeAttackPrefab => meleeAttackPrefab;
    public float DetectionRadius => detectionRadius;

    public AudioClip HitSound => hitSound;
    public AudioClip DeathSound => deathSound;
    public AudioClip DetectionSound => detectionSound;
    public AudioClip AttackSound => attackSound;
    public float Volume => volume;
    public AudioMixerGroup AudioMixerGroup => audioMixerGroup;

    public bool IsMelee => category == EnemyCategory.Melee;
}