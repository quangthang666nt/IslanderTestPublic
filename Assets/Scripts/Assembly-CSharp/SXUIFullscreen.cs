using UnityEngine;
using UnityEngine.UI;

public class SXUIFullscreen : MonoBehaviour
{
	public Toggle toggle;

	private void OnEnable()
	{
		if (!(SettingsManager.Singleton == null))
		{
			toggle.isOn = SettingsManager.Singleton.CurrentData.videoData.fullScreen;
		}
	}

	public void ApplySettings()
	{
		if (base.gameObject.activeInHierarchy)
		{
			SettingsManager.Singleton.CurrentData.videoData.fullScreen = toggle.isOn;
			SettingsManager.Singleton.ApplySettings();
		}
	}
}
