using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private WeaponInventory inventory;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image weaponIconImage;

    private int currentMagazineAmmo;
    private int currentReserveAmmo;
    private WeaponItem lastWeapon;

    private void Update()
    {
        var weapon = inventory.CurrentWeapon;
        if (weapon == lastWeapon) 
            return;

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

        bool showAmmo = weapon.IsRange && weapon.UsesAmmo;
        ammoText.gameObject.SetActive(showAmmo);

        if (showAmmo)
        {
            currentMagazineAmmo = weapon.MagazineSize;
            currentReserveAmmo = weapon.MaxReserveAmmo;
            ammoText.text = $"{currentMagazineAmmo} / {currentReserveAmmo}";
        }
    }
}