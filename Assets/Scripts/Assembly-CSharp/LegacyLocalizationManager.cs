using System.Collections.Generic;
using System.IO;
using Localization;
using UnityEngine;

public static class LegacyLocalizationManager
{
	private static Dictionary<string, string> dicLocalizedText;

	private const string strDefaultLocalizationFileName = "localization_master";

	private const string strMissingText = "NO TEXT AVAILABLE";

	public static string LightFont;

	public static string LightItalicFont;

	public static string RegularFont;

	public static string SemiBoldFont;

	public static string BoldFont;

	public static string ExtraBoldFont;

	static LegacyLocalizationManager()
	{
		dicLocalizedText = new Dictionary<string, string>();
		LightFont = "OpenSans-Light SDF";
		LightItalicFont = "OpenSans-LightItalic SDF";
		RegularFont = "OpenSans-Regular SDF";
		SemiBoldFont = "OpenSans-SemiBold SDF";
		BoldFont = "OpenSans-Bold SDF";
		ExtraBoldFont = "OpenSans-ExtraBold SDF";
		LoadLocalizationFile("localization_master");
	}

	public static void LoadLocalizationFile(string _strFileName)
	{
		dicLocalizedText.Clear();
		string text = Path.Combine(Application.streamingAssetsPath, _strFileName + ".json");
		if (File.Exists(text))
		{
			LocalizationData localizationData = JsonUtility.FromJson<LocalizationData>(File.ReadAllText(text));
			for (int i = 0; i < localizationData.items.Length; i++)
			{
				dicLocalizedText.Add(localizationData.items[i].key, localizationData.items[i].value);
			}
		}
		else
		{
			Debug.LogError("No file found at: " + text);
		}
	}

	public static string StrGetLocalizedString(string _strKey)
	{
		string value = "NO TEXT AVAILABLE";
		dicLocalizedText.TryGetValue(_strKey, out value);
		return value;
	}
}
