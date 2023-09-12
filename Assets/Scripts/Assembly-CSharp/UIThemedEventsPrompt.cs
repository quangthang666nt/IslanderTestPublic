using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIThemedEventsPrompt : MonoBehaviour
{
	public TMP_Text tmptHeader;

	public Image headerBackground;

	[SerializeField]
	private UIThemedEvents themedEvents;

	[SerializeField]
	private bool includeSfx = true;

	private bool hasBeenConfigured;

	private void Update()
	{
		if (!hasBeenConfigured && CosmeticsManager.singleton.IsConfigLoaded())
		{
			if ((bool)tmptHeader)
			{
				tmptHeader.text = CosmeticsManager.singleton.CatalogConfig.publicMainPackageHeader;
			}
			if ((bool)headerBackground && CosmeticsManager.singleton.CatalogConfig.mainPackageHeader != null)
			{
				headerBackground.sprite = CosmeticsManager.singleton.CatalogConfig.mainPackageHeader;
				headerBackground.color = Color.white;
				headerBackground.type = Image.Type.Simple;
				headerBackground.preserveAspect = true;
				headerBackground.SetNativeSize();
				headerBackground.gameObject.SetActive(value: true);
			}
			hasBeenConfigured = true;
		}
	}

	public void Confirm()
	{
		CosmeticsManager.Cosmetics.ask = false;
		CosmeticsManager.Cosmetics.asked = CosmeticsManager.singleton.CatalogConfig.mainPackage;
		OverridePlaylistIfAvailable();
		themedEvents.ChangeTheme(CosmeticsManager.singleton.CatalogConfig.mainPackage, includeSfx, modifyPlaylist: true);
	}

	private void OverridePlaylistIfAvailable()
	{
		MusicPlaylist packageMusicPlaylist = CosmeticsManager.singleton.CatalogConfig.GetPackageMusicPlaylist(CosmeticsManager.singleton.CatalogConfig.mainPackage);
		if (packageMusicPlaylist != null && packageMusicPlaylist.soundtrack != null && packageMusicPlaylist.soundtrack.Length != 0)
		{
			CosmeticsManager.Cosmetics.playlists.Clear();
			CosmeticsManager.Cosmetics.playlists.Add(CosmeticsManager.singleton.CatalogConfig.mainPackage);
		}
	}

	public void Cancel()
	{
		themedEvents.SkipTheme();
	}
}
