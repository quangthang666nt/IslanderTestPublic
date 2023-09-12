using UnityEngine;
using UnityEngine.UI;

public class SXUIOutline : MonoBehaviour
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
			toggle.isOn = SettingsManager.Singleton.CurrentData.gameplayData.outline;
		}
	}

	private void OnToggleValueChangend(bool newValue)
	{
		SettingsManager.Singleton.CurrentData.gameplayData.outline = toggle.isOn;
		if (newValue)
		{
			StructureOutline.activeOutline = true;
		}
		else
		{
			StructureOutline.activeOutline = false;
		}
	}
}
