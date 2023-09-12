using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FlatBuffers;
using Islanders;
using UnityEngine;
using UnityEngine.Events;

public class SaveLoadManager : MonoBehaviour
{
	public enum EtransitionState
	{
		NotInTransition = 0,
		TransitionStart = 1,
		TransitionMid = 2,
		TransitionEnd = 3
	}

	public static SaveLoadManager singleton;

	[Help("This script is responsible for saving and loading the game. It communicates with the island generators and manages the cloudy transitions between islands. The Undo feature is at home here as well.", MessageType.Info)]
	[Tooltip("The animator component on the camera.")]
	public Animator animatorCamera;

	[SerializeField]
	[Tooltip("Reference to islanders logo that is shown during cloud transitions.")]
	private GameObject goIslandersLogo;

	[SerializeField]
	private GameObject m_PressButtonPrompt;

	[Tooltip("The structureIDTranslator gives every object in the game an ID so they can be properly serialized for the save / load feature")]
	public StructureIDTranslator structIDTranslator;

	[Tooltip("The cloud particles")]
	public ParticleSystem particleSystemPrewarm;

	public static List<StructureID> liStructIDRegister = new List<StructureID>();

	public static List<GameObject> liGoStructIDRegister = new List<GameObject>();

	private static string FILE_EXTENSION = ".island";

	private static string ARCHIVE_EXTENSION = ".archi";

	public static SaveGame saveGameCurrent = new SaveGame();

	private EtransitionState eTransitionStateCurrent;

	[HideInInspector]
	public UnityEvent eventOnTransitionStart = new UnityEvent();

	[HideInInspector]
	public UnityEvent eventOnTransitionMidEnd = new UnityEvent();

	[HideInInspector]
	public UnityEvent eventOnTransitionEnd = new UnityEvent();

	public static bool bInLoadTransition = false;

	[Header("Undo Feature")]
	public Building buildingLastBuiltForUndo;

	public int iBusyCoinsBeforePlacingLastBuildingForUndo;

	public int iAvailablePlusesBeforePlacingLastBuildingForUndo;

	public int iUnlockedBoosterPacksBeforePlacingLastBuildingForUndo;

	public int iScoreGoalBeforePlacingLastBuildingForUndo;

	public int iScoreBaseBeforePlacingLastBuildingForUndo;

	public List<GameObject> liGoDisabledObjectsOnLasPlacementForUndo = new List<GameObject>();

	public FlatBufferBuilder m_FlatBufferBuilder = new FlatBufferBuilder(5242880);

	[Header("Save File Names")]
	[Tooltip("The name of the main save file")]
	public string strSaveGameName = "TheSaveV02";

	[Tooltip("The name of all archived islands.")]
	public string strArchiveIslandName = "Archi";

	[Tooltip("This is added at the end of the name if it is only a backup save.")]
	public string strBackupExtension = "_Backup";

	[Tooltip("This is here for debugging only. Use this to look at the current save file.")]
	public SaveGame showSaveGameTest;

	[Header("New Save Load System")]
	[Tooltip("The old save load system was entirely seed based (generated the entire island from scratch with the same seed). The new save load system stores the position of all placed objects. This list contains all placeable objects. Their index is used as an ID to properly serialize them.")]
	public List<GameObject> liGoIslandPlaceableIDs;

	private const float c_DelayBetweenAutosaves = 60f;

	private float m_DelayBeforeNextSaveAllowed;

	private bool m_LastAutoSaveDenied;

	public bool m_IsSaving;

	private bool m_bRecovered;

	private ColorBaker colorBaker;

	private int iReturn;

	private static int iID;

	public static SaveGame SaveGameCurrent => saveGameCurrent;

	public EtransitionState ETransitionStateCurrent => eTransitionStateCurrent;

	public static bool LastAutoSaveDenied
	{
		get
		{
			return singleton.m_LastAutoSaveDenied;
		}
		private set
		{
			singleton.m_LastAutoSaveDenied = value;
			if (singleton.m_OnLastAutoSaveDeniedChange != null)
			{
				singleton.m_OnLastAutoSaveDeniedChange(value, singleton.m_IsSaving);
			}
		}
	}

	private event Action<bool, bool> m_OnLastAutoSaveDeniedChange;

