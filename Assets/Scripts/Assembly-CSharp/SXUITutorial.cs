using UnityEngine;
using UnityEngine.UI;

public class SXUITutorial : MonoBehaviour
{
	public Toggle toggle;

	private void Start()
	{
		toggle.onValueChanged.AddListener(OnToggleValueChangend);
	}

	private void OnEnable()
	{
		if (!(SettingsManager.Singleton == null))
		{
			toggle.isOn = SettingsManager.Singleton.CurrentData.gameplayData.bShowTutorial;
		}
	}

	private void OnToggleValueChangend(bool newValue)
	{
		SettingsManager.Singleton.CurrentData.gameplayData.bShowTutorial = toggle.isOn;
		SettingsManager.Singleton.ApplySettings();
	}
}
