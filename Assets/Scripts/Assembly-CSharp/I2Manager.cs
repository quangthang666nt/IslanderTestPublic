using System;
using System.Collections.Generic;
using I2.Loc;
using Rewired.UI.ControlMapper;
using UnityEngine;

public class I2Manager : MonoBehaviour
{
	public static I2Manager Instance;

	public LocalizationParamsManager localizationParamsManager;

	[SerializeField]
	[Help("For communicating with I2, the localization plugin we're using.", MessageType.Info)]
	private ControlMapper controlMapper;

	private event Action<string> m_OnLanguageChange;

	public static event Action<string> OnLanguageChange
	{
		add
		{
			if (Instance != null)
			{
				Instance.m_OnLanguageChange -= value;
				Instance.m_OnLanguageChange += value;
			}
		}
		remove
		{
			if (Instance != null)
			{
				Instance.m_OnLanguageChange -= value;
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
	}

	public static void SetInitialLanguage(string languageName)
	{
		if (Instance != null)
		{
			Instance.SetLanguage(languageName);
		}
	}

	public void SetLanguage(string languageName)
	{
		if (LocalizationManager.HasLanguage(languageName))
		{
			LocalizationManager.CurrentLanguage = languageName;
		}
		else
		{
			LocalizationManager.CurrentLanguage = LocalizationManager.GetAllLanguages()[0];
		}
		if (this.m_OnLanguageChange != null)
		{
			this.m_OnLanguageChange(languageName);
		}
		ResetControlMapper();
	}

	public void CycleToNextLanguage()
	{
		List<string> allLanguages = LocalizationManager.GetAllLanguages();
		int num = allLanguages.IndexOf(LocalizationManager.CurrentLanguage);
		num = (num + 1) % allLanguages.Count;
		SetLanguage(allLanguages[num]);
	}

	private void ResetControlMapper()
	{
		if ((bool)controlMapper)
		{
			controlMapper.Reset();
		}
	}
}