	public static event Action<bool, bool> OnLastAutoSaveDeniedChange
	{
		add
		{
			singleton.m_OnLastAutoSaveDeniedChange -= value;
			singleton.m_OnLastAutoSaveDeniedChange += value;
		}
		remove
		{
			singleton.m_OnLastAutoSaveDeniedChange -= value;
		}
	}

	private void Awake()
	{
		singleton = this;
		structIDTranslator.Activate();
		liStructIDRegister.Clear();
		liGoStructIDRegister.Clear();
		SettingsManager.LoadSettings();
	}

	private IEnumerator Start()
	{
		colorBaker = ColorBaker.singleton;
		if (PlatformPlayerManagerSystem.RequiresEngagementScreen())
		{
			OpenTitleScreen(skipToMiddle: true);
			yield return new WaitForSeconds(2f);
			InputManager.EnableAllInput();
		}
		else
		{
			LocalGameManager.singleton.GameState = LocalGameManager.EGameState.PreGame;
			StartCoroutine(StartCloudTransition(withLogo: true));
			SkipToMidOfTransitionAnimation();
			yield return new WaitForSeconds(2f);
			InputManager.EnableAllInput();
			ConnectEngagedPlayer();
		}
		PlatformPlayerManagerSystem.Instance.OnUnexpectedNewPlayerConnected += OnEngagedPlayerConnected;
	}

	[CommandLine("title_screen", "", null, true)]
	public static void DebugOpenTitleScreen()
	{
		singleton.OpenTitleScreen();
	}

	public void OpenTitleScreen(bool skipToMiddle = false)
	{
		LocalGameManager.singleton.GameState = LocalGameManager.EGameState.PreGame;
		StartCoroutine(StartCloudTransition(withLogo: true, withPrompt: true));
		if (skipToMiddle)
		{
			SkipToMidOfTransitionAnimation();
		}
		UiCanvasManager.Singleton.ToTitleScreen();
	}

	public void ConnectEngagedPlayer()
	{
		UiCanvasManager.Singleton.ToSigningPopup();
		PlatformPlayerManagerSystem.Instance.OnEngagedPlayerConnected += OnEngagedPlayerConnected;
		PlatformPlayerManagerSystem.Instance.SetEngagedPlayerIndex(0);
		PlatformPlayerManagerSystem.Instance.ConnectEngagedPlayer();
	}

	private void OnEngagedPlayerConnected(PlayerConnectionResult.ResultState result)
	{
		PlatformPlayerManagerSystem.Instance.OnEngagedPlayerConnected -= OnEngagedPlayerConnected;
		if (result == PlayerConnectionResult.ResultState.Success)
		{
			UiCanvasManager.Singleton.ToTransition();
			InitialGameLoad();
			if (m_PressButtonPrompt != null)
			{
				m_PressButtonPrompt.SetActive(value: false);
			}
		}
		else
		{
			UiCanvasManager.Singleton.ToTitleScreen();
		}
	}

	public void InitialGameLoad()
	{
		SettingsManager.LoadSettings();
		StatsManager.LoadGlobalStats();
		CosmeticsManager.Load();
		SandboxGenerator.Load();
		ArchiveManager.Load();
		StartCoroutine(WaitRequirementsToLoadGame());
	}

	private IEnumerator WaitRequirementsToLoadGame()
	{
		while (!CosmeticsManager.singleton.bDataLoaded || !ArchiveManager.singleton.bDataLoaded || !SandboxGenerator.singleton.bDataLoaded)
		{
			yield return null;
		}
		LoadGameIfExistsOtherwiseNewGame();
	}

	[CommandLine("transition_animation_start", "", null, true)]
	public static void CallStartTransitionAnimation()
	{
		singleton.StartCoroutine(singleton.StartCloudTransition(withLogo: true));
	}

	[CommandLine("transition_animation_end", "", null, true)]
	public static void CallEndTransitionAnimation()
	{
		singleton.StartCoroutine(singleton.EndCloudTransition());
	}

	public void SkipToMidOfTransitionAnimation()
	{
		animatorCamera.SetTrigger("SkipToMid");
		if ((bool)particleSystemPrewarm)
		{
			particleSystemPrewarm.Simulate(5f);
			particleSystemPrewarm.Play();
		}
	}

	private void Update()
	{
		if (m_DelayBeforeNextSaveAllowed > 0f)
		{
			m_DelayBeforeNextSaveAllowed -= Time.deltaTime;
		}
		if (InputManager.Singleton.InputDataCurrent.bUndo)
		{
			Undo();
		}
	}

