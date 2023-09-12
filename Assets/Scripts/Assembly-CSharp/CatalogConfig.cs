using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

[CreateAssetMenu(menuName = "Islanders/Catalog Config")]
public class CatalogConfig : ScriptableObject
{
	public enum EPackage
	{
		None = 0,
		Christmas = 1,
		ValentinesDay = 2,
		EasterDay = 3,
		Summmer = 4,
		Halloween = 5
	}

	[Serializable]
	public struct PackageData
	{
		public EPackage type;

		public string id;

		public LocalizedString localizedName;

		public Sprite titleLogo;

		public MusicPlaylist playlist;
	}

	[Serializable]
	public struct ExtraPlaylist
	{
		public string id;

		public LocalizedString localizedName;

		public AudioClip[] soundtracks;
	}

	public static string CATALOG_CONFIG_LABEL = "Configuration";

	public List<string> availablePackages = new List<string>();

	[Header("Public Main Package Presentation")]
	public string mainPackage = "";

	public LocalizedString publicMainPackageHeader;

	public Sprite mainPackageHeader;

	[Header("Packages Data")]
	public LocalizedString originalLocalizedName;

	[SerializeField]
	private PackageData[] packages;

	[Header("Extras playlists")]
	public ExtraPlaylist[] extrasPlaylists;

	public List<string> GetAvailablePackages()
	{
		return new List<string>(availablePackages);
	}

	public string GetPackageID(EPackage ePackage)
	{
		for (int i = 0; i < packages.Length; i++)
		{
			if (packages[i].type == ePackage)
			{
				return packages[i].id;
			}
		}
		return string.Empty;
	}

	private PackageData GetPackageData(string id)
	{
		for (int i = 0; i < packages.Length; i++)
		{
			if (packages[i].id.Equals(id))
			{
				return packages[i];
			}
		}
		return default(PackageData);
	}

	public Sprite GetPackageTitleLogo(string id)
	{
		return GetPackageData(id).titleLogo;
	}

	public MusicPlaylist GetPackageMusicPlaylist(string id)
	{
		return GetPackageData(id).playlist;
	}

	public List<PackageData> GetAllAvailablePackagesData()
	{
		List<PackageData> list = new List<PackageData>();
		for (int i = 0; i < availablePackages.Count; i++)
		{
			PackageData packageData = GetPackageData(availablePackages[i]);
			if (!string.IsNullOrEmpty(packageData.id))
			{
				list.Add(packageData);
			}
		}
		return list;
	}

	public bool IsExtraPlaylistAvailable(string playlistID)
	{
		if (extrasPlaylists == null || extrasPlaylists.Length == 0 || string.IsNullOrEmpty(playlistID))
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < extrasPlaylists.Length; i++)
		{
			if (playlistID.Equals(extrasPlaylists[i].id))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public AudioClip[] GetPlaylistSoundtrack(string playlistID)
	{
		if (string.IsNullOrEmpty(playlistID))
		{
			return null;
		}
		if (extrasPlaylists != null && extrasPlaylists.Length != 0)
		{
			for (int i = 0; i < extrasPlaylists.Length; i++)
			{
				if (playlistID.Equals(extrasPlaylists[i].id))
				{
					return extrasPlaylists[i].soundtracks;
				}
			}
		}
		AudioClip[] result = null;
		MusicPlaylist packageMusicPlaylist = GetPackageMusicPlaylist(playlistID);
		if (packageMusicPlaylist != null)
		{
			result = packageMusicPlaylist.soundtrack;
		}
		return result;
	}
}
