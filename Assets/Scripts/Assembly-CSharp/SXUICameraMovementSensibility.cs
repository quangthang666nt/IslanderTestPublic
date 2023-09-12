using UnityEngine;
using UnityEngine.UI;

public class SXUICameraMovementSensibility : MonoBehaviour
{
	public Slider slider;

	private void Start()
	{
		slider.onValueChanged.AddListener(OnSliderValueChanged);
		slider.minValue = -0.5f;
		slider.maxValue = 0.5f;
	}

	private void OnEnable()
	{
		if (!(SettingsManager.Singleton == null))
		{
			slider.value = SettingsManager.Singleton.CurrentData.gameplayData.CameraMovementSensibility;
		}
	}

	private void OnSliderValueChanged(float newValue)
	{
		SettingsManager.Singleton.CurrentData.gameplayData.CameraMovementSensibility = slider.value;
		SettingsManager.Singleton.ApplySettings();
	}
}