	private void LoadGameIfExistsOtherwiseNewGame()
	{
		PlatformPlayerManagerSystem.Instance.LoadData(StrSaveFileNameToFullPath(strSaveGameName), OnLoadCompleted);
	}

	private void OnLoadCompleted(string fileName, LoadResult result, byte[] data)
	{
		if (result == LoadResult.Success)
		{
			Load(SaveGame.ELoadMode.Normal, 0, data);
			return;
		}
		Debug.Log("[SaveLoadManager] Loading file " + fileName + " failed (" + result.ToString() + "), starting new game!");
		saveGameCurrent = new SaveGame();
		LocalGameManager.singleton.GameMode = LocalGameManager.EGameMode.Default;
		LocalGameManager.singleton.NewGame();
	}

	public static void LoadGameFromSlot(string slotPath, int slot)
	{
		PlatformPlayerManagerSystem.Instance.LoadData(slotPath, singleton.OnLoadSlotCompleted, slot);
	}

	private void OnLoadSlotCompleted(string fileName, LoadResult result, byte[] data)
	{
		if (result == LoadResult.Success)
		{
			Load(SaveGame.ELoadMode.Normal, 0, data, forceSave: true);
			return;
		}
		Debug.Log("[SaveLoadManager] Loading slot " + fileName + " failed (" + result.ToString() + "), starting new game!!");
		LocalGameManager.singleton.NewGame();
	}

	public static void PerformAutosave(bool force = false)
	{
		if (!singleton.m_IsSaving && (force || singleton.m_DelayBeforeNextSaveAllowed <= 0f))
		{
			StrSaveFileNameToFullPath(singleton.strSaveGameName + singleton.strBackupExtension);
			StrSaveFileNameToFullPath(singleton.strSaveGameName);
			singleton.m_DelayBeforeNextSaveAllowed = 60f;
			Save(singleton.strSaveGameName);
			LastAutoSaveDenied = false;
		}
		else
		{
			LastAutoSaveDenied = true;
		}
	}

	public void InstancePerformAutosave(bool force = false)
	{
		PerformAutosave(force);
	}

	public static void PerformReloadIsland()
	{
		singleton.StartCoroutine(singleton.ReloadIsland());
	}

	private IEnumerator ReloadIsland()
	{
		yield return StartCloudTransition(withLogo: true);
		while (m_IsSaving)
		{
			yield return null;
		}
		PerformAutosave(force: true);
		while (m_IsSaving)
		{
			yield return null;
		}
		LoadGameIfExistsOtherwiseNewGame();
	}

	public static void ArchiveIsland()
	{
		LocalGameManager.singleton.bViewingArchivedIsland = true;
		Save(singleton.strArchiveIslandName + "_" + StrGetCurrentTimeStamp(), _bArchive: true);
		LocalGameManager.singleton.bViewingArchivedIsland = false;
	}

	public static string GetFileExtension()
	{
		return FILE_EXTENSION;
	}

	public static string StrGetCurrentTimeStamp()
	{
		return DateTime.Now.ToString("yyyyMMddHHmmssfff");
	}

	public static void DeleteAutosave()
	{
		string dataName = StrSaveFileNameToFullPath(singleton.strSaveGameName);
		PlatformPlayerManagerSystem.Instance.DeleteData(dataName, null);
	}

	public static void Save(string _strFileName, bool _bArchive = false)
	{
		singleton.m_IsSaving = true;
		LocalGameManager.singleton.iScore += CoinManager.Singleton.IGetCoinTransitionBalance();
		saveGameCurrent.PopulateFile();
		LocalGameManager.singleton.iScore -= CoinManager.Singleton.IGetCoinTransitionBalance();
		WriteSaveFile(_strFileName, _bArchive);
	}

	private static void WriteSaveFile(string _strFileName, bool _bArchive)
	{
		FlatBufferBuilder flatBufferBuilder = singleton.m_FlatBufferBuilder;
		flatBufferBuilder.Clear();
		saveGameCurrent.ToFlatBuffer(flatBufferBuilder);
		byte[] data = flatBufferBuilder.DataBuffer.ToSizedArray();
		PlatformPlayerManagerSystem.Instance.SaveData(StrSaveFileNameToFullPath(_strFileName, _bArchive), ref data, OnSaveCompleted);
	}

	public static void PerformSaveExternal(string _strFileName, int slot)
	{
		singleton.StartCoroutine(singleton.SaveExternal(_strFileName, slot));
	}

