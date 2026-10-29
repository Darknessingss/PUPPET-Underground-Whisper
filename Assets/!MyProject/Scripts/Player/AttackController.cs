using UnityEngine;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{
    [SerializeField] private WeaponInventory inventory;
    [SerializeField] private Collider2D attackOriginCollider;

    private const int WeaponSlotCount = 5;

    private int[] magazineAmmo = new int[WeaponSlotCount];
    private int[] reserveAmmo = new int[WeaponSlotCount];
    private float nextAttackTime;
    private bool isReloading;
    private float reloadEndTime;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        for (int i = 0; i < WeaponSlotCount; i++)
        {
            magazineAmmo[i] = -1;
            reserveAmmo[i] = -1;
        }
    }

    private void Update()
    {
        var weapon = inventory?.CurrentWeapon;
        if (weapon == null)
            return;

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null)
            return;

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

            if (weapon.IsRange)
                AttackRange(weapon, aimDirection);
            else
                AttackMelee(weapon, aimDirection);
        }
    }

    private Vector2 GetAimDirection()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
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
        float radius;

        if (attackOriginCollider is CircleCollider2D circleCollider)
            radius = circleCollider.radius * attackOriginCollider.transform.lossyScale.x;
        else
            radius = attackOriginCollider.bounds.extents.x;

        return center + (Vector3)(aimDirection * radius);
    }

    private void AttackMelee(WeaponItem weapon, Vector2 aimDirection)
    {
        if (weapon.SlashPrefab == null || BulletPool.Instance == null)
            return;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion slashRotation = Quaternion.Euler(0, 0, angle);

        var slashInstance = BulletPool.Instance.Get(weapon.SlashPrefab, GetSpawnPosition(aimDirection), slashRotation);
        if (slashInstance.TryGetComponent<MeleeHitbox>(out var hitbox))
            hitbox.Init(weapon.Damage);
    }

    private void AttackRange(WeaponItem weapon, Vector2 aimDirection)
    {
        if (weapon.BulletPrefab == null || BulletPool.Instance == null)
            return;

        int weaponIndex = inventory.CurrentIndex;
        if (weapon.UsesAmmo)
        {
            EnsureAmmoInit(weaponIndex, weapon);
            if (magazineAmmo[weaponIndex] <= 0)
            {
                TryStartReload(weapon);
                return;
            }
            magazineAmmo[weaponIndex]--;
        }

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);

        var projectileInstance = BulletPool.Instance.Get(weapon.BulletPrefab, GetSpawnPosition(aimDirection), bulletRotation);
        if (projectileInstance.TryGetComponent<Bullet>(out var bullet))
            bullet.Launch(aimDirection, weapon.Damage);
    }

    private void EnsureAmmoInit(int weaponIndex, WeaponItem weapon)
    {
        if (magazineAmmo[weaponIndex] < 0)
        {
            magazineAmmo[weaponIndex] = weapon.MagazineSize;
            reserveAmmo[weaponIndex] = weapon.MaxReserveAmmo;
        }
    }

    private void TryStartReload(WeaponItem weapon)
    {
        if (!weapon.UsesAmmo || !weapon.IsRange || isReloading)
            return;

        int weaponIndex = inventory.CurrentIndex;
        EnsureAmmoInit(weaponIndex, weapon);

        if (magazineAmmo[weaponIndex] >= weapon.MagazineSize || reserveAmmo[weaponIndex] <= 0)
            return;

        isReloading = true;
        reloadEndTime = Time.time + weapon.ReloadTime;
    }

    public void EnsureAmmoInitForCurrent()
    {
        var weapon = inventory?.CurrentWeapon;
        if (weapon != null && weapon.UsesAmmo)
            EnsureAmmoInit(inventory.CurrentIndex, weapon);
    }

    private void FinishReload(WeaponItem weapon)
    {
        int weaponIndex = inventory.CurrentIndex;
        int amountToReload = Mathf.Min(weapon.MagazineSize - magazineAmmo[weaponIndex], reserveAmmo[weaponIndex]);
        magazineAmmo[weaponIndex] += amountToReload;
        reserveAmmo[weaponIndex] -= amountToReload;
    }

    public int GetMagazine(int slot) => magazineAmmo[slot];
    public int GetReserve(int slot) => reserveAmmo[slot];
    public bool IsReloading => isReloading;
}