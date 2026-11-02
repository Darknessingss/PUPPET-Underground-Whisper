using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class LoreSlotUI
{
    [Header("Ссылки на UI панельки")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Закрытое состояние")]
    [SerializeField] private string lockedText = "?????";
    [SerializeField] private Color lockedTextColor = Color.gray;

    public Image BackgroundImage => backgroundImage;
    public TextMeshProUGUI TitleText => titleText;
    public string LockedText => lockedText;
    public Color LockedTextColor => lockedTextColor;
}