using System;
using System.Collections.Generic;
using UnityEngine;

public class LoreManager : MonoBehaviour
{
    public static LoreManager Instance { get; private set; }

    [SerializeField] private List<LoreEntry> entries = new List<LoreEntry>();
    [SerializeField] private List<LoreSlotUI> slots = new List<LoreSlotUI>();
    [SerializeField] private int totalShardsNeeded = 10;

    [Header("Цвета текста")]
    [SerializeField] private Color openTextColor = Color.white;

    private int shardsCollected;

    public event Action OnLoreUpdated;

    public IReadOnlyList<LoreEntry> Entries => entries;
    public int ShardsCollected => shardsCollected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        RefreshAll();
    }

    public void CollectShard()
    {
        if (shardsCollected >= totalShardsNeeded) 
            return;

        shardsCollected++;

        for (int i = 0; i < entries.Count; i++)
        {
            if (!entries[i].isUnlocked)
            {
                entries[i].isUnlocked = true;
                break;
            }
        }

        RefreshAll();
        OnLoreUpdated?.Invoke();
    }

    public bool TryUnlock(int index)
    {
        if (index < 0 || index >= entries.Count || entries[index].isUnlocked)
            return false;

        entries[index].isUnlocked = true;
        RefreshAll();
        OnLoreUpdated?.Invoke();
        return true;
    }

    public void RefreshAll()
    {
        int count = Mathf.Min(entries.Count, slots.Count);
        for (int i = 0; i < count; i++)
            ApplySlotState(i);
    }

    private void ApplySlotState(int index)
    {
        var entry = entries[index];
        var slot = slots[index];

        if (entry.isUnlocked)
        {
            slot.TitleText.text = entry.title;
            SetTextColor(slot.TitleText, openTextColor);
        }
        else
        {
            slot.TitleText.text = slot.LockedText;
            SetTextColor(slot.TitleText, slot.LockedTextColor);
        }
    }

    private void SetTextColor(TMPro.TextMeshProUGUI textComponent, Color color)
    {
        color.a = 1f;
        textComponent.color = color;
    }
}