	private IEnumerator SaveExternal(string _strFileName, int slot)
	{
		while (m_IsSaving)
		{
			yield return null;
		}
		singleton.m_IsSaving = true;
		saveGameCurrent.PopulateFile();
		FlatBufferBuilder flatBufferBuilder = singleton.m_FlatBufferBuilder;
		flatBufferBuilder.Clear();
		saveGameCurrent.ToFlatBuffer(flatBufferBuilder);
		byte[] data = flatBufferBuilder.DataBuffer.ToSizedArray();
		PlatformPlayerManagerSystem.Instance.SaveData(_strFileName, ref data, OnSaveExternalCompleted, slot);
	}

	private static void OnSaveCompleted(string arg1, bool arg2)
	{
		singleton.m_IsSaving = false;
		if (singleton.m_OnLastAutoSaveDeniedChange != null)
		{
			singleton.m_OnLastAutoSaveDeniedChange(LastAutoSaveDenied, singleton.m_IsSaving);
		}
	}

	private void OnSaveExternalCompleted(string arg1, bool arg2)
	{
		Debug.Log("[SaveLoadManager] Save External Completed");
		singleton.m_IsSaving = false;
	}

	public static void Load(SaveGame.ELoadMode _eLoadMode, int _iIsland = 0, byte[] data = null, bool forceSave = false)
	{
		singleton.StartCoroutine(singleton.LoadTransition(_eLoadMode, _iIsland, data, forceSave));
	}

	public IEnumerator StartCloudTransition(bool withLogo, bool withPrompt = false)
	{
		bInLoadTransition = true;
		if (eTransitionStateCurrent != EtransitionState.TransitionStart && eTransitionStateCurrent != EtransitionState.TransitionMid)
		{
			LocalGameManager.singleton.StopAllCoroutines();
			eTransitionStateCurrent = EtransitionState.TransitionStart;
			eventOnTransitionStart.Invoke();
			animatorCamera.SetBool("StartCamTransition", value: true);
			animatorCamera.SetBool("EndCamTransition", value: false);
			if (goIslandersLogo != null)
			{
				goIslandersLogo.SetActive(withLogo);
			}
			if (m_PressButtonPrompt != null)
			{
				m_PressButtonPrompt.SetActive(withPrompt);
			}
			while (!animatorCamera.GetCurrentAnimatorStateInfo(0).IsName("IslandTransitionMid"))
			{
				yield return null;
			}
			eTransitionStateCurrent = EtransitionState.TransitionMid;
		}
	}

