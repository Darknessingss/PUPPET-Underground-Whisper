using UnityEngine;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{
    [SerializeField] private WeaponInventory inventory;
    [SerializeField] private Collider2D attackOriginCollider;

    private const int WeaponSlotCount = 5;

    private readonly int[] magazineAmmo = new int[WeaponSlotCount];
    private readonly int[] reserveAmmo = new int[WeaponSlotCount];
    private float nextAttackTime;
    private bool isReloading;
    private float reloadEndTime;
    private Camera mainCamera;
    private Keyboard keyboard;
    private Mouse mouse;

    private void Awake()
    {
        mainCamera = Camera.main;
        keyboard = Keyboard.current;
        mouse = Mouse.current;

        for (int i = 0; i < WeaponSlotCount; i++)
        {
            magazineAmmo[i] = -1;
            reserveAmmo[i] = -1;
        }
    }

    private void Update()
    {
        var weapon = inventory?.CurrentWeapon;
        if (weapon == null || keyboard == null || mouse == null) return;

        if (isReloading)
        {
            if (Time.time >= reloadEndTime)
            {
                FinishReload(weapon);
                isReloading = false;
            }
            return;
        }

        if (keyboard.rKey.wasPressedThisFrame)
            TryStartReload(weapon);

        if (mouse.leftButton.isPressed && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / Mathf.Max(0.01f, weapon.AttackSpeed);
            Vector2 aimDirection = GetAimDirection();

            bool attacked = weapon.IsRange
                ? AttackRange(weapon, aimDirection)
                : AttackMelee(weapon, aimDirection);

            if (attacked)
                WeaponAudioService.Instance?.PlayAttack(weapon);
        }
    }

    private Vector2 GetAimDirection()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouse.position.ReadValue());
        mouseWorldPosition.z = 0f;

        Vector3 origin = attackOriginCollider != null
            ? attackOriginCollider.bounds.center
            : transform.position;

        Vector2 direction = (Vector2)(mouseWorldPosition - origin);
        return direction.sqrMagnitude < 0.0001f ? Vector2.right : direction.normalized;
    }

    private Vector3 GetSpawnPosition(Vector2 aimDirection)
    {
        if (attackOriginCollider == null)
            return transform.position;

        Vector3 center = attackOriginCollider.bounds.center;
        float radius = attackOriginCollider is CircleCollider2D circle
            ? circle.radius * attackOriginCollider.transform.lossyScale.x
            : attackOriginCollider.bounds.extents.x;

        return center + (Vector3)(aimDirection * radius);
    }

    private bool AttackMelee(WeaponItem weapon, Vector2 aimDirection)
    {
        if (weapon.SlashPrefab == null || BulletPool.Instance == null) 
            return false;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        var slashInstance = BulletPool.Instance.Get(
            weapon.SlashPrefab,
            GetSpawnPosition(aimDirection),
            Quaternion.Euler(0, 0, angle));

        if (slashInstance == null) 
            return false;

        if (slashInstance.TryGetComponent<MeleeHitbox>(out var hitbox))
        {
            hitbox.Init(weapon.Damage);
            return true;
        }
        BulletPool.Instance.Return(slashInstance);
        return false;
    }

    private bool AttackRange(WeaponItem weapon, Vector2 aimDirection)
    {
        if (weapon.BulletPrefab == null || BulletPool.Instance == null) return false;

        int idx = inventory.CurrentIndex;
        if (weapon.UsesAmmo)
        {
            EnsureAmmoInit(idx, weapon);
            if (magazineAmmo[idx] <= 0)
            {
                TryStartReload(weapon);
                return false;
            }
        }

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        var bulletInstance = BulletPool.Instance.Get(
            weapon.BulletPrefab,
            GetSpawnPosition(aimDirection),
            Quaternion.Euler(0, 0, angle));

        if (bulletInstance == null) return false;

        if (!bulletInstance.TryGetComponent<Bullet>(out var bullet))
        {
            BulletPool.Instance.Return(bulletInstance);
            return false;
        }

        if (weapon.UsesAmmo)
            magazineAmmo[idx]--;

        bullet.Launch(aimDirection, weapon.Damage);
        return true;
    }

    private void EnsureAmmoInit(int weaponIndex, WeaponItem weapon)
    {
        if (magazineAmmo[weaponIndex] >= 0) 
            return;
        magazineAmmo[weaponIndex] = weapon.MagazineSize;
        reserveAmmo[weaponIndex] = weapon.MaxReserveAmmo;
    }

    private void TryStartReload(WeaponItem weapon)
    {
        if (isReloading || !weapon.IsRange || !weapon.UsesAmmo) 
            return;

        int idx = inventory.CurrentIndex;
        EnsureAmmoInit(idx, weapon);

        if (magazineAmmo[idx] >= weapon.MagazineSize || reserveAmmo[idx] <= 0)
            return;

        isReloading = true;
        reloadEndTime = Time.time + weapon.ReloadTime;

        WeaponAudioService.Instance?.PlayReload(weapon);
    }

    public void EnsureAmmoInitForCurrent()
    {
        var weapon = inventory?.CurrentWeapon;
        if (weapon != null && weapon.UsesAmmo)
            EnsureAmmoInit(inventory.CurrentIndex, weapon);
    }

    private void FinishReload(WeaponItem weapon)
    {
        int idx = inventory.CurrentIndex;
        int amount = Mathf.Min(weapon.MagazineSize - magazineAmmo[idx], reserveAmmo[idx]);
        magazineAmmo[idx] += amount;
        reserveAmmo[idx] -= amount;
    }

    public int GetMagazine(int slot) => magazineAmmo[slot];
    public int GetReserve(int slot) => reserveAmmo[slot];
    public bool IsReloading => isReloading;
}