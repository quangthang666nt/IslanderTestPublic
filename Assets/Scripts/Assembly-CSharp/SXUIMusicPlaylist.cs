using System.Collections;
using I2.Loc;
using UnityEngine;

public class SXUIMusicPlaylist : MonoBehaviour
{
	public LocalizedString lsOriginalMusic;

	public LocalizedString lsEventMusic;

	public UISelector selector;

	public UISettingsMenu menu;

	private int currentPlaylistIndex;

	private bool playlistUpdated;

	private void Start()
	{
		selector.eventOnSelectionChange.AddListener(OnSelectionChange);
	}

	private void OnEnable()
	{
		playlistUpdated = false;
		if (AreCosmeticsReady())
		{
			SetupOptions();
		}
		else
		{
			StartCoroutine(WaitForCosmetics());
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		if (playlistUpdated)
		{
			CosmeticsManager.Save();
		}
	}

	private void OnSelectionChange()
	{
		bool themePlaylist = CosmeticsManager.Cosmetics.themePlaylist;
		CosmeticsManager.Cosmetics.themePlaylist = selector.Index == 1;
		if (themePlaylist != CosmeticsManager.Cosmetics.themePlaylist)
		{
			playlistUpdated = true;
			MusicPlaylistController component = AudioManager.singleton.GetComponent<MusicPlaylistController>();
			if ((bool)component)
			{
				component.CheckActivePlaylist();
			}
		}
		StartCoroutine(ResizeMenu());
	}

	private IEnumerator ResizeMenu()
	{
		yield return null;
		menu.ForceResize();
		menu.ToAudio();
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
		selector.options.Clear();
		currentPlaylistIndex = 0;
		selector.options.Add(lsOriginalMusic);
		selector.options.Add(lsEventMusic);
		if (CosmeticsManager.Cosmetics.themePlaylist)
		{
			currentPlaylistIndex = 1;
		}
		selector.SetIndex(currentPlaylistIndex);
	}
}
