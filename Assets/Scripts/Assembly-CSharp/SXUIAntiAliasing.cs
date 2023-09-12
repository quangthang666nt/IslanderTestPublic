using System.Collections.Generic;
using UnityEngine;

public class SXUIAntiAliasing : MonoBehaviour
{
	public UISelector selector;

	private Dictionary<string, int> lookupDic = new Dictionary<string, int>();

	private void Start()
	{
		lookupDic.Add("No AA", 0);
		lookupDic.Add("2xAA", 2);
		lookupDic.Add("4xAA", 4);
		lookupDic.Add("8xAA", 8);
		selector.eventOnSelectionChange.AddListener(OnValueChange);
	}

	private void OnEnable()
	{
		if (!(SettingsManager.Singleton == null))
		{
			int index = 0;
			List<string> list = new List<string>();
			list.Add("No AA");
			list.Add("2xAA");
			list.Add("4xAA");
			list.Add("8xAA");
			switch (SettingsManager.Singleton.CurrentData.videoData.antiAliasing)
			{
			case 2:
				index = 1;
				break;
			case 4:
				index = 2;
				break;
			case 8:
				index = 3;
				break;
			}
			selector.options = list;
			selector.SetIndex(index);
		}
	}

	private void OnValueChange()
	{
		int value = 0;
		lookupDic.TryGetValue(selector.options[selector.Index], out value);
		SettingsManager.Singleton.CurrentData.videoData.antiAliasing = value;
		SettingsManager.Singleton.ApplySettings();
	}
}
