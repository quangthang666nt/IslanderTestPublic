using System.Collections;
using System.Collections.Generic;
using CustomEvents;
using UnityEngine;
using UnityEngine.Events;

public class LocalGameManager : MonoBehaviour
{
	public enum EGameState
	{
		TitleScreen = 0,
		PreGame = 1,
		InGame = 2,
		GameOver = 3
	}

	public enum EGameMode
	{
		Default = 0,
		Sandbox = 1
	}

	[Help("This script manages the core game loop including score, inventory and islands.", MessageType.Info)]
	[HideInInspector]
	public int iGameVersionCurrentlyPlayingOn = -1;

	public const int iGameVersionInstalled = 1;

	[HideInInspector]
	public UnityEvent eventOnBuildingChoiceActivate = new UnityEvent();

	private bool bPlayerHasAlreadyBeenInGame;

	public static LocalGameManager singleton;

	private UiBuildingButtonManager uiBuildingButtonManager;

	private IslandManager islandManager;

	[SerializeField]
	[Header("Building Resources")]
	[Tooltip("The UI element used in the building inventory.")]
	private GameObject goBuildingButtonPrefab;

	private List<GameObject> liGoAddedBuildings = new List<GameObject>();

	private List<GameObject> liGoButtons = new List<GameObject>();

	[Tooltip("Every number in this list represents one plus button the player can press with <int> buildings inside.")]
	public List<int> liIPlusBuildingButtonsIncludingBuildingCounts = new List<int>();

	[SerializeField]
	[Tooltip("The amount of buildings you can choose from when clicking on a plus.")]
	private int iChoiceAmountForUnlocks = 2;

	[SerializeField]
	[Tooltip("The amount of points you need to reach to get a new plus button. (Resets on every new island).")]
	private List<int> liIGetNewBuildingsAtScore = new List<int>();

	[SerializeField]
	[Tooltip("When the list above ends, this is is how much the required score increases after each plus button.")]
	private int iIncreaseRequiredScoreAfterList = 2;

	[SerializeField]
	[Tooltip("Determines how many buildings are in each plus button.")]
	private AnimationCurve animCurveBuildingsPerPack;

	private int iRequiredScoreForNextPack;

	private int iRequiredScoreForLastPack;

	private int iUnlockedBoosterPacks;

	[HideInInspector]
	public GoEvent eventAddBuilding = new GoEvent();

	[HideInInspector]
	public UnityEvent eventOnInventoryRefill = new UnityEvent();

	private float fBoosterPackProgress;

	[SerializeField]
	[Tooltip("Determines how often you can pick between new buildings. 1 means every time you press on a plus button. 2 means every 2nd time etc.")]
	private int iNewBuildingsLevelInterval = 1;

	[HideInInspector]
	public UnityEvent eventOnNewBoosterPack = new UnityEvent();

	[HideInInspector]
	public int iLastGottenBuildingAmountSave;

	[HideInInspector]
	public UnityEvent eventOnPreGame = new UnityEvent();

	[HideInInspector]
	public UnityEvent eventOnRoundStart = new UnityEvent();

	[HideInInspector]
	public UnityEvent eventOnGameOver = new UnityEvent();

	private EGameState eGameState = EGameState.PreGame;

	private EGameMode eGameMode;

	[Tooltip("List of buildings available in sandbox mode.")]
	public List<GameObject> sandboxBuldings = new List<GameObject>();

	[Header("Islands & Score")]
	[Tooltip("Is true if you're viewing an old archived island. This feature is currently only available by manually renaming arch files.")]
	public bool bViewingArchivedIsland;

	[SerializeField]
	[Tooltip("The bonus points you receive for traveling to the next island.")]
	private int iNextIslandBonusScore = 200;

	[Tooltip("The current score of the current match.")]
	public int iScore;

	[SerializeField]
	private int lockProgressAtIsland = 20;

	private int iRequiredScoreForNextIsland;

	private int iScoreWhenEnteredThisIsland;

	private int iCurrentActiveIsland;

	private bool bNextIslandAvailable;

	public bool BPlayerHasAlreadyBeenInGame
	{
		get
		{
			return bPlayerHasAlreadyBeenInGame;
		}
		set
		{
			bPlayerHasAlreadyBeenInGame = value;
		}
	}

