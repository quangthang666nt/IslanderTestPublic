using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FlatBuffers;
using I2.Loc;
using Islanders;
using UnityEngine;

public class ArchiveManager : MonoBehaviour
{
	public struct ArchiveIslandData
	{
		public ushort id;

		public string slot;

		public string name;

		public string datetime;

		public string screenshot;
	}

	private static string FILE_EXTENSION = ".aris";

	private static string FILE_NAME = "Archive";

	private static string GALLERY_FILE_EXTENSION = ".gallery";

	private static int SAVE_DATA_SLOT = 1;

	private static ushort SANDBOX_START_ID = 1;

	private static string SANDBOX_PATH = "archive";

	private const string DATE_SEPARATOR = "#";

	private const string DATE_FORMAT = "yyyy#MM#dd#HH#mm#ss#fff";

	public static ArchiveManager singleton;

	public static Archive Archive = new Archive();

	public FlatBufferBuilder m_ImgGalleryFlatBufferBuilder = new FlatBufferBuilder(5242880);

	[Header("Configuration")]
	public int maxSlots = 20;

	[SerializeField]
	private LocalizedString defaultIslandName;

	public int maxIslandNameLength = 25;

	public List<string> lLanguagesWithValidation = new List<string>();

	[Header("Save File Names")]
	[Tooltip("The name of the sandbox save file")]
	public string strSandboxSaveGameName = "SandboxV1";

	[Header("ScreenShot")]
	[SerializeField]
	private int iSsWidthRes = 16;

	[SerializeField]
	private int iSsHeightRes = 9;

	[SerializeField]
	private int iSsWidth = 100;

	[SerializeField]
	private int iLowSsWidth = 100;

	[SerializeField]
	private bool bSsIsTransparent;

	[HideInInspector]
	public bool bDataLoaded;

	private bool bIsSaving;

	public bool bWaitingScreenshot;

	public bool bScreenshotBeingDeleted;

	private void Awake()
	{
		singleton = this;
	}

	public static void Save()
	{
		singleton.bIsSaving = true;
		StatsManager.statsMatch.OnSavedArchive();
		FlatBufferBuilder flatBufferBuilder = SaveLoadManager.singleton.m_FlatBufferBuilder;
		flatBufferBuilder.Clear();
		Offset<Islanders.Archive> offset = Archive.ToFlatBuffer(flatBufferBuilder);
		Islanders.Archive.FinishArchiveBuffer(flatBufferBuilder, offset);
		byte[] data = flatBufferBuilder.DataBuffer.ToSizedArray();
		PlatformPlayerManagerSystem.Instance.SaveData(StrSaveFileNameToFullPath(FILE_NAME), ref data, OnArchiveSaved, SAVE_DATA_SLOT);
	}

	private static void OnArchiveSaved(string fileName, bool result)
	{
		if (result)
		{
			Debug.Log("[ArchiveManager] Saved " + fileName + " successfully!");
		}
		else
		{
			Debug.Log("[ArchiveManager] Failed to saved " + fileName + "!");
		}
		singleton.bIsSaving = false;
	}

	public static void Load()
	{
		singleton.bDataLoaded = false;
		PlatformPlayerManagerSystem.Instance.LoadData(StrSaveFileNameToFullPath(FILE_NAME), OnArchiveLoaded, SAVE_DATA_SLOT);
	}

	private static void OnArchiveLoaded(string fileName, LoadResult result, byte[] data)
	{
		if (result == LoadResult.Success)
		{
			ByteBuffer bb = new ByteBuffer(data);
			if (!Islanders.Archive.ArchiveBufferHasIdentifier(bb))
			{
				Debug.LogWarning("[CosmeticsManager] Couldn't find identifier in archive buffer!");
			}
			else
			{
				Islanders.Archive rootAsArchive = Islanders.Archive.GetRootAsArchive(bb);
				Archive.FromFlatBuffer(rootAsArchive);
				Debug.Log("[ArchiveManager] Loaded " + fileName + " successfully!");
			}
		}
		else
		{
			Archive = new Archive();
		}
		singleton.bDataLoaded = true;
	}

