using System.Collections.Generic;
using UnityEngine;

public class SXUIVideoResolution : MonoBehaviour
{
	public static SXUIVideoResolution singleton;

	public UISelector selector;

	private static List<Resolution> possibleResolutions = new List<Resolution>();

	private Resolution currentResolution;

	private const int minScreenWidth = 800;

	private const int minScreenHeight = 600;

	private const int minRefreshRate = 1;

	private void Awake()
	{
		singleton = this;
		UpdatePossibleResolutions();
	}

	private void Start()
	{
		selector.eventOnSelectionChange.AddListener(OnSelectionChange);
	}

	public static void UpdatePossibleResolutions()
	{
		possibleResolutions = new List<Resolution>(Screen.resolutions);
		for (int num = possibleResolutions.Count - 1; num >= 0; num--)
		{
			if (possibleResolutions[num].height < 600 || possibleResolutions[num].width < 800 || possibleResolutions[num].refreshRate < 1)
			{
				possibleResolutions.RemoveAt(num);
			}
		}
	}

	public static bool BIsResolutionSupported(int _iWidth, int _iHeight, int _iRefreshRate)
	{
		UpdatePossibleResolutions();
		for (int num = possibleResolutions.Count - 1; num >= 0; num--)
		{
			if (possibleResolutions[num].width == _iWidth && possibleResolutions[num].height == _iHeight && possibleResolutions[num].refreshRate == _iRefreshRate)
			{
				return true;
			}
		}
		return false;
	}

	public static Resolution ResolutionGetBestDefault()
	{
		if (BIsResolutionSupported(Screen.currentResolution.width, Screen.currentResolution.height, Screen.currentResolution.refreshRate))
		{
			return Screen.currentResolution;
		}
		Resolution result = default(Resolution);
		if (BIsResolutionSupported(Screen.width, Screen.height, Screen.currentResolution.refreshRate))
		{
			result.width = Screen.width;
			result.height = Screen.height;
			result.refreshRate = Screen.currentResolution.refreshRate;
			return result;
		}
		UpdatePossibleResolutions();
		int width = 1920;
		int height = 1080;
		int refreshRate = 60;
		double num = -1.0;
		for (int num2 = possibleResolutions.Count - 1; num2 >= 0; num2--)
		{
			double num3 = possibleResolutions[num2].width * possibleResolutions[num2].height * possibleResolutions[num2].refreshRate;
			if (num3 > num)
			{
				width = possibleResolutions[num2].width;
				height = possibleResolutions[num2].height;
				refreshRate = possibleResolutions[num2].refreshRate;
				num = num3;
			}
		}
		result.width = width;
		result.height = height;
		result.refreshRate = refreshRate;
		return result;
	}

	private void OnEnable()
	{
		UpdatePossibleResolutions();
		if (SettingsManager.Singleton.CurrentData.videoData.resolution != null)
		{
			SettingsManager.SettingsData.VideoData.SerializableResolution resolution = SettingsManager.Singleton.CurrentData.videoData.resolution;
			currentResolution = default(Resolution);
			currentResolution.height = resolution.height;
			currentResolution.width = resolution.width;
			currentResolution.refreshRate = resolution.refreshRate;
		}
		else
		{
			currentResolution = ResolutionGetBestDefault();
		}
		int num = -1;
		List<string> list = new List<string>();
		for (int i = 0; i < possibleResolutions.Count; i++)
		{
			if (possibleResolutions[i].width == currentResolution.width && possibleResolutions[i].height == currentResolution.height && possibleResolutions[i].refreshRate == currentResolution.refreshRate)
			{
				num = i;
			}
			list.Add(possibleResolutions[i].width + "x" + possibleResolutions[i].height + " @" + possibleResolutions[i].refreshRate + "hz");
		}
		if (num == -1)
		{
			num = list.Count - 1;
		}
		selector.options = list;
		selector.SetIndex(num);
	}

	private void OnSelectionChange()
	{
		currentResolution = possibleResolutions[selector.Index];
	}

	public void ApplySettings()
	{
		if (base.gameObject.activeInHierarchy)
		{
			SettingsManager.Singleton.CurrentData.videoData.resolution = new SettingsManager.SettingsData.VideoData.SerializableResolution(currentResolution);
			SettingsManager.Singleton.ApplySettings();
		}
	}
}
