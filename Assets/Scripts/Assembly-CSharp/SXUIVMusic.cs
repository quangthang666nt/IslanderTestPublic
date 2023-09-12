using UnityEngine;
using UnityEngine.UI;

public class SXUIVMusic : MonoBehaviour
{
	public Slider slider;

	private void Start()
	{
		slider.onValueChanged.AddListener(OnSliderValueChanged);
	}

	private void OnEnable()
	{
		if (!(SettingsManager.Singleton == null))
		{
			slider.value = SettingsManager.Singleton.CurrentData.audioData.volumeMusic * slider.maxValue;
		}
	}

	private void OnSliderValueChanged(float newValue)
	{
		SettingsManager.Singleton.CurrentData.audioData.volumeMusic = slider.value / slider.maxValue;
		SettingsManager.Singleton.ApplySettings();
	}
}
