using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.Events;

public class SXUILanguage : MonoBehaviour
{
	[Serializable]
	public struct LocalisedLanguage
	{
		public string LanguageName;

		public string LocalisedName;
	}

	public UISelector selector;

	public UISettingsMenu menu;

	private List<string> availableLanguages;

	public List<LocalisedLanguage> m_PotentialLanguages;

	private int currentLanguageIndex;

	public UnityEvent OnSelectionChangeEvent;

	private void Start()
	{
		selector.eventOnSelectionChange.AddListener(OnSelectionChange);
		availableLanguages = LocalizationManager.GetAllLanguages();
		selector.options.Clear();
		currentLanguageIndex = -1;
		for (int i = 0; i < availableLanguages.Count; i++)
		{
			for (int j = 0; j < m_PotentialLanguages.Count; j++)
			{
				if (availableLanguages[i] == m_PotentialLanguages[j].LanguageName)
				{
					selector.options.Add(m_PotentialLanguages[j].LocalisedName);
					if (availableLanguages[i] == LocalizationManager.CurrentLanguage)
					{
						currentLanguageIndex = selector.options.Count - 1;
					}
					break;
				}
			}
		}
		selector.SetIndex(currentLanguageIndex);
	}

	private void OnEnable()
	{
		availableLanguages = LocalizationManager.GetAllLanguages();
		selector.options.Clear();
		currentLanguageIndex = -1;
		for (int i = 0; i < availableLanguages.Count; i++)
		{
			for (int j = 0; j < m_PotentialLanguages.Count; j++)
			{
				if (availableLanguages[i] == m_PotentialLanguages[j].LanguageName)
				{
					selector.options.Add(m_PotentialLanguages[j].LocalisedName);
					if (availableLanguages[i] == LocalizationManager.CurrentLanguage)
					{
						currentLanguageIndex = selector.options.Count - 1;
					}
					break;
				}
			}
		}
		selector.SetIndex(currentLanguageIndex);
	}

	private void OnSelectionChange()
	{
		string text = selector.options[selector.Index];
		string text2 = string.Empty;
		for (int i = 0; i < m_PotentialLanguages.Count; i++)
		{
			if (text == m_PotentialLanguages[i].LocalisedName)
			{
				text2 = m_PotentialLanguages[i].LanguageName;
				break;
			}
		}
		string currentLanguage = text2;
		SettingsManager.Singleton.CurrentData.gameplayData.currentLanguage = currentLanguage;
		SettingsManager.Singleton.ApplySettings();
		StartCoroutine(ResizeMenu());
		OnSelectionChangeEvent?.Invoke();
	}

	private IEnumerator ResizeMenu()
	{
		yield return null;
		menu.ForceResize();
		menu.ToGameplay();
	}
}
