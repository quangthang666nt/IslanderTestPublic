using UnityEngine;
using UnityEngine.UI;

public class SXUIVMaster : MonoBehaviour
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
			slider.value = SettingsManager.Singleton.CurrentData.audioData.volumeMaster * slider.maxValue;
		}
	}

	private void OnSliderValueChanged(float newValue)
	{
		SettingsManager.Singleton.CurrentData.audioData.volumeMaster = slider.value / slider.maxValue;
		SettingsManager.Singleton.ApplySettings();
	}
}
