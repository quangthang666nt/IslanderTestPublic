using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PackageLoader : MonoBehaviour
{
	[SerializeField]
	private TMP_Dropdown dropdown;

	[SerializeField]
	private bool rebakeIslandAfterChanges;

	private void Start()
	{
		LoadCatalogConfig();
	}

	private void LoadCatalogConfig()
	{
		AsyncOperationHandle<CatalogConfig> asyncOperationHandle = Addressables.LoadAssetAsync<CatalogConfig>(CatalogConfig.CATALOG_CONFIG_LABEL);
		asyncOperationHandle.Completed += OnConfigurationLoaded;
	}

	private void OnConfigurationLoaded(AsyncOperationHandle<CatalogConfig> opHandle)
	{
		List<string> availablePackages = opHandle.Result.GetAvailablePackages();
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		for (int i = 0; i < availablePackages.Count; i++)
		{
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
			optionData.text = availablePackages[i];
			list.Add(optionData);
		}
		dropdown.options = list;
	}

	public void LoadPackage()
	{
		if (dropdown.options == null && dropdown.options.Count == 0)
		{
			Debug.LogError("There is not catalog options");
			return;
		}
		string text = dropdown.options[dropdown.value].text;
		StartCoroutine(LoadCatalog(text));
	}

	private IEnumerator LoadCatalog(string catalog)
	{
		AsyncOperationHandle<IList<GameObject>> asyncOperationHandle = Addressables.LoadAssetsAsync(new List<string> { catalog }, delegate(GameObject obj)
		{
			CatalogElement component = obj.GetComponent<CatalogElement>();
			if (!(component == null))
			{
				CosmeticsManager.Cosmetics.AddCosmetic(component.id, CatalogHelper.GetElementId(catalog, component.id.id));
			}
		}, Addressables.MergeMode.Union);
		yield return asyncOperationHandle;
		CosmeticsManager.Save();
		if (rebakeIslandAfterChanges)
		{
			SaveLoadManager.PerformReloadIsland();
		}
		else
		{
			CosmeticsManager.LaunchComesticsUpdate();
		}
	}

	public void CleanPackages()
	{
		CosmeticsManager.Cosmetics.CleanAllCosmetics();
		CosmeticsManager.Save();
		if (rebakeIslandAfterChanges)
		{
			SaveLoadManager.PerformReloadIsland();
		}
		else
		{
			CosmeticsManager.LaunchComesticsUpdate();
		}
	}
}
