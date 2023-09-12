using SCS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class SXUIDayNightCycle : MonoBehaviour
{
	[SerializeField]
	private GameObject[] dncycleRefs;

	public Toggle toggle;

	private void Start()
	{
		toggle.onValueChanged.AddListener(OnToggleValueChangend);
	}

	private void OnEnable()
	{
		if (!(SettingsManager.Singleton == null))
		{
			toggle.isOn = SettingsManager.Singleton.CurrentData.gameplayData.enableDayNightCycle;
			UpdateDNCycleSlider(toggle.isOn);
		}
	}

	private void OnToggleValueChangend(bool newValue)
	{
		SettingsManager.Singleton.CurrentData.gameplayData.enableDayNightCycle = toggle.isOn;
		if (newValue)
		{
			DayNightCycle.Instance.Enable();
		}
		else
		{
			DayNightCycle.Instance.Disable();
		}
		UpdateDNCycleSlider(newValue);
	}

	private void UpdateDNCycleSlider(bool isActive)
	{
		if (dncycleRefs != null)
		{
			for (int i = 0; i < dncycleRefs.Length; i++)
			{
				dncycleRefs[i].SetActive(isActive);
			}
		}
	}
}
