using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIThemedEvents : MonoBehaviour
{
	[SerializeField]
	private CatalogConfig.EPackage debugPackage = CatalogConfig.EPackage.ValentinesDay;

	private string packageid;

	public void ChangeTheme(string theme, bool includeSfx, bool modifyPlaylist)
	{
		if (string.IsNullOrEmpty(theme))
		{
			CleanTheme(modifyPlaylist);
			return;
		}
		packageid = theme;
		StartCoroutine(LoadTheme(includeSfx, packageid, modifyPlaylist));
	}

	[ContextMenu("Change to debug theme")]
	public void ChangeToDebugTheme()
	{
		string packageID = CosmeticsManager.singleton.CatalogConfig.GetPackageID(debugPackage);
		StartCoroutine(LoadTheme(includeSfx: true, packageID, modifyPlaylist: false));
	}

	public void SkipTheme()
	{
		CosmeticsManager.Cosmetics.ask = false;
		CosmeticsManager.Cosmetics.asked = CosmeticsManager.singleton.CatalogConfig.mainPackage;
		CosmeticsManager.Save();
		UiCanvasManager.Singleton.ToPrevious();
	}

	public void CancelAction()
	{
		UiCanvasManager.Singleton.ToPrevious();
	}

	private void CleanTheme(bool modifyPlaylist)
	{
		CosmeticsManager.Cosmetics.CleanAllCosmetics();
		CosmeticsManager.Cosmetics.themePlaylist = false;
		CheckPlaylists(modifyPlaylist);
		CosmeticsManager.Save();
		SaveLoadManager.PerformReloadIsland();
	}

	private IEnumerator LoadTheme(bool includeSfx, string theme, bool modifyPlaylist)
	{
		CosmeticsManager.Cosmetics.CleanAllCosmetics();
		AsyncOperationHandle<IList<GameObject>> asyncOperationHandle = Addressables.LoadAssetsAsync(new List<string> { theme }, delegate(GameObject obj)
		{
			CatalogElement component = obj.GetComponent<CatalogElement>();
			if (!(component == null))
			{
				CosmeticsManager.Cosmetics.AddCosmetic(component.id, CatalogHelper.GetElementId(theme, component.id.id));
			}
		}, Addressables.MergeMode.Union);
		yield return asyncOperationHandle;
		CosmeticsManager.Cosmetics.theme = theme;
		if (includeSfx)
		{
			CosmeticsManager.Cosmetics.themePlaylist = true;
			CheckPlaylists(modifyPlaylist);
		}
		CosmeticsManager.Save();
		if ((bool)FeedbackManager.Singleton)
		{
			FeedbackManager.Singleton.LoadCosmeticsVFXAsync();
		}
		SaveLoadManager.PerformReloadIsland();
	}

	private void CheckPlaylists(bool modifyPlaylist)
	{
		MusicPlaylistController component = AudioManager.singleton.GetComponent<MusicPlaylistController>();
		if ((bool)component)
		{
			if (modifyPlaylist)
			{
				component.CheckActivePlaylist();
			}
			component.CheckActiveFanfare();
		}
	}
}
