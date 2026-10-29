using UnityEngine;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{
    [SerializeField] private WeaponInventory inventory;

    private int[] magazine = new int[5];
    private int[] reserve = new int[5];
    private float nextAttackTime;
    private bool isReloading;
    private float reloadEndTime;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        for (int i = 0; i < magazine.Length; i++) { magazine[i] = -1; reserve[i] = -1; }
    }

    private void Update()
    {
        var weapon = inventory != null ? inventory.CurrentWeapon : null;
        if (weapon == null) 
            return;

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) 
            return;

        Vector2 dir = GetMouseDirection();

        if (isReloading)
        {
            if (Time.time >= reloadEndTime)
            {
                FinishReload(weapon);
                isReloading = false;
            }
            return;
        }

        if (keyboard.rKey.wasPressedThisFrame && weapon.IsRange && weapon.UsesAmmo)
            TryStartReload(weapon);

        if (mouse.leftButton.isPressed && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / Mathf.Max(0.01f, weapon.AttackSpeed);

            if (weapon.IsRange) AttackRange(weapon, dir);
            else AttackMelee(weapon, dir);
        }
    }

    private Vector2 GetMouseDirection()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;
        Vector2 dir = (Vector2)(mouseWorld - transform.position);
        return dir.sqrMagnitude < 0.0001f ? Vector2.right : dir.normalized;
    }

    private void AttackMelee(WeaponItem weapon, Vector2 dir)
    {
        if (weapon.SlashPrefab == null) return;
        if (BulletPool.Instance == null)
        {
            Debug.LogError("BulletPool не найден в сцене!");
            return;
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        var slash = BulletPool.Instance.Get(weapon.SlashPrefab, transform.position, rot);
        var hb = slash.GetComponent<MeleeHitbox>();
        if (hb != null) hb.Init(weapon.Damage);
    }

    private void AttackRange(WeaponItem weapon, Vector2 dir)
    {
        if (weapon.BulletPrefab == null) 
            return;
        if (BulletPool.Instance == null)
        {
            Debug.LogError("BulletPool не найден в сцене!");
            return;
        }

        int idx = inventory.CurrentIndex;

        if (weapon.UsesAmmo)
        {
            EnsureAmmoInit(idx, weapon);
            if (magazine[idx] <= 0)
            {
                TryStartReload(weapon);
                return;
            }
            magazine[idx]--;
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        var bulletGO = BulletPool.Instance.Get(weapon.BulletPrefab, transform.position, rot);
        var bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null) bullet.Launch(dir, weapon.Damage);
    }

    private void EnsureAmmoInit(int idx, WeaponItem weapon)
    {
        if (magazine[idx] < 0)
        {
            magazine[idx] = weapon.MagazineSize;
            reserve[idx] = weapon.MaxReserveAmmo;
        }
    }

    private void TryStartReload(WeaponItem weapon)
    {
        if (!weapon.UsesAmmo || isReloading) 
            return;

        int idx = inventory.CurrentIndex;
        EnsureAmmoInit(idx, weapon);

        if (magazine[idx] >= weapon.MagazineSize) 
            return;
        if (reserve[idx] <= 0) 
            return;

        isReloading = true;
        reloadEndTime = Time.time + weapon.ReloadTime;
    }
    public void EnsureAmmoInitForCurrent()
    {
        var weapon = inventory != null ? inventory.CurrentWeapon : null;
        if (weapon == null || !weapon.UsesAmmo) 
            return;
        EnsureAmmoInit(inventory.CurrentIndex, weapon);
    }

    private void FinishReload(WeaponItem weapon)
    {
        int idx = inventory.CurrentIndex;
        int need = weapon.MagazineSize - magazine[idx];
        int take = Mathf.Min(need, reserve[idx]);
        magazine[idx] += take;
        reserve[idx] -= take;
    }

    public int GetMagazine(int slot) => magazine[slot];
    public int GetReserve(int slot) => reserve[slot];
    public bool IsReloading => isReloading;
}