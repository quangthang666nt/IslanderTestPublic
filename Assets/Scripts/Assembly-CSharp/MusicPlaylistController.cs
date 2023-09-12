using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlaylistController : MonoBehaviour
{
	private MusicPlaylist themeFanfareOn;

	private MusicPlaylist themePlaylistOn;

	private void Awake()
	{
		if (CosmeticsReady())
		{
			CheckActivePlaylist();
		}
		else
		{
			StartCoroutine(WaitForCosmetics());
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	private IEnumerator WaitForCosmetics()
	{
		while (!CosmeticsReady())
		{
			yield return null;
		}
		CheckActivePlaylist();
	}

	private bool CosmeticsReady()
	{
		if ((bool)CosmeticsManager.singleton && CosmeticsManager.singleton.IsConfigLoaded())
		{
			return CosmeticsManager.singleton.bDataLoaded;
		}
		return false;
	}

	public void CheckActiveFanfare()
	{
		if (!string.IsNullOrEmpty(CosmeticsManager.Cosmetics.theme))
		{
			themeFanfareOn = CosmeticsManager.singleton.CatalogConfig.GetPackageMusicPlaylist(CosmeticsManager.Cosmetics.theme);
			if (themeFanfareOn != null)
			{
				if (themeFanfareOn.fanfare != null)
				{
					AudioManager.singleton.ChangeFanfare(themeFanfareOn.fanfare);
				}
				else
				{
					themeFanfareOn = null;
				}
			}
		}
		else if (themeFanfareOn != null)
		{
			themeFanfareOn = null;
			AudioManager.singleton.RestoreFanfare();
		}
	}

	public void CheckActivePlaylist()
	{
		List<AudioClip> list = new List<AudioClip>();
		if (CosmeticsManager.Cosmetics.playlists.Count > 0)
		{
			for (int i = 0; i < CosmeticsManager.Cosmetics.playlists.Count; i++)
			{
				AudioClip[] array = ((!AudioManager.singleton.IsOriginalPlaylist(CosmeticsManager.Cosmetics.playlists[i])) ? CosmeticsManager.singleton.CatalogConfig.GetPlaylistSoundtrack(CosmeticsManager.Cosmetics.playlists[i]) : AudioManager.singleton.GetOriginalPlaylistSoundtracks());
				if (array != null)
				{
					list.AddRange(array);
				}
			}
		}
		else
		{
			AudioClip[] originalPlaylistSoundtracks = AudioManager.singleton.GetOriginalPlaylistSoundtracks();
			if (originalPlaylistSoundtracks != null)
			{
				list.AddRange(originalPlaylistSoundtracks);
			}
		}
		if (list.Count > 1)
		{
			list.Sort((AudioClip a, AudioClip b) => 1 - 2 * Random.Range(0, 2));
		}
		AudioManager.singleton.ChangeMusicPlaylist(list);
	}
}
