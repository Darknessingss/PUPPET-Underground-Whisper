using UnityEngine;

public enum WeaponCategory { Melee, Range }

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Inventory/Weapon Item")]
public class WeaponItem : ScriptableObject
{
    [Header("Basic")]
    [SerializeField] private string itemID;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject worldPrefab;
    [SerializeField] private GameObject slashPrefab;

    [Header("Category")]
    [SerializeField] private WeaponCategory category;

    [Header("Stats")]
    [SerializeField] private float damage;
    [SerializeField] private float attackSpeed;
    [Header("Range Only")]
    [SerializeField] private bool usesAmmo;
    [SerializeField] private int magazineSize = 5;
    [SerializeField] private int maxReserveAmmo = 40;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float reloadTime = 1.5f;

    public string ItemID => itemID;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public GameObject WorldPrefab => worldPrefab;
    public WeaponCategory Category => category;
    public float Damage => damage;
    public float AttackSpeed => attackSpeed;
    public GameObject SlashPrefab => slashPrefab;
    public bool UsesAmmo => usesAmmo;
    public int MagazineSize => magazineSize;
    public int MaxReserveAmmo => maxReserveAmmo;
    public GameObject BulletPrefab => bulletPrefab;
    public float ReloadTime => reloadTime;

    public bool IsRange => category == WeaponCategory.Range;
}