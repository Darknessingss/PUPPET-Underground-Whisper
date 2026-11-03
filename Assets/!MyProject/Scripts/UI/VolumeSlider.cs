using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    [Header("Настройки аудио")]
    [SerializeField] private AudioMixerGroup mixerGroup;
    [SerializeField] private string exposedParameterName = "MasterVolume";

    [Header("UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text percentText;

    private string _prefsKey;

    private void Awake()
    {
        if (!slider) slider = GetComponent<Slider>();
        _prefsKey = "Volume_" + mixerGroup.name;
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnEnable()
    {
        LoadAndApply();
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void LoadAndApply()
    {
        float savedValue = PlayerPrefs.GetFloat(_prefsKey, 0.75f);
        slider.SetValueWithoutNotify(savedValue);
        ApplyVolume(savedValue);
        UpdatePercentText(savedValue);
    }

    private void OnSliderChanged(float value)
    {
        ApplyVolume(value);
        UpdatePercentText(value);
        PlayerPrefs.SetFloat(_prefsKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyVolume(float sliderValue)
    {
        mixerGroup.audioMixer.SetFloat(exposedParameterName, Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f);
    }

    private void UpdatePercentText(float value)
    {
        percentText.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}