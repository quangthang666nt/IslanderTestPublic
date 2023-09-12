using System.Collections;
using System.Collections.Generic;
using FlatBuffers;
using Islanders;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CosmeticsManager : MonoBehaviour
{
	private class CosmeticLoaded
	{
		public int count;

		public bool asyncDone;

		public AsyncOperationHandle<GameObject> handle;

		public UnityEvent<GameObject> OnLoaded = new UnityEvent<GameObject>();
	}

	private static string FILE_EXTENSION = ".cosm";

	private static string FILE_NAME = "Cosmetics";

	private static int SAVE_DATA_SLOT = 1;

	public static Cosmetics Cosmetics = new Cosmetics();

	public static UnityEvent OnCosmeticsUpdate = new UnityEvent();

	public static CosmeticsManager singleton;

	[HideInInspector]
	public CatalogConfig CatalogConfig;

	[HideInInspector]
	public bool bDataLoaded;

	[HideInInspector]
	public Dictionary<string, Material> dicMaterialsRef;

	private Dictionary<string, CosmeticLoaded> buildingCosmeticsLoaded = new Dictionary<string, CosmeticLoaded>();

	private void Awake()
	{
		singleton = this;
		StartCoroutine(LoadCatalogConfig());
	}

	public static void LaunchComesticsUpdate()
	{
		OnCosmeticsUpdate?.Invoke();
	}

	public static bool HasPlayerTheme()
	{
		if (singleton == null)
		{
			return false;
		}
		return !string.IsNullOrEmpty(Cosmetics.theme);
	}

	public static void Save()
	{
		FlatBufferBuilder flatBufferBuilder = SaveLoadManager.singleton.m_FlatBufferBuilder;
		flatBufferBuilder.Clear();
		Offset<Islanders.Cosmetics> offset = Cosmetics.ToFlatBuffer(flatBufferBuilder);
		Islanders.Cosmetics.FinishCosmeticsBuffer(flatBufferBuilder, offset);
		byte[] data = flatBufferBuilder.DataBuffer.ToSizedArray();
		PlatformPlayerManagerSystem.Instance.SaveData(StrSaveFileNameToFullPath(FILE_NAME), ref data, OnCosmeticsSaved, SAVE_DATA_SLOT);
	}

	private static void OnCosmeticsSaved(string fileName, bool result)
	{
		if (result)
		{
			Debug.Log("[CosmeticsManager] Saved " + fileName + " successfully!");
		}
		else
		{
			Debug.Log("[CosmeticsManager] Failed to saved " + fileName + "!");
		}
	}

	public static void Load()
	{
		singleton.bDataLoaded = false;
		Cosmetics.CleanAllCosmetics();
		PlatformPlayerManagerSystem.Instance.LoadData(StrSaveFileNameToFullPath(FILE_NAME), OnCosmeticsLoaded, SAVE_DATA_SLOT);
	}

	private static void OnCosmeticsLoaded(string fileName, LoadResult result, byte[] data)
	{
		if (result == LoadResult.Success)
		{
			ByteBuffer bb = new ByteBuffer(data);
			if (!Islanders.Cosmetics.CosmeticsBufferHasIdentifier(bb))
			{
				Debug.LogWarning("[CosmeticsManager] Couldn't find identifier in cosmetics buffer!");
			}
			else
			{
				Islanders.Cosmetics rootAsCosmetics = Islanders.Cosmetics.GetRootAsCosmetics(bb);
				Cosmetics.FromFlatBuffer(rootAsCosmetics);
			}
		}
		singleton.StartCoroutine(singleton.SetupCurrentCosmetics());
	}

	private IEnumerator SetupCurrentCosmetics()
	{
		while (!IsConfigLoaded())
		{
			yield return null;
		}
		bool flag = CheckCurrentTheme();
		bool flag2 = CheckCurrentCosmetics();
		bool flag3 = CheckCurrentPlaylists();
		if (flag || flag2 || flag3)
		{
			Save();
		}
		while (ColorGenerator.singleton == null)
		{
			yield return null;
		}
		LoadMaterialReferences();
		singleton.bDataLoaded = true;
	}

	private void LoadMaterialReferences()
	{
		dicMaterialsRef = new Dictionary<string, Material>();
		if (ColorGenerator.singleton.colorSetups == null)
		{
			return;
		}
		for (int i = 0; i < ColorGenerator.singleton.colorSetups.Length; i++)
		{
			ColorSetup colorSetup = ColorGenerator.singleton.colorSetups[i];
			if (colorSetup.materialSetups != null)
			{
				for (int j = 0; j < colorSetup.materialSetups.Count; j++)
				{
					AddMaterialSetupToDicRefs(colorSetup.materialSetups[j]);
				}
			}
		}
	}

	private void AddMaterialSetupToDicRefs(ColorSetup.MaterialSetup materialSetup)
	{
		AddMaterialToDicRefs(materialSetup.targetMaterial);
		if (materialSetup.additionalTargets != null)
		{
			for (int i = 0; i < materialSetup.additionalTargets.Length; i++)
			{
				AddMaterialToDicRefs(materialSetup.additionalTargets[i]);
			}
		}
	}

	private void AddMaterialToDicRefs(Material material)
	{
		if (dicMaterialsRef != null && !(material == null) && !dicMaterialsRef.ContainsKey(material.name))
		{
			dicMaterialsRef.Add(material.name, material);
		}
	}

	public bool MainThemeEventAvailable()
	{
		if (!string.IsNullOrEmpty(CatalogConfig.mainPackage))
		{
			return IsValidThemeEvent(CatalogConfig.mainPackage);
		}
		return false;
	}

	private bool CheckCurrentTheme()
	{
		bool result = false;
		if (MainThemeEventAvailable())
		{
			bool num = string.IsNullOrEmpty(Cosmetics.theme) || !Cosmetics.theme.Equals(CatalogConfig.mainPackage);
			bool flag = string.IsNullOrEmpty(Cosmetics.asked) || !Cosmetics.asked.Equals(CatalogConfig.mainPackage);
			if (num && flag)
			{
				Cosmetics.ask = true;
				result = true;
			}
		}
		else if (Cosmetics.ask)
		{
			Cosmetics.ask = false;
			result = true;
		}
		if (!string.IsNullOrEmpty(Cosmetics.theme) && !IsValidThemeEvent(Cosmetics.theme))
		{
			if (Cosmetics.entries != null && Cosmetics.entries.Count > 0)
			{
				Cosmetics.entries.Clear();
			}
			Cosmetics.theme = "";
			result = true;
		}
		return result;
	}

	private bool CheckCurrentPlaylists()
	{
		bool result = false;
		List<string> list = new List<string>();
		for (int i = 0; i < Cosmetics.playlists.Count; i++)
		{
			if (!AudioManager.singleton.IsOriginalPlaylist(Cosmetics.playlists[i]) && !IsValidThemeEvent(Cosmetics.playlists[i]) && !IsValidExtraPlaylist(Cosmetics.playlists[i]))
			{
				list.Add(Cosmetics.playlists[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			result = true;
			Cosmetics.playlists.Remove(list[j]);
		}
		return result;
	}

	private bool CheckCurrentCosmetics()
	{
		bool result = false;
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, string> entry in Cosmetics.entries)
		{
			if (!CosmeticIsAvailable(entry.Value))
			{
				Debug.LogFormat("Cosmetic {0} is no available anymore", entry.Value);
				list.Add(entry.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			Cosmetics.entries.Remove(list[i]);
			result = true;
		}
		return result;
	}

	public bool IsConfigLoaded()
	{
		return CatalogConfig != null;
	}

	private IEnumerator LoadCatalogConfig()
	{
		AsyncOperationHandle<CatalogConfig> opHandle = Addressables.LoadAssetAsync<CatalogConfig>(CatalogConfig.CATALOG_CONFIG_LABEL);
		while (!opHandle.IsDone)
		{
			yield return null;
		}
		CatalogConfig = opHandle.Result;
	}

	private bool CosmeticIsAvailable(string cosmetic)
	{
		if (!IsConfigLoaded() || CatalogConfig.availablePackages == null || CatalogConfig.availablePackages.Count == 0 || string.IsNullOrEmpty(cosmetic))
		{
			return false;
		}
		string[] array = cosmetic.Split(CatalogHelper.PACKAGE_SEPARATOR);
		if (array == null || array.Length == 0)
		{
			return false;
		}
		return CatalogConfig.availablePackages.Contains(array[0]);
	}

	private bool CosmeticBelongToMainPackage(string cosmetic)
	{
		if (!IsConfigLoaded() || string.IsNullOrEmpty(cosmetic) || string.IsNullOrEmpty(CatalogConfig.mainPackage))
		{
			return false;
		}
		string[] array = cosmetic.Split(CatalogHelper.PACKAGE_SEPARATOR);
		if (array == null || array.Length == 0)
		{
			return false;
		}
		return array[0].Equals(CatalogConfig.mainPackage);
	}

	private bool IsValidThemeEvent(string theme)
	{
		if (!IsConfigLoaded() || CatalogConfig.availablePackages == null || CatalogConfig.availablePackages.Count == 0 || string.IsNullOrEmpty(theme))
		{
			return false;
		}
		return CatalogConfig.availablePackages.Contains(theme);
	}

	private bool IsValidExtraPlaylist(string playlistID)
	{
		if (!IsConfigLoaded())
		{
			return false;
		}
		return CatalogConfig.IsExtraPlaylistAvailable(playlistID);
	}

	public void LoadBuildingCosmeticAsync(string key, UnityAction<GameObject> OnLoaded)
	{
		CosmeticLoaded cosmeticLoaded;
		if (buildingCosmeticsLoaded.TryGetValue(key, out var value))
		{
			cosmeticLoaded = value;
			if (cosmeticLoaded.asyncDone)
			{
				OnLoaded?.Invoke(cosmeticLoaded.handle.Result);
			}
			else
			{
				cosmeticLoaded.OnLoaded.AddListener(OnLoaded);
			}
		}
		else
		{
			cosmeticLoaded = new CosmeticLoaded();
			buildingCosmeticsLoaded.Add(key, cosmeticLoaded);
			cosmeticLoaded.OnLoaded.AddListener(OnLoaded);
			cosmeticLoaded.handle = Addressables.LoadAssetAsync<GameObject>(key);
			cosmeticLoaded.handle.Completed += delegate(AsyncOperationHandle<GameObject> opHandle)
			{
				CosmeticLoaded cosmeticLoaded2 = null;
				if (buildingCosmeticsLoaded.TryGetValue(key, out var value2))
				{
					cosmeticLoaded2 = value2;
					if (opHandle.Status != AsyncOperationStatus.Succeeded)
					{
						Debug.LogErrorFormat("Error loading cosmetic handler {0} with key {1}. Status {2}.", base.transform.parent.name, key, opHandle.Status.ToString());
					}
					cosmeticLoaded2.asyncDone = true;
					cosmeticLoaded2.OnLoaded?.Invoke(cosmeticLoaded2.handle.Result);
				}
				else
				{
					Debug.LogErrorFormat("CosmeticLoaded ref missing for key {0}", key);
				}
			};
		}
		cosmeticLoaded.count++;
	}

	public void UnloadBuildingCosmeticIfApply(string key)
	{
		if (buildingCosmeticsLoaded.TryGetValue(key, out var value))
		{
			value.count--;
			if (value.count <= 0)
			{
				buildingCosmeticsLoaded.Remove(key);
				Addressables.Release(value.handle);
			}
		}
	}

	public static string StrSaveFileNameToFullPath(string _strName)
	{
		return _strName + FILE_EXTENSION;
	}
}
