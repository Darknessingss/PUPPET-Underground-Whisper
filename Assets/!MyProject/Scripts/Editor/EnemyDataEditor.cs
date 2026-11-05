using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyData))]
public class EnemyDataEditor : Editor
{
    private SerializedProperty _enemyID;
    private SerializedProperty _displayName;
    private SerializedProperty _enemyPrefab;
    private SerializedProperty _category;
    private SerializedProperty _maxHealth;
    private SerializedProperty _soulReward;
    private SerializedProperty _moveSpeed;
    private SerializedProperty _damage;
    private SerializedProperty _attackSpeed;
    private SerializedProperty _meleeAttackPrefab;
    private SerializedProperty _bulletPrefab;
    private SerializedProperty _muzzleFlashPrefab;
    private SerializedProperty _keepDistance;
    private SerializedProperty _detectionRadius;
    private SerializedProperty _hitSound;
    private SerializedProperty _deathSound;
    private SerializedProperty _detectionSound;
    private SerializedProperty _attackSound;
    private SerializedProperty _volume;
    private SerializedProperty _audioMixerGroup;

    private void OnEnable()
    {
        _enemyID = serializedObject.FindProperty("enemyID");
        _displayName = serializedObject.FindProperty("displayName");
        _enemyPrefab = serializedObject.FindProperty("enemyPrefab");
        _category = serializedObject.FindProperty("category");
        _maxHealth = serializedObject.FindProperty("maxHealth");
        _soulReward = serializedObject.FindProperty("soulReward");
        _moveSpeed = serializedObject.FindProperty("moveSpeed");
        _damage = serializedObject.FindProperty("damage");
        _attackSpeed = serializedObject.FindProperty("attackSpeed");
        _meleeAttackPrefab = serializedObject.FindProperty("meleeAttackPrefab");
        _bulletPrefab = serializedObject.FindProperty("bulletPrefab");
        _muzzleFlashPrefab = serializedObject.FindProperty("muzzleFlashPrefab");
        _keepDistance = serializedObject.FindProperty("keepDistance");
        _detectionRadius = serializedObject.FindProperty("detectionRadius");
        _hitSound = serializedObject.FindProperty("hitSound");
        _deathSound = serializedObject.FindProperty("deathSound");
        _detectionSound = serializedObject.FindProperty("detectionSound");
        _attackSound = serializedObject.FindProperty("attackSound");
        _volume = serializedObject.FindProperty("volume");
        _audioMixerGroup = serializedObject.FindProperty("audioMixerGroup");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Basic", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_enemyID);
        EditorGUILayout.PropertyField(_displayName);
        EditorGUILayout.PropertyField(_enemyPrefab);
        EditorGUILayout.PropertyField(_category);

        bool isMelee = _category.enumValueIndex == (int)EnemyCategory.Melee;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Health", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_maxHealth);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Rewards", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_soulReward, new GUIContent("Soul Reward"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_moveSpeed);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Combat", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_damage);
        EditorGUILayout.PropertyField(_attackSpeed);

        if (isMelee)
        {
            EditorGUILayout.PropertyField(_meleeAttackPrefab, new GUIContent("Melee Attack (Slash)"));
        }
        else
        {
            EditorGUILayout.LabelField("Range Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_bulletPrefab, new GUIContent("Bullet Prefab"));
            EditorGUILayout.PropertyField(_muzzleFlashPrefab, new GUIContent("Muzzle Flash Prefab"));
            EditorGUILayout.PropertyField(_keepDistance, new GUIContent("Keep Distance"));
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Detection", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_detectionRadius, new GUIContent("Detection Radius"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_hitSound, new GUIContent("Hit Sound"));
        EditorGUILayout.PropertyField(_deathSound, new GUIContent("Death Sound"));
        EditorGUILayout.PropertyField(_detectionSound, new GUIContent("Detection Sound"));
        EditorGUILayout.PropertyField(_attackSound, new GUIContent("Attack Sound"));
        EditorGUILayout.PropertyField(_volume);
        EditorGUILayout.PropertyField(_audioMixerGroup);

        serializedObject.ApplyModifiedProperties();
    }
}