	public Dictionary<GameObject, int> dicBuildingInventory { get; } = new Dictionary<GameObject, int>();


	public int IRequiredScoreForNextPack
	{
		get
		{
			return iRequiredScoreForNextPack;
		}
		set
		{
			iRequiredScoreForNextPack = value;
		}
	}

	public int IRequiredScoreForLastPack
	{
		get
		{
			return iRequiredScoreForLastPack;
		}
		set
		{
			iRequiredScoreForLastPack = value;
		}
	}

	public int IUnlockedBoosterPacks
	{
		get
		{
			return iUnlockedBoosterPacks;
		}
		set
		{
			iUnlockedBoosterPacks = value;
		}
	}

	public EGameState GameState
	{
		get
		{
			return eGameState;
		}
		set
		{
			if (value == eGameState)
			{
				return;
			}
			switch (value)
			{
			case EGameState.InGame:
				eventOnRoundStart.Invoke();
				if (GameMode != EGameMode.Sandbox)
				{
					break;
				}
				dicBuildingInventory.Clear();
				foreach (GameObject sandboxBulding in sandboxBuldings)
				{
					AddBuildingToInventory(sandboxBulding, 1);
				}
				break;
			case EGameState.GameOver:
				eventOnGameOver.Invoke();
				AudioManager.singleton.PlayGameOver();
				StatsManager.OnMatchEnd();
				SaveLoadManager.PerformAutosave(force: true);
				break;
			}
			eGameState = value;
		}
	}

	public EGameMode GameMode
	{
		get
		{
			return eGameMode;
		}
		set
		{
			eGameMode = value;
		}
	}

	public int IScore
	{
		get
		{
			return iScore;
		}
		set
		{
			iScore = value;
			CheckForNewBoosterPack();
			CheckForNewIsland();
		}
	}

	public int IRequiredScoreForNextIsland
	{
		get
		{
			return iRequiredScoreForNextIsland;
		}
		set
		{
			iRequiredScoreForNextIsland = value;
		}
	}

	public int IScoreWhenEnteredThisIsland
	{
		get
		{
			return iScoreWhenEnteredThisIsland;
		}
		set
		{
			iScoreWhenEnteredThisIsland = value;
		}
	}

	public int ICurrentActiveIsland
	{
		get
		{
			return iCurrentActiveIsland;
		}
		set
		{
			iCurrentActiveIsland = value;
		}
	}

	public bool BNextIslandAvailable => bNextIslandAvailable;

	public float FUnlockIslandProgress
	{
		get
		{
			if (IsProgressLocked())
			{
				return 0f;
			}
			int num = iRequiredScoreForNextIsland;
			if ((bool)TerrainGenerator.singleton)
			{
				num = (int)((float)iRequiredScoreForNextIsland * TerrainGenerator.singleton.FScoreGoalMultiplyer) + TerrainGenerator.singleton.IScoreGoalOffset;
			}
			return Mathf.InverseLerp(iScoreWhenEnteredThisIsland, num, iScore);
		}
	}

	public void Awake()
	{
		singleton = this;
	}

	private void Start()
	{
		eventOnPreGame.Invoke();
		islandManager = IslandManager.singleton;
		uiBuildingButtonManager = UiBuildingButtonManager.singleton;
		UiBuildingButtonManager.singleton.eventOnBuildingPlace.AddListener(CheckForGameOver);
		iRequiredScoreForNextIsland = islandManager.LiIslandsAndGoals[0].iScoreGoal;
	}

	public void AddBuildingToInventory(GameObject _goAdd, int _iAmount)
	{
		EnsureBuildingButtonIsPresent(_goAdd);
		if (dicBuildingInventory.ContainsKey(_goAdd))
		{
			int value = dicBuildingInventory[_goAdd] + _iAmount;
			dicBuildingInventory[_goAdd] = value;
		}
		else if ((bool)_goAdd)
		{
			dicBuildingInventory.Add(_goAdd, _iAmount);
		}
		CountBuildingsInInventoryForStats();
		StatsManager.statsMatch.CheckForAchievements();
	}

	public void SubstractBuildingFromInventory(GameObject _goSubstract, int _iAmount)
	{
		if (eGameMode != EGameMode.Sandbox)
		{
			if (dicBuildingInventory.ContainsKey(_goSubstract))
			{
				int value = dicBuildingInventory[_goSubstract] - _iAmount;
				dicBuildingInventory[_goSubstract] = value;
			}
			else
			{
				dicBuildingInventory.Add(_goSubstract, -_iAmount);
			}
			CountBuildingsInInventoryForStats();
		}
	}

