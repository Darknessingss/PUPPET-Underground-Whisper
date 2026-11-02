using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class LoreSlotHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private int slotIndex;
    [SerializeField] private TextMeshProUGUI tooltipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        var entry = LoreManager.Instance.Entries[slotIndex];
        if (!entry.isUnlocked)
            return;

        tooltipText.text = entry.body;
    }
}