using UnityEngine;
using UnityEngine.UI;

public class SXUIBloom : MonoBehaviour
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
			toggle.isOn = SettingsManager.Singleton.CurrentData.gameplayData.bloom;
		}
	}

	private void OnToggleValueChangend(bool newValue)
	{
		SettingsManager.Singleton.CurrentData.gameplayData.bloom = toggle.isOn;
		if (newValue)
		{
			ColorGenerator.singleton.EnableBloom(value: true);
		}
		else
		{
			ColorGenerator.singleton.EnableBloom(value: false);
		}
	}
}