	private void CountBuildingsInInventoryForStats()
	{
		int num = 0;
		foreach (int value in dicBuildingInventory.Values)
		{
			num += value;
		}
		if (GameMode == EGameMode.Default)
		{
			StatsManager.statsMatch.iMaxBuildingsInInventoryInAnyOrThisRun = Mathf.Max(StatsManager.statsMatch.iMaxBuildingsInInventoryInAnyOrThisRun, num);
		}
	}

	public UiBuildingButton EnsureBuildingButtonIsPresent(GameObject _goAdd)
	{
		if (!liGoAddedBuildings.Contains(_goAdd))
		{
			uiBuildingButtonManager = UiBuildingButtonManager.singleton;
			Building component = _goAdd.GetComponent<Building>();
			liGoAddedBuildings.Add(_goAdd);
			Transform transButtonParentNormalMode = uiBuildingButtonManager.transButtonParentNormalMode;
			GameObject gameObject = Object.Instantiate(goBuildingButtonPrefab, transButtonParentNormalMode);
			gameObject.transform.SetSiblingIndex(gameObject.transform.childCount - 1);
			liGoButtons.Add(gameObject);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			UiBuildingButton component2 = gameObject.GetComponent<UiBuildingButton>();
			uiBuildingButtonManager.LiBuildingButtonsExisting.Add(component2);
			component2.buildingName = component.strBuildingName;
			component2.goResourceBuilding = _goAdd;
			return component2;
		}
		for (int i = 0; i < uiBuildingButtonManager.LiBuildingButtonsExisting.Count; i++)
		{
			if (uiBuildingButtonManager.LiBuildingButtonsExisting[i].goResourceBuilding == _goAdd)
			{
				return uiBuildingButtonManager.LiBuildingButtonsExisting[i];
			}
		}
		return null;
	}

	public GameObject GetButton(GameObject _goOfInterest)
	{
		foreach (GameObject liGoButton in liGoButtons)
		{
			if (liGoButton.GetComponent<UiBuildingButton>().goResourceBuilding == _goOfInterest)
			{
				return liGoButton;
			}
		}
		return null;
	}

	public int IBuildingsInInventory(GameObject _goCheck)
	{
		if (eGameMode == EGameMode.Sandbox)
		{
			return 1;
		}
		if (dicBuildingInventory.ContainsKey(_goCheck))
		{
			return dicBuildingInventory[_goCheck];
		}
		return 0;
	}

	public void CalculateBoosterPackProgress()
	{
		if (iRequiredScoreForLastPack != iRequiredScoreForNextPack)
		{
			fBoosterPackProgress = Mathf.InverseLerp(iRequiredScoreForLastPack, iRequiredScoreForNextPack, iScore);
			fBoosterPackProgress = Mathf.Clamp(fBoosterPackProgress, 0f, 1f);
		}
		else if (iScore >= iRequiredScoreForNextPack)
		{
			fBoosterPackProgress = 1f;
		}
		else
		{
			fBoosterPackProgress = 0f;
		}
	}

	public void CheckForNewBoosterPack()
	{
		if (GameMode == EGameMode.Sandbox)
		{
			return;
		}
		fBoosterPackProgress = 1f;
		int num = 100;
		while (fBoosterPackProgress >= 1f && num > 0)
		{
			num--;
			CalculateBoosterPackProgress();
			if (fBoosterPackProgress >= 1f)
			{
				if (iUnlockedBoosterPacks != 0)
				{
					eventOnNewBoosterPack.Invoke();
				}
				int item = (int)Mathf.Round(animCurveBuildingsPerPack.Evaluate(iUnlockedBoosterPacks));
				iUnlockedBoosterPacks++;
				if (StatsManager.statsMatch.liIRequiredScoreForPack == null)
				{
					StatsManager.statsMatch.liIRequiredScoreForPack = new List<int>();
				}
				StatsManager.statsMatch.liIRequiredScoreForPack.Add(iRequiredScoreForNextPack);
				iRequiredScoreForLastPack = iRequiredScoreForNextPack;
				iRequiredScoreForNextPack += liIGetNewBuildingsAtScore[Mathf.Min(iUnlockedBoosterPacks, liIGetNewBuildingsAtScore.Count - 1)];
				iRequiredScoreForNextPack += iIncreaseRequiredScoreAfterList * Mathf.Max(iUnlockedBoosterPacks - liIGetNewBuildingsAtScore.Count, 0);
				iRequiredScoreForNextPack = (int)Mathf.Floor((float)iRequiredScoreForNextPack / 5f) * 5;
				liIPlusBuildingButtonsIncludingBuildingCounts.Add(item);
				UIPlusBuildingsButton.singleton.UpdateButton();
			}
		}
	}