	public static void SaveIslandGallery(ushort islandId, IslandGallery islandGallery)
	{
		FlatBufferBuilder imgGalleryFlatBufferBuilder = singleton.m_ImgGalleryFlatBufferBuilder;
		imgGalleryFlatBufferBuilder.Clear();
		Offset<Islanders.IslandGallery> offset = islandGallery.ToFlatBuffer(imgGalleryFlatBufferBuilder);
		Islanders.IslandGallery.FinishIslandGalleryBuffer(imgGalleryFlatBufferBuilder, offset);
		byte[] data = imgGalleryFlatBufferBuilder.DataBuffer.ToSizedArray();
		PlatformPlayerManagerSystem.Instance.SaveData(GetIslandGalleryFullName(islandId), ref data, OnIslandGallerySaved, SAVE_DATA_SLOT);
	}

	private static void OnIslandGallerySaved(string fileName, bool result)
	{
		if (result)
		{
			Debug.Log("[ArchiveManager] Saved " + fileName + " successfully!");
		}
		else
		{
			Debug.Log("[ArchiveManager] Failed to saved " + fileName + "!");
		}
		singleton.bWaitingScreenshot = false;
	}

	public static void LoadIslandGallery(ushort islandId, Action<string, LoadResult, byte[]> loadCallback)
	{
		PlatformPlayerManagerSystem.Instance.LoadData(GetIslandGalleryFullName(islandId), loadCallback, SAVE_DATA_SLOT);
	}

	public static void TakeScreenShot(ushort islandId)
	{
		IslandGallery islandGallery = new IslandGallery();
		Texture2D texture2D = TakeScreenShot(singleton.iSsWidth, GetSsHeight());
		islandGallery.highres = texture2D.EncodeToPNG();
		UnityEngine.Object.Destroy(texture2D);
		Texture2D texture2D2 = TakeScreenShot(singleton.iLowSsWidth, GetLowSsHeight());
		islandGallery.lowres = texture2D2.EncodeToPNG();
		UnityEngine.Object.Destroy(texture2D2);
		singleton.bWaitingScreenshot = true;
		SaveIslandGallery(islandId, islandGallery);
	}

	private static Texture2D TakeScreenShot(int width, int height)
	{
		Camera main = Camera.main;
		TextureFormat textureFormat = ((!singleton.bSsIsTransparent) ? TextureFormat.RGB24 : TextureFormat.ARGB32);
		RenderTexture renderTexture = new RenderTexture(width, height, 24);
		RenderTexture targetTexture = main.targetTexture;
		main.targetTexture = renderTexture;
		main.Render();
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = renderTexture;
		Texture2D texture2D = new Texture2D(width, height, textureFormat, mipChain: false);
		texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		main.targetTexture = targetTexture;
		RenderTexture.active = active;
		return texture2D;
	}

	public static int GetSsHeight()
	{
		return singleton.iSsWidth * singleton.iSsHeightRes / singleton.iSsWidthRes;
	}

	public static int GetLowSsWidth()
	{
		return singleton.iLowSsWidth;
	}

	public static int GetLowSsHeight()
	{
		return singleton.iLowSsWidth * singleton.iSsHeightRes / singleton.iSsWidthRes;
	}

	public static int GetSsWidth()
	{
		return singleton.iSsWidth;
	}

	public static string GetIslandGalleryFullName(ushort islandId)
	{
		return Path.Combine(SANDBOX_PATH, $"{singleton.GetSaveGameName(islandId)}{GALLERY_FILE_EXTENSION}");
	}

	public ushort GetSandboxAvailableId()
	{
		ushort num = SANDBOX_START_ID;
		while (Archive.sandbox.ContainsKey(num))
		{
			num++;
		}
		return num;
	}

	public static ArchiveIslandData GetArchiveIslandData(ushort id)
	{
		ArchiveIslandData result = default(ArchiveIslandData);
		if (Archive.sandbox.TryGetValue(id, out var value))
		{
			result.id = value.id;
			result.name = value.name;
			result.screenshot = value.screenshot;
			result.slot = value.slot;
			result.datetime = value.datetime;
		}
		return result;
	}

	public static void SaveEntry(ushort? forceId = null, string islandName = null, string datetime = null)
	{
		SaveLoadManager.PerformSaveExternal(singleton.CreateNewEntry(forceId, islandName, datetime).slot, SAVE_DATA_SLOT);
	}

