using System;
using System.Collections.Generic;
using FlatBuffers;
using I2.Loc;
using Islanders;
using UnityEngine;

public class SandboxGenerator : MonoBehaviour
{
	public enum Weather
	{
		Clear = 0,
		Rain = 1,
		Snow = 2
	}

	[Serializable]
	public class AdvancedOptions
	{
		public int flowerWeight;

		public int treeWeight;

		public Weather Weather;

		public int weatherWeight;
	}

	private static string FILE_EXTENSION = ".conf";

	private static string FILE_NAME = "Sandbox";

	private static int SAVE_DATA_SLOT = 1;

	private const float COLORS_MAX_PROB = 10f;

	[Header("Setup")]
	public List<IslandCollection> allTypes;

	public List<BiomeColors> allBiomes;

	public List<IslandCollection> allSizes;

	public LocalizedString weatherClear;

	public LocalizedString weatherRain;

	public LocalizedString weatherSnow;

	public SandboxGenerationView sandboxGenerationView;

	[Header("Generation")]
	public GameObject rTreesGroups;

	public GameObject rFlowersGroups;

	public GameObject rGold;

	public GameObject rClearResourcesTower;

	public SandboxConfiguration sandboxDefaultConfig;

	public List<BiomeColors> biomesWithIceFloes;

	public float maxRainIntensity = 700f;

	public float maxSnowIntensity = 700f;

	public int defaultMaxTreesGroupsValue = 20;

	public int defaultMaxFlowersGroupsValue = 20;

	public GameObject errorDefaultGenerateIsland;

	private List<UnityEngine.Object> instancesToDestroy = new List<UnityEngine.Object>();

	public static SandboxGenerator singleton;

	public static SandboxConfig SandboxConfig = new SandboxConfig();

	[HideInInspector]
	public bool bDataLoaded;

	private void Awake()
	{
		singleton = this;
	}

	public static void Save()
	{
		FlatBufferBuilder flatBufferBuilder = SaveLoadManager.singleton.m_FlatBufferBuilder;
		flatBufferBuilder.Clear();
		Offset<Islanders.SandboxConfig> offset = SandboxConfig.ToFlatBuffer(flatBufferBuilder);
		Islanders.SandboxConfig.FinishSandboxConfigBuffer(flatBufferBuilder, offset);
		byte[] data = flatBufferBuilder.DataBuffer.ToSizedArray();
		PlatformPlayerManagerSystem.Instance.SaveData(StrSaveFileNameToFullPath(FILE_NAME), ref data, OnSandboxConfigSaved, SAVE_DATA_SLOT);
	}

	private static void OnSandboxConfigSaved(string fileName, bool result)
	{
		if (result)
		{
			Debug.Log("[SandboxGenerator] Saved " + fileName + " successfully!");
		}
		else
		{
			Debug.Log("[SandboxGenerator] Failed to saved " + fileName + "!");
		}
	}

	public static void Load()
	{
		singleton.bDataLoaded = false;
		PlatformPlayerManagerSystem.Instance.LoadData(StrSaveFileNameToFullPath(FILE_NAME), OnSandboxConfigLoaded, SAVE_DATA_SLOT);
	}

	private static void OnSandboxConfigLoaded(string fileName, LoadResult result, byte[] data)
	{
		if (result == LoadResult.Success)
		{
			ByteBuffer bb = new ByteBuffer(data);
			if (!Islanders.SandboxConfig.SandboxConfigBufferHasIdentifier(bb))
			{
				Debug.LogWarning("[SandboxGenerator] Couldn't find identifier in sandbox config buffer!");
			}
			else
			{
				Islanders.SandboxConfig rootAsSandboxConfig = Islanders.SandboxConfig.GetRootAsSandboxConfig(bb);
				SandboxConfig.FromFlatBuffer(rootAsSandboxConfig);
			}
		}
		else
		{
			SandboxConfig = new SandboxConfig();
		}
		singleton.bDataLoaded = true;
	}

	public static void ClearSaveData(bool matchTutorialDone = false, ushort matchBuildsPlaced = 0)
	{
		bool globalTutorialDone = SandboxConfig.globalTutorialDone;
		SandboxConfig = new SandboxConfig();
		SandboxConfig.matchTutorialDone = matchTutorialDone;
		SandboxConfig.matchBuildsPlaced = matchBuildsPlaced;
		SandboxConfig.globalTutorialDone = globalTutorialDone;
		Save();
	}

	public void SetDefaultSandboxGenerationView()
	{
		sandboxGenerationView.SetOriginalValues(sandboxDefaultConfig, fromSaveLoad: false);
	}

