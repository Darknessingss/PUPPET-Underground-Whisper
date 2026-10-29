using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private WeaponInventory inventory;
    [SerializeField] private AttackController controller;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image weaponIconImage;

    private WeaponItem lastWeapon;

    private void Update()
    {
        var weapon = inventory.CurrentWeapon;

        if (weapon != lastWeapon)
        {
            lastWeapon = weapon;

            if (weapon == null)
            {
                weaponNameText.text = "";
                weaponIconImage.sprite = null;
                ammoText.gameObject.SetActive(false);
                return;
            }

            weaponNameText.text = weapon.DisplayName;
            weaponIconImage.sprite = weapon.Icon;
            ammoText.gameObject.SetActive(weapon.IsRange && weapon.UsesAmmo);

            if (weapon.IsRange && weapon.UsesAmmo)
                controller.EnsureAmmoInitForCurrent();
        }

        if (weapon != null && weapon.IsRange && weapon.UsesAmmo)
        {
            int idx = inventory.CurrentIndex;
            ammoText.text = $"{controller.GetMagazine(idx)} / {controller.GetReserve(idx)}";
        }
    }
}