	public static void LoadEntry(ushort id)
	{
		ArchiveIsland archiveIsland = singleton.GetArchiveIsland(id);
		if (archiveIsland == null)
		{
			Debug.LogErrorFormat("Island Archive {0} not found", id);
			return;
		}
		SandboxGenerator.singleton.LoadSandboxGenerationDataFromArchive(archiveIsland);
		LocalGameManager.singleton.NewGameFromArchive(archiveIsland.slot, SAVE_DATA_SLOT);
		Archive.current = archiveIsland.id;
		Archive.currentUpdated = true;
		Save();
	}

	public static void SetCurrentStickyIfAvailable()
	{
		if (HasCurrent() && Archive.currentUpdated)
		{
			Archive.currentUpdated = false;
			Save();
		}
	}

	public static void DeleteEntry(ushort id)
	{
		ArchiveIsland archiveIsland = singleton.GetArchiveIsland(id);
		if (archiveIsland == null)
		{
			Debug.LogErrorFormat("Island Archive {0} not found", id);
			return;
		}
		if (!string.IsNullOrEmpty(archiveIsland.screenshot))
		{
			singleton.bScreenshotBeingDeleted = true;
			PlatformPlayerManagerSystem.Instance.DeleteData(archiveIsland.screenshot, singleton.OnScreenshootDeleted, SAVE_DATA_SLOT);
		}
		if (!string.IsNullOrEmpty(archiveIsland.slot))
		{
			PlatformPlayerManagerSystem.Instance.DeleteData(archiveIsland.slot, singleton.OnSaveDataDeleted, SAVE_DATA_SLOT);
		}
		Archive.sandbox.Remove(id);
		if (Archive.current == id)
		{
			CleanCurrent(save: false);
		}
		Save();
	}

	public bool MaxFileReached()
	{
		return Archive.sandbox.Count >= maxSlots;
	}

	public void TrySaveIsland()
	{
		if (MaxFileReached())
		{
			UiCanvasManager.Singleton.ToArchiveIslandMaxFile();
			return;
		}
		TakeScreenShot(GetSandboxAvailableId());
		UiCanvasManager.Singleton.ToArchiveIslandSavePrompt();
	}

	public static bool HasCurrent()
	{
		return Archive.current != 0;
	}

	public static void CleanCurrent(bool save = true)
	{
		if (Archive.current != 0 || Archive.currentUpdated)
		{
			Archive.current = 0;
			Archive.currentUpdated = false;
			if (save)
			{
				Save();
			}
		}
	}

	public string GetSaveGameName(ushort id)
	{
		return $"{strSandboxSaveGameName}-{id}";
	}

	private ArchiveIsland CreateNewEntry(ushort? forceId = null, string islandName = null, string datetime = null)
	{
		ushort num = ((forceId.HasValue && forceId.Value != 0) ? forceId.Value : GetSandboxAvailableId());
		ArchiveIsland archiveIsland;
		if (Archive.sandbox.ContainsKey(num))
		{
			archiveIsland = Archive.sandbox[num];
		}
		else
		{
			archiveIsland = new ArchiveIsland();
			archiveIsland.id = num;
		}
		archiveIsland.name = ((!string.IsNullOrEmpty(islandName)) ? islandName : GetIslandDefaultName(archiveIsland.id));
		archiveIsland.slot = Path.Combine(SANDBOX_PATH, GetSaveGameName(archiveIsland.id) + SaveLoadManager.GetFileExtension());
		archiveIsland.datetime = ((!string.IsNullOrEmpty(datetime)) ? datetime : GetCurrentDateTime());
		string islandGalleryFullName = GetIslandGalleryFullName(archiveIsland.id);
		if (!string.IsNullOrEmpty(archiveIsland.screenshot) && !archiveIsland.screenshot.Equals(islandGalleryFullName))
		{
			PlatformPlayerManagerSystem.Instance.DeleteData(archiveIsland.screenshot, singleton.OnScreenshootDeleted, SAVE_DATA_SLOT);
		}
		archiveIsland.screenshot = islandGalleryFullName;
		SetSandboxGenerationData(archiveIsland);
		AddEntryOrReplaceIfExists(Archive.sandbox, archiveIsland);
		Archive.current = archiveIsland.id;
		Archive.currentUpdated = true;
		Save();
		return archiveIsland;
	}