	public IEnumerator EndCloudTransition()
	{
		CameraController.singleton.SetToDefaultPos();
		yield return null;
		eventOnTransitionMidEnd.Invoke();
		eTransitionStateCurrent = EtransitionState.TransitionEnd;
		animatorCamera.SetBool("StartCamTransition", value: false);
		animatorCamera.SetBool("EndCamTransition", value: true);
		while (!animatorCamera.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
		{
			yield return null;
		}
		eTransitionStateCurrent = EtransitionState.NotInTransition;
		eventOnTransitionEnd.Invoke();
		bInLoadTransition = false;
	}

	private IEnumerator LoadNewGameStep(SaveGame.ELoadMode _eLoadMode, int _iIsland = 0)
	{
		saveGameCurrent = new SaveGame();
		saveGameCurrent.randomStateIslandCreation = UnityEngine.Random.state;
		saveGameCurrent.iDebugLinearIndexIslandGenerator = _iIsland;
		yield return null;
		StartCoroutine(saveGameCurrent.ApplyFile(_eLoadMode));
	}

	public IEnumerator LoadTransition(SaveGame.ELoadMode _eLoadMode, int _iIsland = 0, byte[] data = null, bool forceSave = false)
	{
		yield return StartCloudTransition(withLogo: true);
		if (_eLoadMode != 0)
		{
			if (!m_bRecovered)
			{
				StartCoroutine(RecoveryIsland.LoadGrizzlyFile(delegate
				{
					LocalGameManager.singleton.GameState = LocalGameManager.EGameState.PreGame;
					_eLoadMode = SaveGame.ELoadMode.Normal;
					m_bRecovered = true;
				}, delegate
				{
					StartCoroutine(LoadNewGameStep(_eLoadMode, _iIsland));
				}));
			}
			else
			{
				StartCoroutine(LoadNewGameStep(_eLoadMode, _iIsland));
			}
		}
		else
		{
			m_bRecovered = true;
			ReadSaveFile(data);
			yield return null;
			StartCoroutine(saveGameCurrent.ApplyFile());
		}
		while (!saveGameCurrent.BApplyFileDone)
		{
			yield return null;
		}
		yield return null;
		if (_eLoadMode != SaveGame.ELoadMode.Normal || forceSave)
		{
			yield return null;
			PerformAutosave(force: true);
		}
		yield return null;
		colorBaker.ClearCache();
		colorBaker.BakeEntireScene();
		while (colorBaker.bBakingInProcess)
		{
			yield return null;
		}
		yield return null;
		TutorialManager.AfterLoad();
		TutorialSandboxManager.AfterLoad();
		yield return EndCloudTransition();
		if (LocalGameManager.singleton.GameState != LocalGameManager.EGameState.InGame)
		{
			Stats statsMatch = StatsManager.statsMatch;
			bool num = statsMatch.iBuildingsBuilt == statsMatch.iReceivedBuildings;
			bool flag = statsMatch.iBuildingPacksUnlocked == statsMatch.liIRequiredScoreForPack.Count;
			bool flag2 = statsMatch.iTotalScore < LocalGameManager.singleton.IRequiredScoreForNextIsland;
			bool flag3 = LocalGameManager.singleton.GameMode != LocalGameManager.EGameMode.Sandbox;
			bool flag4 = num && flag && flag2 && flag3;
			if (_eLoadMode == SaveGame.ELoadMode.NewGame || flag4)
			{
				UiCanvasManager.Singleton.ToMenuNoCurrent();
			}
			else if (_eLoadMode == SaveGame.ELoadMode.Normal)
			{
				UiCanvasManager.Singleton.ToMenuWithCurrent();
			}
		}
		yield return null;
	}

	private static void ReadSaveFile(byte[] data)
	{
		bool flag = true;
		if (data != null)
		{
			ByteBuffer bb = new ByteBuffer(data);
			if (!SaveData.SaveDataBufferHasIdentifier(bb))
			{
				flag = false;
				saveGameCurrent = new SaveGame();
				Debug.LogWarning("[SaveLoadManager] Couldn't find identifier in save data buffer!");
				return;
			}
			SaveData rootAsSaveData = SaveData.GetRootAsSaveData(bb);
			saveGameCurrent.FromFlatBuffer(rootAsSaveData);
		}
		else
		{
			flag = false;
		}
		if (!flag)
		{
			PlatformPlayerManagerSystem.Instance.LoadData(StrSaveFileNameToFullPath(singleton.strSaveGameName + singleton.strBackupExtension), OnBackupFileLoaded);
		}
	}

	private static void OnBackupFileLoaded(string fileName, LoadResult result, byte[] data)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		if (result == LoadResult.Success)
		{
			try
			{
				using MemoryStream serializationStream = new MemoryStream(data);
				saveGameCurrent = (SaveGame)binaryFormatter.Deserialize(serializationStream);
				return;
			}
			catch (SerializationException ex)
			{
				Debug.LogWarning(ex.Message);
				saveGameCurrent = new SaveGame();
				return;
			}
		}
		Debug.LogError("[SaveLoadManager] Failed to load backup");
	}

	public static string StrSaveFileNameToFullPath(string _strName, bool _bArchive = false)
	{
		if (_bArchive)
		{
			return Path.Combine("Arch", _strName + ARCHIVE_EXTENSION);
		}
		return _strName + FILE_EXTENSION;
	}

	public void ClearStoredUndoData()
	{
		buildingLastBuiltForUndo = null;
	}

