using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SXUIThemes : MonoBehaviour
{
	[SerializeField]
	private UISelector selector;

	[SerializeField]
	private UISettingsMenu menu;

	[SerializeField]
	private UIUpdateThemedEvent updateThemedEventPrompt;

	[SerializeField]
	private SXUILanguage uiLanguage;

	private bool themeUpdated;

	private int playerPreviousIndex;

	private List<CatalogConfig.PackageData> packagesData;

	private void Start()
	{
		selector.eventOnSelectionChange.AddListener(OnSelectionChange);
	}

	private void OnEnable()
	{
		themeUpdated = false;
		if (AreCosmeticsReady())
		{
			SetupOptions();
		}
		else
		{
			StartCoroutine(WaitForCosmetics());
		}
		uiLanguage.OnSelectionChangeEvent.AddListener(ResetLabels);
	}

	private void OnDisable()
	{
		if (themeUpdated)
		{
			string themeId = string.Empty;
			if (selector.Index != 0 && packagesData.Count > selector.Index - 1)
			{
				themeId = packagesData[selector.Index - 1].id;
			}
			updateThemedEventPrompt.themeId = themeId;
			UiCanvasManager.Singleton.ToUpdateThemeEvent();
		}
		uiLanguage.OnSelectionChangeEvent.RemoveListener(ResetLabels);
	}

	private bool AreCosmeticsReady()
	{
		if ((bool)CosmeticsManager.singleton && CosmeticsManager.singleton.IsConfigLoaded())
		{
			return CosmeticsManager.singleton.bDataLoaded;
		}
		return false;
	}

	private IEnumerator WaitForCosmetics()
	{
		while (!AreCosmeticsReady())
		{
			yield return null;
		}
		SetupOptions();
	}

	private void SetupOptions()
	{
		packagesData = CosmeticsManager.singleton.CatalogConfig.GetAllAvailablePackagesData();
		selector.options.Clear();
		selector.options.Add(CosmeticsManager.singleton.CatalogConfig.originalLocalizedName);
		playerPreviousIndex = 0;
		for (int i = 0; i < packagesData.Count; i++)
		{
			selector.options.Add(packagesData[i].localizedName);
			if (packagesData[i].id.Equals(CosmeticsManager.Cosmetics.theme))
			{
				playerPreviousIndex = i + 1;
			}
		}
		selector.SetIndex(playerPreviousIndex);
	}

	private void ResetLabels()
	{
		themeUpdated = false;
		if (AreCosmeticsReady())
		{
			SetupOptions();
		}
	}

	private void OnSelectionChange()
	{
		themeUpdated = selector.Index != playerPreviousIndex;
		StartCoroutine(ResizeMenu());
	}

	private IEnumerator ResizeMenu()
	{
		yield return null;
		menu.ForceResize();
		menu.ToGameplay();
	}
}
