using UnityEngine;
using UnityEngine.UI;

public class SXUIVWorld : MonoBehaviour
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
			slider.value = SettingsManager.Singleton.CurrentData.audioData.volumeWorld * slider.maxValue;
		}
	}

	private void OnSliderValueChanged(float newValue)
	{
		SettingsManager.Singleton.CurrentData.audioData.volumeWorld = slider.value / slider.maxValue;
		SettingsManager.Singleton.ApplySettings();
	}
}