	public void SetData(IslandCollection[] types, BiomeColors biome, IslandCollection size, AdvancedOptions advanced)
	{
		SandboxConfig.types = GetIslandsIds(types);
		SandboxConfig.biome = biome.id;
		SandboxConfig.size = size.id;
		SandboxConfig.treeWeight = (ushort)advanced.treeWeight;
		SandboxConfig.flowerWeight = (ushort)advanced.flowerWeight;
		SandboxConfig.weather = (ushort)advanced.Weather;
		SandboxConfig.weatherWeight = (ushort)advanced.weatherWeight;
		SandboxConfig.playerData = false;
	}

	public GameObject GenerateIslandFromExistingData()
	{
		IslandCollection[] islandsById = GetIslandsById(SandboxConfig.types, allTypes);
		BiomeColors biome = ((SandboxConfig.biome != 0) ? GetBiomeById(SandboxConfig.biome) : sandboxDefaultConfig.biome);
		IslandCollection size;
		if (SandboxConfig.size == 0)
		{
			size = sandboxDefaultConfig.size;
		}
		else
		{
			IslandCollection[] islandsById2 = GetIslandsById(new ushort[1] { SandboxConfig.size }, allSizes);
			size = ((islandsById2.Length != 0) ? islandsById2[0] : null);
		}
		AdvancedOptions advanced = new AdvancedOptions
		{
			flowerWeight = SandboxConfig.flowerWeight,
			treeWeight = SandboxConfig.treeWeight,
			Weather = (Weather)SandboxConfig.weather,
			weatherWeight = SandboxConfig.weatherWeight
		};
		ushort selectedType = 0;
		int? selectedIndex = null;
		bool persistData = true;
		if (SandboxConfig.playerData)
		{
			selectedType = SandboxConfig.selectedType;
			selectedIndex = SandboxConfig.selectedIndex;
			persistData = false;
		}
		return GenerateIsland(islandsById, biome, size, advanced, selectedType, selectedIndex, persistData);
	}