	private void SetSandboxGenerationData(ArchiveIsland island)
	{
		island.matchTutorialDone = SandboxGenerator.SandboxConfig.matchTutorialDone;
		island.matchBuildsPlaced = SandboxGenerator.SandboxConfig.matchBuildsPlaced;
		if (SandboxGenerator.SandboxConfig.playerData)
		{
			island.types = SandboxGenerator.SandboxConfig.types;
			island.biome = SandboxGenerator.SandboxConfig.biome;
			island.size = SandboxGenerator.SandboxConfig.size;
			island.flowerWeight = SandboxGenerator.SandboxConfig.flowerWeight;
			island.treeWeight = SandboxGenerator.SandboxConfig.treeWeight;
			island.weather = SandboxGenerator.SandboxConfig.weather;
			island.weatherWeight = SandboxGenerator.SandboxConfig.weatherWeight;
			island.selectedType = SandboxGenerator.SandboxConfig.selectedType;
			island.selectedIndex = SandboxGenerator.SandboxConfig.selectedIndex;
			island.playerData = true;
		}
	}

	public string GetIslandDefaultName(ushort id)
	{
		string text = defaultIslandName;
		if (id < 10)
		{
			return text + $" 0{id}";
		}
		return text + $" {id}";
	}

	private ArchiveIsland GetArchiveIsland(ushort id)
	{
		ArchiveIsland result = null;
		if (Archive.sandbox.TryGetValue(id, out var value))
		{
			result = value;
		}
		return result;
	}

	private void DeleteScreenshotDataIfExists(ushort id)
	{
		ArchiveIsland archiveIsland = GetArchiveIsland(id);
		if (archiveIsland != null && !string.IsNullOrEmpty(archiveIsland.screenshot))
		{
			PlatformPlayerManagerSystem.Instance.DeleteData(archiveIsland.screenshot, OnScreenshootDeleted, SAVE_DATA_SLOT);
		}
	}

	private void OnScreenshootDeleted(string filename, bool success)
	{
		if (success)
		{
			Debug.Log($"Screenshoot {filename} succesfully deleted");
		}
		else
		{
			Debug.LogError(string.Format("Scheenshoot {1} couldnt be deleted", filename));
		}
		singleton.bScreenshotBeingDeleted = false;
	}

	private void OnSaveDataDeleted(string filename, bool success)
	{
		if (success)
		{
			Debug.Log($"File {filename} succesfully deleted");
		}
		else
		{
			Debug.LogError(string.Format("File {1} couldnt be deleted", filename));
		}
	}

	public string GetCurrentDateTime()
	{
		return DateTime.Now.ToString("yyyy#MM#dd#HH#mm#ss#fff");
	}

	public DateTime GetDateFromData(string data)
	{
		string[] array = data.Split("#");
		if (array.Length > 2)
		{
			try
			{
				int year = int.Parse(array[0]);
				int month = int.Parse(array[1]);
				int day = int.Parse(array[2]);
				return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
			}
			catch (FormatException ex)
			{
				Debug.LogErrorFormat("Failed parsing date. {0}", ex.Message);
			}
		}
		Debug.LogErrorFormat("Failed recovering date from data: {0}. Using current date.", data);
		return DateTime.Now;
	}

	public string GetHourFromData(string data)
	{
		string result = string.Empty;
		string[] array = data.Split("#");
		if (array.Length > 4)
		{
			result = array[3] + ":" + array[4];
		}
		return result;
	}

	private void AddEntryOrReplaceIfExists(Dictionary<ushort, ArchiveIsland> collection, ArchiveIsland island)
	{
		if (collection.ContainsKey(island.id))
		{
			collection[island.id] = island;
		}
		else
		{
			collection.Add(island.id, island);
		}
	}

	private IEnumerator SaveWhenAvailable()
	{
		while (bIsSaving)
		{
			yield return null;
		}
		Save();
	}

	public static double DatetimeToMilliseconds(DateTime datetime)
	{
		return datetime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
	}

	public static DateTime MillisecondsToDatetime(double milliseconds)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);
	}

	public static string ParseDateTimeToDate(DateTime datetime)
	{
		return datetime.ToString("d");
	}

	public static string ParseDateTimeToHour(DateTime datetime)
	{
		return datetime.ToString("H:m");
	}

	public static string StrSaveFileNameToFullPath(string _strName)
	{
		return _strName + FILE_EXTENSION;
	}
}
