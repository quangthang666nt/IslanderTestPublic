using UnityEngine;
using UnityEngine.UI;

public class SXUIVsync : MonoBehaviour
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
			toggle.isOn = SettingsManager.Singleton.CurrentData.videoData.vSync;
		}
	}

	private void OnToggleValueChangend(bool newValue)
	{
		SettingsManager.Singleton.CurrentData.videoData.vSync = toggle.isOn;
		SettingsManager.Singleton.ApplySettings();
	}
}
