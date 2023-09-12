using UnityEngine;
using UnityEngine.UI;

public class SXUIJoystickSensibility : MonoBehaviour
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
			slider.value = SettingsManager.Singleton.CurrentData.gameplayData.PointerMovementSensibility;
		}
	}

	private void OnSliderValueChanged(float newValue)
	{
		SettingsManager.Singleton.CurrentData.gameplayData.PointerMovementSensibility = slider.value;
		SettingsManager.Singleton.ApplySettings();
	}
}
