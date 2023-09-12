using UnityEngine;
using UnityEngine.UI;

public class SXUIDragBuildmode : MonoBehaviour
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
			toggle.isOn = SettingsManager.Singleton.CurrentData.gameplayData.dragCameraInBuildmode;
		}
	}

	private void OnToggleValueChangend(bool newValue)
	{
		SettingsManager.Singleton.CurrentData.gameplayData.dragCameraInBuildmode = toggle.isOn;
		SettingsManager.Singleton.ApplySettings();
	}
}
