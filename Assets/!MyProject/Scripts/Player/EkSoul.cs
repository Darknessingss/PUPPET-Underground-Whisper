using UnityEngine;
using UnityEngine.UI;

public class EkSoul : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxSoul = 100f;
    [SerializeField] private float drainPerSecond = 5f;
    [SerializeField] private float restorePerSecond = 1f;

    [Header("Light detection")]
    [SerializeField] private LayerMask lightMask;
    [SerializeField] private float lightCheckRadius = 0.5f;
    [SerializeField] private Vector2 lightCheckOffset = Vector2.zero;

    [Header("Refs")]
    [SerializeField] private EkHealth health;
    [SerializeField] private Image soulFill;

    private float _currentSoul;

    public float CurrentSoul => _currentSoul;
    public float Normalized => _currentSoul / maxSoul;

    private void Awake() => _currentSoul = maxSoul;

    private void Update()
    {
        bool inLight = Physics2D.OverlapCircle(
            (Vector2)transform.position + lightCheckOffset,
            lightCheckRadius,
            lightMask) != null;

        _currentSoul += (inLight ? restorePerSecond : -drainPerSecond) * Time.deltaTime;
        _currentSoul = Mathf.Clamp(_currentSoul, 0f, maxSoul);

        soulFill.fillAmount = Normalized;
    }
}