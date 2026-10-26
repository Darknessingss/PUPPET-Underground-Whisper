using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHotbarInput : MonoBehaviour
{
    private static readonly Key[] DigitKeys =
    {
        Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5,
        Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9
    };

    [SerializeField] private WeaponInventory inventory;

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) 
            return;

        int limit = Mathf.Min(DigitKeys.Length, inventory.SlotCount);
        for (int i = 0; i < limit; i++)
        {
            if (keyboard[DigitKeys[i]].wasPressedThisFrame)
                inventory.SelectSlot(i);
        }
    }
}