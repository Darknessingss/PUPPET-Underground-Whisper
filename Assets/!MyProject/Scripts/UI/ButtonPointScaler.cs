using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPointScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Настройки анимации")]
    [SerializeField] private float hoverScale = 1.035f;
    [SerializeField] private float animationDuration = 0.15f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private RectTransform rectTransform;

    private Vector3 _originalScale;
    private Coroutine _animationCoroutine;

    private void Awake()
    {
        _originalScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScaleAnimation(_originalScale * hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScaleAnimation(_originalScale);
    }

    private void StartScaleAnimation(Vector3 targetScale)
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(AnimateScale(targetScale));
    }

    private System.Collections.IEnumerator AnimateScale(Vector3 targetScale)
    {
        Vector3 startScale = rectTransform.localScale;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float curveT = easeCurve.Evaluate(t);
            rectTransform.localScale = Vector3.LerpUnclamped(startScale, targetScale, curveT);
            yield return null;
        }

        rectTransform.localScale = targetScale;
        _animationCoroutine = null;
    }
}