	public void BuildingPackUnlocked()
	{
		BuildorderBrainB.singleton.SetNextGuaranteedBuilings();
		int num = liIPlusBuildingButtonsIncludingBuildingCounts[0];
		if (GameMode == EGameMode.Default)
		{
			StatsManager.statsMatch.iReceivedBuildings += num;
			StatsManager.statsMatch.liIReceivedBuildings.Add(num);
			StatsManager.statsMatch.iBuildingPacksUnlocked++;
		}
		liIPlusBuildingButtonsIncludingBuildingCounts.RemoveAt(0);
		SaveLoadManager.singleton.ClearStoredUndoData();
		StartCoroutine(GetBuildings(num));
	}

	public void OpenBuildingChoice()
	{
		if (liIPlusBuildingButtonsIncludingBuildingCounts.Count > 0)
		{
			SaveLoadManager.singleton.ClearStoredUndoData();
			eventOnBuildingChoiceActivate.Invoke();
			UIBuildingChoice.Singleton.Activate();
			uiBuildingButtonManager.DeselectUndoAndPlusButtons();
		}
	}

	public IEnumerator GetBuildings(int _iAmount)
	{
		iLastGottenBuildingAmountSave = _iAmount;
		if ((bool)UIBuildingChoice.Singleton)
		{
			while (UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.InGamePicking || UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.MenuWithCurrent)
			{
				yield return null;
			}
		}
		while (_iAmount > 0)
		{
			eventOnInventoryRefill.Invoke();
			GameObject gameObject = BuildorderBrainB.singleton.GoGetBuilding();
			if ((bool)gameObject)
			{
				eventAddBuilding.Invoke(gameObject);
			}
			_iAmount--;
		}
	}

	public void SetGameMode(EGameMode _eGoToMode)
	{
		eGameMode = _eGoToMode;
	}

	public void CheckForGameOver()
	{
		if (singleton.GameMode != EGameMode.Sandbox)
		{
			StartCoroutine(DelayGameOverCheckUntilCoinsArrived());
		}
	}

	private IEnumerator DelayGameOverCheckUntilCoinsArrived()
	{
		yield return null;
		while (CoinManager.Singleton.BCoinBarFeedbackRunning || CoinManager.Singleton.IGetCoinTransitionBalance() > 0)
		{
			yield return null;
		}
		yield return null;
		bool flag = true;
		foreach (KeyValuePair<GameObject, int> item in dicBuildingInventory)
		{
			if (item.Value > 0)
			{
				flag = false;
			}
		}
		if (flag && FeedbackManager.Singleton.IButtonsInTransition <= 0 && eGameState != EGameState.GameOver && UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.InGamePicking && UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.MenuWithCurrent && !bNextIslandAvailable && liIPlusBuildingButtonsIncludingBuildingCounts.Count <= 0)
		{
			GameState = EGameState.GameOver;
		}
	}

	public void GameModeToDefault()
	{
		_ = GameMode;
		GameMode = EGameMode.Default;
		NewGame();
	}

	public void GameModeToSandboxGeneration()
	{
		_ = GameMode;
		GameMode = EGameMode.Sandbox;
		SandboxGenerator.singleton.SetDefaultSandboxGenerationView();
		UiCanvasManager.Singleton.ToNewSandbox();
	}

	public void GameModeToSandbox()
	{
		_ = GameMode;
		GameMode = EGameMode.Sandbox;
		NewGame();
	}

	public bool IsProgressLocked()
	{
		return iCurrentActiveIsland >= lockProgressAtIsland;
	}

