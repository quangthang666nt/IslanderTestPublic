using UnityEngine;
using UnityEngine.UI;

public class SXUIVScore : MonoBehaviour
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
			slider.value = SettingsManager.Singleton.CurrentData.audioData.volumeScore * slider.maxValue;
		}
	}

	private void OnSliderValueChanged(float newValue)
	{
		SettingsManager.Singleton.CurrentData.audioData.volumeScore = slider.value / slider.maxValue;
		SettingsManager.Singleton.ApplySettings();
	}
}