	public void Undo()
	{
		if (!bIsUndoAvailable())
		{
			return;
		}
		UiBuildingButtonManager.singleton.GoSelectedButton = null;
		UiBuildingButtonManager.singleton.DeselectUndoAndPlusButtons();
		LocalGameManager.singleton.AddBuildingToInventory(buildingLastBuiltForUndo.goOritinalPrefab, 1);
		buildingLastBuiltForUndo.uiBuildingButtonForUndo.iBuildingSeed = buildingLastBuiltForUndo.iBuildingVariationSeedForUndo;
		buildingLastBuiltForUndo.uiBuildingButtonForUndo.iBuildingSeedNext = buildingLastBuiltForUndo.iBuildingVariationSeedForUndoNext;
		CoinManager.Singleton.InstantlyFinishOffAllCoinTransitions();
		_ = StatsManager.statsMatch.liIPlacedBuildings;
		List<int> liIScorePerBuilding = StatsManager.statsMatch.liIScorePerBuilding;
		int index = liIScorePerBuilding.Count - 1;
		LocalGameManager.singleton.IScore -= liIScorePerBuilding[index];
		StatsManager.statsMatch.iHighscore -= liIScorePerBuilding[index];
		StatsManager.statsMatch.iTotalScore -= liIScorePerBuilding[index];
		if (liIScorePerBuilding[index] < 0)
		{
			StatsManager.statsMatch.iTotalNegativePoints -= -liIScorePerBuilding[index];
		}
		liIScorePerBuilding.RemoveAt(index);
		StatsManager.statsMatch.liIMilisecondsPerBuilding.RemoveAt(index);
		StatsManager.statsMatch.iTotalBuildingsBuilt--;
		StatsManager.statsMatch.iBuildingsBuilt--;
		LocalGameManager.singleton.IUnlockedBoosterPacks = iUnlockedBoosterPacksBeforePlacingLastBuildingForUndo;
		List<int> liIPlusBuildingButtonsIncludingBuildingCounts = LocalGameManager.singleton.liIPlusBuildingButtonsIncludingBuildingCounts;
		while (liIPlusBuildingButtonsIncludingBuildingCounts.Count > iAvailablePlusesBeforePlacingLastBuildingForUndo)
		{
			liIPlusBuildingButtonsIncludingBuildingCounts.RemoveAt(liIPlusBuildingButtonsIncludingBuildingCounts.Count - 1);
		}
		LocalGameManager.singleton.IRequiredScoreForNextPack = iScoreGoalBeforePlacingLastBuildingForUndo;
		LocalGameManager.singleton.IRequiredScoreForLastPack = iScoreBaseBeforePlacingLastBuildingForUndo;
		LocalGameManager.singleton.CheckForNewBoosterPack();
		UIPlusBuildingsButton.singleton.UpdateButton();
		buildingLastBuiltForUndo.Demolish();
		AudioManager.singleton.PlayUndoSound();
		foreach (GameObject item in liGoDisabledObjectsOnLasPlacementForUndo)
		{
			if (item != null)
			{
				item.SetActive(value: true);
			}
		}
		liGoDisabledObjectsOnLasPlacementForUndo.Clear();
		LocalGameManager.singleton.CheckForNewIsland(_bOverrideAvailable: true);
		PerformAutosave();
	}

	public bool bIsUndoAvailable()
	{
		if (!buildingLastBuiltForUndo)
		{
			return false;
		}
		if (!buildingLastBuiltForUndo.goOritinalPrefab)
		{
			return false;
		}
		if (buildingLastBuiltForUndo.IsBeingDemolish())
		{
			return false;
		}
		if (StatsManager.statsMatch.liIPlacedBuildings.Count <= 0)
		{
			return false;
		}
		return true;
	}

	public void DeleteLastRemovedOverlapObjectsForGood()
	{
		for (int num = liGoDisabledObjectsOnLasPlacementForUndo.Count - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(liGoDisabledObjectsOnLasPlacementForUndo[num]);
		}
		liGoDisabledObjectsOnLasPlacementForUndo.Clear();
	}

	public int IGetIslandPlaceableID(GameObject _goPrefab)
	{
		iReturn = liGoIslandPlaceableIDs.IndexOf(_goPrefab);
		return iReturn;
	}

	public GameObject GoGetIslandPlaceable(int _iID)
	{
		if (_iID < 0 || _iID >= liGoIslandPlaceableIDs.Count)
		{
			return null;
		}
		return liGoIslandPlaceableIDs[_iID];
	}

	public static void SaveIslandObjectToCurrentSaveFile(GameObject _goPrefab, UnityEngine.Transform _transObject)
	{
		iID = singleton.IGetIslandPlaceableID(_goPrefab);
		if (saveGameCurrent == null)
		{
			Debug.LogError("Save Game Current is Null.");
		}
		if (saveGameCurrent.liIPlaceableIds == null)
		{
			Debug.LogError("LiIPlaceableIds is Null.");
		}
		saveGameCurrent.liIPlaceableIds.Add(iID);
		saveGameCurrent.liSeriTransOfPlaceables.Add(new SeriTransform(_transObject));
	}
}