	private GameObject GenerateIsland(IslandCollection[] types, BiomeColors biome, IslandCollection size, AdvancedOptions advanced, ushort selectedType = 0, int? selectedIndex = null, bool persistData = true)
	{
		GameObject gameObject;
		if (selectedType != 0 && selectedIndex.HasValue)
		{
			IslandCollection[] islandsById = GetIslandsById(new ushort[1] { selectedType }, allTypes);
			Debug.Log($"Result {islandsById[0]}  index {selectedIndex.Value} prefab {islandsById[0].prefabs[selectedIndex.Value]}");
			if (islandsById.Length == 0)
			{
				Debug.LogError("No island result in current DB collection");
				CleanInstancesToDestroy();
				return UnityEngine.Object.Instantiate(errorDefaultGenerateIsland);
			}
			gameObject = islandsById[0].prefabs[selectedIndex.Value];
		}
		else
		{
			List<GameObject> list = FilterIslandWithSizeLimit(types, size);
			if (list.Count == 0)
			{
				Debug.LogError("No island result after applying filters");
				CleanInstancesToDestroy();
				return UnityEngine.Object.Instantiate(errorDefaultGenerateIsland);
			}
			gameObject = list[UnityEngine.Random.Range(0, list.Count)];
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		TerrainGenerator componentInChildren = gameObject2.GetComponentInChildren<TerrainGenerator>();
		ExpandedSandboxIslandInfo componentInChildren2 = gameObject2.GetComponentInChildren<ExpandedSandboxIslandInfo>();
		instancesToDestroy.Add(gameObject2);
		if (biome.colors.Length == 0)
		{
			Debug.LogError("No biome colors found");
			CleanInstancesToDestroy();
			return UnityEngine.Object.Instantiate(errorDefaultGenerateIsland);
		}
		ColorGenerator.OverrideColorSetup[] array = new ColorGenerator.OverrideColorSetup[biome.colors.Length];
		for (int i = 0; i < biome.colors.Length; i++)
		{
			ColorSetup colorSetup = UnityEngine.Object.Instantiate(biome.colors[i], gameObject2.transform);
			switch (advanced.Weather)
			{
			case Weather.Clear:
				colorSetup.enableRain = false;
				colorSetup.enableSnow = false;
				break;
			case Weather.Rain:
				colorSetup.enableRain = true;
				colorSetup.enableSnow = false;
				colorSetup.rainIntensity = (float)advanced.weatherWeight * maxRainIntensity / 10f;
				break;
			case Weather.Snow:
				colorSetup.enableRain = false;
				colorSetup.enableSnow = true;
				colorSetup.snowIntensity = (float)advanced.weatherWeight * maxSnowIntensity / 10f;
				break;
			}
			ColorGenerator.OverrideColorSetup overrideColorSetup = new ColorGenerator.OverrideColorSetup();
			overrideColorSetup.colorSetup = colorSetup;
			overrideColorSetup.fProbability = 10f;
			array[i] = overrideColorSetup;
		}
		componentInChildren.overrideColorSetups = array;
		if ((bool)componentInChildren2 && componentInChildren2.iceFloesRef != null)
		{
			componentInChildren2.iceFloesRef.SetActive(biomesWithIceFloes.Contains(biome));
		}
		if ((bool)componentInChildren2 && componentInChildren2.resourcesChildRef != null)
		{
			UnityEngine.Object.Destroy(componentInChildren2.resourcesChildRef);
		}
		GameObject gameObject3 = UnityEngine.Object.Instantiate(rClearResourcesTower, componentInChildren.transform);
		if (advanced.treeWeight > 0)
		{
			TO_DropAssets component = UnityEngine.Object.Instantiate(rTreesGroups, gameObject3.transform).GetComponent<TO_DropAssets>();
			if ((bool)component)
			{
				int num = (componentInChildren2 ? componentInChildren2.maxTreesGroupsValue : defaultMaxTreesGroupsValue);
				component.iMaxAmount = (component.iMinAmount = advanced.treeWeight * num / 10);
			}
		}
		if (advanced.flowerWeight > 0)
		{
			TO_NarrowWideAssetDrop component2 = UnityEngine.Object.Instantiate(rFlowersGroups, gameObject3.transform).GetComponent<TO_NarrowWideAssetDrop>();
			if ((bool)component2)
			{
				int num2 = (componentInChildren2 ? componentInChildren2.maxFlowersGroupsValue : defaultMaxFlowersGroupsValue);
				component2.iAmount = advanced.flowerWeight * num2 / 10;
			}
		}
		TO_DropAssets component3 = UnityEngine.Object.Instantiate(rGold, gameObject3.transform).GetComponent<TO_DropAssets>();
		if ((bool)component3)
		{
			component3.iMinAmount = 1;
			component3.iMaxAmount = 1;
		}
		if (persistData)
		{
			SandboxConfig.types = GetIslandsIds(types);
			SandboxConfig.biome = biome.id;
			SandboxConfig.size = size.id;
			SandboxConfig.treeWeight = (ushort)advanced.treeWeight;
			SandboxConfig.flowerWeight = (ushort)advanced.flowerWeight;
			SandboxConfig.weather = (ushort)advanced.Weather;
			SandboxConfig.weatherWeight = (ushort)advanced.weatherWeight;
			SetSelectedIslandTypeAndIndex(gameObject);
			SandboxConfig.playerData = true;
			SandboxConfig.matchBuildsPlaced = 0;
			SandboxConfig.matchTutorialDone = false;
			Save();
		}
		return gameObject2;
	}

	public void LoadSandboxGenerationDataFromArchive(ArchiveIsland island)
	{
		if (!island.playerData)
		{
			ClearSaveData(island.matchTutorialDone, island.matchBuildsPlaced);
			return;
		}
		SandboxConfig.types = island.types;
		SandboxConfig.biome = island.biome;
		SandboxConfig.size = island.size;
		SandboxConfig.treeWeight = island.treeWeight;
		SandboxConfig.flowerWeight = island.flowerWeight;
		SandboxConfig.weather = island.weather;
		SandboxConfig.weatherWeight = island.weatherWeight;
		SandboxConfig.selectedType = island.selectedType;
		SandboxConfig.selectedIndex = island.selectedIndex;
		SandboxConfig.playerData = true;
		SandboxConfig.matchTutorialDone = island.matchTutorialDone;
		SandboxConfig.matchBuildsPlaced = island.matchBuildsPlaced;
		Save();
	}

	private void SetSelectedIslandTypeAndIndex(GameObject islandPrefab)
	{
		ushort num = 0;
		ushort num2 = 0;
		for (int i = 0; i < allTypes.Count; i++)
		{
			for (int j = 0; j < allTypes[i].prefabs.Length; j++)
			{
				if (allTypes[i].prefabs[j] == islandPrefab)
				{
					num = allTypes[i].id;
					num2 = (ushort)j;
					break;
				}
			}
			if (num != 0)
			{
				break;
			}
		}
		Debug.Log($"Selected island {num} index {num2}");
		SandboxConfig.selectedType = num;
		SandboxConfig.selectedIndex = num2;
	}

	private List<GameObject> FilterIslandWithSizeLimit(IslandCollection[] types, IslandCollection size)
	{
		if (types.Length == 0)
		{
			types = allTypes.ToArray();
		}
		List<GameObject> list = new List<GameObject>();
		list.AddRange(size.prefabs);
		List<GameObject> list2 = new List<GameObject>();
		for (int i = 0; i < types.Length; i++)
		{
			for (int j = 0; j < types[i].prefabs.Length; j++)
			{
				GameObject item = types[i].prefabs[j];
				if (list.Contains(item))
				{
					list2.Add(item);
				}
			}
		}
		return list2;
	}

	private void CleanInstancesToDestroy()
	{
		for (int i = 0; i < instancesToDestroy.Count; i++)
		{
			UnityEngine.Object.Destroy(instancesToDestroy[i]);
		}
		instancesToDestroy.Clear();
	}

	public IslandCollection[] GetIslandTypesByIdsFromSave()
	{
		if (!SandboxConfig.playerData)
		{
			return sandboxDefaultConfig.types;
		}
		return GetIslandsById(SandboxConfig.types, allTypes);
	}

	public IslandCollection GetIslandSizeFromSave()
	{
		if (!SandboxConfig.playerData)
		{
			return sandboxDefaultConfig.size;
		}
		IslandCollection[] islandsById = GetIslandsById(new ushort[1] { SandboxConfig.size }, allSizes);
		if (islandsById.Length == 0)
		{
			return null;
		}
		return islandsById[0];
	}

	private IslandCollection[] GetIslandsById(ushort[] ids, List<IslandCollection> collection)
	{
		List<IslandCollection> list = new List<IslandCollection>();
		for (int i = 0; i < ids.Length; i++)
		{
			for (int j = 0; j < collection.Count; j++)
			{
				if (collection[j].id.Equals(ids[i]))
				{
					list.Add(collection[j]);
					break;
				}
			}
		}
		return list.ToArray();
	}

	private ushort[] GetIslandsIds(IslandCollection[] islands)
	{
		List<ushort> list = new List<ushort>();
		for (int i = 0; i < islands.Length; i++)
		{
			list.Add(islands[i].id);
		}
		return list.ToArray();
	}

	private BiomeColors GetBiomeById(ushort id)
	{
		BiomeColors result = null;
		for (int i = 0; i < allBiomes.Count; i++)
		{
			if (allBiomes[i].id.Equals(id))
			{
				result = allBiomes[i];
				break;
			}
		}
		return result;
	}

	public BiomeColors GetBiomeFromSave()
	{
		if (!SandboxConfig.playerData)
		{
			return sandboxDefaultConfig.biome;
		}
		return GetBiomeById(SandboxConfig.biome);
	}

	public AdvancedOptions GetAdvancedOptionsFromSave()
	{
		if (!SandboxConfig.playerData)
		{
			return sandboxDefaultConfig.advanced;
		}
		return new AdvancedOptions
		{
			flowerWeight = SandboxConfig.flowerWeight,
			treeWeight = SandboxConfig.treeWeight,
			Weather = (Weather)SandboxConfig.weather,
			weatherWeight = SandboxConfig.weatherWeight
		};
	}

	public string GetWeatherLocalized(Weather weather)
	{
		string result = string.Empty;
		switch (weather)
		{
		case Weather.Clear:
			result = weatherClear;
			break;
		case Weather.Rain:
			result = weatherRain;
			break;
		case Weather.Snow:
			result = weatherSnow;
			break;
		}
		return result;
	}

	public string GetIslandTypeLocalized(ushort id)
	{
		string result = string.Empty;
		IslandCollection[] islandsById = GetIslandsById(new ushort[1] { id }, allTypes);
		if (islandsById.Length != 0)
		{
			result = islandsById[0].name;
		}
		return result;
	}

	public string GetIslandBiomeLocalized(ushort id)
	{
		string result = string.Empty;
		BiomeColors biomeById = GetBiomeById(id);
		if ((bool)biomeById)
		{
			result = biomeById.name;
		}
		return result;
	}

	public string GetIslandSizeLocalized(ushort id)
	{
		string result = string.Empty;
		IslandCollection[] islandsById = GetIslandsById(new ushort[1] { id }, allSizes);
		if (islandsById.Length != 0)
		{
			result = islandsById[0].name;
		}
		return result;
	}

	public static string StrSaveFileNameToFullPath(string _strName)
	{
		return _strName + FILE_EXTENSION;
	}
}