	public void CheckForNewIsland(bool _bOverrideAvailable = false)
	{
		if (!bNextIslandAvailable || _bOverrideAvailable)
		{
			bNextIslandAvailable = FUnlockIslandProgress >= 1f;
		}
	}

	public bool BTryToTravelToNextIsland()
	{
		if (bNextIslandAvailable)
		{
			TravelToNextIsland();
			return true;
		}
		return false;
	}

	public void TryTravelToNextIsland()
	{
		BTryToTravelToNextIsland();
	}

	public void TravelToNextIsland()
	{
		PlatformPlayerManagerSystem.Instance?.AddPlayerStat("GS_FINISHED_ISLANDS", 1);
		CoinManager.Singleton.InstantlyFinishOffAllCoinTransitions();
		islandManager = IslandManager.singleton;
		islandManager.BlockCurrentIslandFromAppearingAgain();
		int num = iScore;
		iScore += iNextIslandBonusScore;
		iScore = (int)Mathf.Ceil((float)iScore / 10f) * 10;
		int num2 = iScore - num;
		if (GameMode == EGameMode.Default)
		{
			StatsManager.statsMatch.iHighscore += num2;
			StatsManager.statsMatch.iTotalScore += num2;
			StatsManager.statsMatch.liIPlacedBuildings.Add(-1);
			StatsManager.statsMatch.liIScorePerBuilding.Add(num2);
			int item = (int)(StatsManager.statsMatch.fTotalTimePlayed * 1000f);
			StatsManager.statsMatch.liIMilisecondsPerBuilding.Add(item);
			StatsManager.statsMatch.liIWentToNextIslandAtScores.Add(num);
			StatsManager.statsMatch.liIWentToNextIslandAtMiliseconds.Add(item);
		}
		BuildorderBrainB.singleton.Reset();
		iRequiredScoreForLastPack = iScore;
		iRequiredScoreForNextPack = iScore;
		iCurrentActiveIsland++;
		iScoreWhenEnteredThisIsland = iScore;
		iRequiredScoreForNextIsland = iScoreWhenEnteredThisIsland + islandManager.LiIslandsAndGoals[Mathf.Min(iCurrentActiveIsland, islandManager.LiIslandsAndGoals.Count - 1)].iScoreGoal;
		bNextIslandAvailable = false;
		if (GameMode == EGameMode.Default)
		{
			StatsManager.statsMatch.iMaxIslandsReachedInAnyOrThisRun++;
			StatsManager.statsMatch.iTotalIslandsReached++;
		}
		StatsManager.statsMatch.CheckForAchievements();
		ArchiveManager.CleanCurrent();
		SaveLoadManager.Load(SaveGame.ELoadMode.NextIsland, iCurrentActiveIsland);
	}

	public void StartRound()
	{
		GameState = EGameState.InGame;
		UiCanvasManager.Singleton.ToStartMatch();
		CheckForGameOver();
	}

	public void NewGame()
	{
		CoinManager.Singleton.InstantlyFinishOffAllCoinTransitions();
		StatsManager.OnMatchEnd();
		UiBuildingButtonManager.singleton.OnStartMatch();
		ArchiveManager.CleanCurrent();
		SaveLoadManager.Load(SaveGame.ELoadMode.NewGame);
		GameState = EGameState.InGame;
		UiCanvasManager.Singleton.ToTransition();
	}

	public void NewGameFromArchive(string slotPath, int slot)
	{
		CoinManager.Singleton.InstantlyFinishOffAllCoinTransitions();
		StatsManager.OnMatchEnd();
		UiBuildingButtonManager.singleton.OnStartMatch();
		SaveLoadManager.LoadGameFromSlot(slotPath, slot);
		GameState = EGameState.InGame;
		UiCanvasManager.Singleton.ToTransition();
	}

	public void NewGameButCheckCurrentIslandForBeingEmpty(EGameMode previousGameMode)
	{
		if (previousGameMode == EGameMode.Sandbox)
		{
			NewGame();
		}
		else if (StatsManager.statsMatch.iBuildingsBuilt == 0)
		{
			StartRound();
		}
		else
		{
			NewGame();
		}
	}

	private void Update()
	{
		if (eGameState == EGameState.InGame && !SaveLoadManager.bInLoadTransition)
		{
			StatsManager.statsMatch.fTotalTimePlayed += Time.deltaTime;
		}
	}
}
