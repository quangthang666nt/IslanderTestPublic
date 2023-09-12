using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

public class SXUIShadows : MonoBehaviour
{
	public UISelector selector;

	public LocalizedString descritorLow = "Settings / Video / Quality Descriptors/'High'";

	public LocalizedString descritorMedium = "Settings / Video / Quality Descriptors/'High'";

	public LocalizedString descritorHigh = "Settings / Video / Quality Descriptors/'High'";

	public LocalizedString descritorUltra = "Settings / Video / Quality Descriptors/'High'";

	private void Start()
	{
		selector.eventOnSelectionChange.AddListener(OnSelectionChange);
	}

	private void OnEnable()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < 4; i++)
		{
			switch (i)
			{
			case 0:
				list.Add(descritorLow);
				break;
			case 1:
				list.Add(descritorMedium);
				break;
			case 2:
				list.Add(descritorHigh);
				break;
			case 3:
				list.Add(descritorUltra);
				break;
			}
		}
		selector.options = list;
		selector.SetIndex(SettingsManager.Singleton.CurrentData.videoData.shadowResolution);
	}

	private void OnSelectionChange()
	{
		SettingsManager.Singleton.CurrentData.videoData.shadowResolution = selector.Index;
		SettingsManager.Singleton.ApplySettings();
	}
}
