using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private WeaponItem[] slots = new WeaponItem[5];
    [SerializeField] private int currentIndex = 0;

    public int SlotCount => slots.Length;
    public int CurrentIndex => currentIndex;

    public WeaponItem CurrentWeapon =>
        (currentIndex >= 0 && currentIndex < slots.Length) ? slots[currentIndex] : null;

    public WeaponItem GetSlot(int index)
    {
        if (index < 0 || index >= slots.Length) 
            return null;
        return slots[index];
    }

    public bool TryAddWeapon(WeaponItem item)
    {
        if (item == null) 
            return false;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = item;
                return true;
            }
        }
        return false;
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) 
            return;
        if (slots[index] == null) 
            return;
        currentIndex = index;
    }
}