using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class UiCanvasManager : MonoBehaviour
{
	public enum EUIState
	{
		MenuNoCurrent = 0,
		MenuWithCurrent = 1,
		Settings = 2,
		InGamePlaying = 3,
		TitleScreen = 4,
		InGamePicking = 5,
		InGameNewIslandPrompt = 6,
		InGameTransition = 7,
		GameOver = 8,
		InCamTutorial = 9,
		Leaderboard = 10,
		NewGamePrompt = 11,
		ScreenshotMode = 12,
		GameModeSelection = 13,
		NoControllerPrompt = 14,
		SandboxLimitPrompt = 15,
		SigningInPrompt = 16,
		AccountDisconnectionPrompt = 17,
		RedefinePrompt = 18,
		RestoreKeyboardPrompt = 19,
		RestoreGamepadPrompt = 20,
		NewThemeEventPrompt = 21,
		UpdateThemeEventPrompt = 22,
		ScreenshotMenu = 23,
		NewSandboxPrompt = 24,
		ArchiveIslandSandboxPrompt = 25,
		ArchiveIslandHighscorePrompt = 26,
		ArchiveIslandMaxFile = 27,
		ArchiveIslandSavePrompt = 28,
		ArchiveIslandLoadPrompt = 29,
		ArchiveIslandDeletePrompt = 30,
		ArchiveislandDeleteConfirmation = 31,
		ArchiveIslandLoseHighscore = 32,
		ArchiveIslandLoseSandbox = 33,
		ArchiveIslandPhotoPrompt = 34
	}

	[Help("This script is responsible for enabling and disabling UI elements.", MessageType.Info)]
	[SerializeField]
	[Tooltip("Enable to disable game mode selection when you start a new round.")]
	private bool DEBUG_BypassGameModeSelection = true;

	[SerializeField]
	private bool bShowBackButton;

	private static UiCanvasManager singleton;

	[SerializeField]
	private Canvas cnvs;

	[SerializeField]
	private UIElement uieOverlayMenu;

	[SerializeField]
	private UIElement uieBuildingButtons;

	[SerializeField]
	private UIElement uieScore;

	[SerializeField]
	private UIElement uieBackButton;

	[SerializeField]
	private UIElement uieIslandButton;

	[SerializeField]
	private UIElement uieBuildingChoice;

	[SerializeField]
	private UIElement uieNewIslandPrompt;

	[SerializeField]
	private UIElement uieNewIslandPromptSandbox;

	[SerializeField]
	private UIElement uieTravelTransition;

	[SerializeField]
	private UIElement uieGameOverScreen;

	[SerializeField]
	private UIElement uieSettings;

	[SerializeField]
	private UIElement uieCamTutorial;

	[SerializeField]
	private UIElement uieLeaderboard;

	[SerializeField]
	private UIElement uieNewGamePrompt;

	[SerializeField]
	private UIElement uieNoControllerPrompt;

	[SerializeField]
	private UIElement uieSandboxLimitPrompt;

	[SerializeField]
	private UIElement uieGameModeSelection;

	[SerializeField]
	private UIElement uieSigningInPrompt;

	[SerializeField]
	private UIElement uieDisconnectionPrompt;

	[SerializeField]
	private UIElement uieSandboxBuildingButtons;

	[SerializeField]
	private UIElement uieRedefinePrompt;

	[SerializeField]
	private UIElement uieRestoreKeyboardPrompt;

	[SerializeField]
	private UIElement uieRestoreGamepadPrompt;

	[SerializeField]
	private UIElement uieNewThemeEventPrompt;

	[SerializeField]
	private UIElement uieUpdateThemeEventPrompt;

	[SerializeField]
	private UIElement uieNewSandboxPrompt;

	[SerializeField]
	private UIElement uieArchiveIslandSandboxPrompt;

	[SerializeField]
	private UIElement uieArchiveIslandHighscorePrompt;

	[SerializeField]
	private UIElement uieArchiveIslandSavePrompt;

	[SerializeField]
	private UIElement uieArchiveIslandMaxFile;

	[SerializeField]
	private UIElement uieArchiveIslandLoadPrompt;

	[SerializeField]
	private UIElement uieArchiveIslandDeletePrompt;

	[SerializeField]
	private UIElement uieArchiveIslandDeleteConfirmation;

	[SerializeField]
	private UIElement uieArchiveIslandLoseHighscore;

	[SerializeField]
	private UIElement uieArchiveIslandLoseSandbox;

	[SerializeField]
	private UIElement uieArchiveIslandPhotoPrompt;

	[SerializeField]
	private UIElement uieScreenshotMode;

	[SerializeField]
	private UIElement uieScreenshotMenu;

	private EUIState eUIState;

	private EUIState uiStateLastFrame;

	private EUIState eUIStatePrevious;

	private List<EUIState> m_PreviousStates = new List<EUIState>();

	private EUIState eUIStatePreviousInGame = EUIState.InGamePlaying;

	private List<UIElement> liUiElements;

	public Shader m_DefaultUIShader;

	private bool detectInput = true;

	private bool isInScreenshotMode;

	public static UiCanvasManager Singleton => singleton;

	public EUIState UIState => eUIState;

	public EUIState PrevUIState => eUIStatePrevious;

	public EUIState UIStateLastFrame => uiStateLastFrame;

	private void Awake()
	{
		InitializeSingleton();
		InitializeParentList();
		SubscribeTStateEvents();
		Canvas.GetDefaultCanvasMaterial().shader = m_DefaultUIShader;
		cnvs.enabled = false;
		StartCoroutine(EnableCanvasAfterSeconds(0.1f));
	}

	private void Start()
	{
		PlatformPlayerManagerSystem.Instance.OnNoControllerDetected += OnNoControllerDetected;
		PlatformPlayerManagerSystem.Instance.PlatformPlayerManager.OnSystemPanic += OnSystemPanic;
	}

	private void OnDestroy()
	{
		if (PlatformPlayerManagerSystem.IsReady)
		{
			PlatformPlayerManagerSystem.Instance.OnNoControllerDetected -= OnNoControllerDetected;
			PlatformPlayerManagerSystem.Instance.PlatformPlayerManager.OnSystemPanic -= OnSystemPanic;
		}
	}

	private void OnSystemPanic()
	{
		ToAccountDisconnectionPrompt();
	}

	private void OnNoControllerDetected()
	{
		ToNoControllerPrompt();
	}

	private void Update()
	{
		if (detectInput)
		{
			CheckForStateTransitions();
		}
	}

	private void InitializeSingleton()
	{
		if (singleton != null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			singleton = this;
		}
	}

	private void LateUpdate()
	{
		uiStateLastFrame = eUIState;
	}

	private IEnumerator EnableCanvasAfterSeconds(float delay)
	{
		yield return null;
		yield return new WaitForSeconds(delay);
		DisableUIElementsImmediate(liUiElements);
		cnvs.enabled = true;
	}

	public void EnableCanvas()
	{
		cnvs.enabled = true;
	}

	private void InitializeParentList()
	{
		liUiElements = new List<UIElement>();
		liUiElements.Add(uieOverlayMenu);
		liUiElements.Add(uieBuildingButtons);
		if (bShowBackButton)
		{
			liUiElements.Add(uieBackButton);
		}
		else
		{
			uieBackButton.gameObject.SetActive(value: false);
		}
		liUiElements.Add(uieScore);
		liUiElements.Add(uieIslandButton);
		liUiElements.Add(uieBuildingChoice);
		liUiElements.Add(uieNewIslandPrompt);
		liUiElements.Add(uieNewIslandPromptSandbox);
		liUiElements.Add(uieTravelTransition);
		liUiElements.Add(uieGameOverScreen);
		liUiElements.Add(uieSettings);
		liUiElements.Add(uieCamTutorial);
		liUiElements.Add(uieLeaderboard);
		liUiElements.Add(uieNewGamePrompt);
		liUiElements.Add(uieGameModeSelection);
		liUiElements.Add(uieSandboxBuildingButtons);
		liUiElements.Add(uieNoControllerPrompt);
		liUiElements.Add(uieSandboxLimitPrompt);
		liUiElements.Add(uieSigningInPrompt);
		liUiElements.Add(uieDisconnectionPrompt);
		liUiElements.Add(uieRedefinePrompt);
		liUiElements.Add(uieRestoreKeyboardPrompt);
		liUiElements.Add(uieRestoreGamepadPrompt);
		liUiElements.Add(uieNewThemeEventPrompt);
		liUiElements.Add(uieUpdateThemeEventPrompt);
		liUiElements.Add(uieNewSandboxPrompt);
		liUiElements.Add(uieArchiveIslandSandboxPrompt);
		liUiElements.Add(uieArchiveIslandHighscorePrompt);
		liUiElements.Add(uieArchiveIslandMaxFile);
		liUiElements.Add(uieArchiveIslandSavePrompt);
		liUiElements.Add(uieArchiveIslandLoadPrompt);
		liUiElements.Add(uieArchiveIslandDeletePrompt);
		liUiElements.Add(uieArchiveIslandDeleteConfirmation);
		liUiElements.Add(uieArchiveIslandLoseHighscore);
		liUiElements.Add(uieArchiveIslandLoseSandbox);
		liUiElements.Add(uieArchiveIslandPhotoPrompt);
		liUiElements.Add(uieScreenshotMenu);
		liUiElements.Add(uieScreenshotMode);
	}

	private void SubscribeTStateEvents()
	{
		LocalGameManager.singleton.eventOnPreGame.AddListener(ToMenuNoCurrent);
		LocalGameManager.singleton.eventOnRoundStart.AddListener(ToStartMatch);
		LocalGameManager.singleton.eventOnGameOver.AddListener(ToGameOver);
		SaveLoadManager.singleton.eventOnTransitionStart.AddListener(ToTransition);
		SaveLoadManager.singleton.eventOnTransitionEnd.AddListener(OutOfTransition);
	}

	private void DisableUIElements(List<UIElement> _liUIElements)
	{
		foreach (UIElement _liUIElement in _liUIElements)
		{
			_liUIElement.DisableElement();
		}
	}

	private void DisableUIElementsImmediate(List<UIElement> _liUIElements)
	{
		foreach (UIElement _liUIElement in _liUIElements)
		{
			_liUIElement.DisableElementImmediate();
		}
	}

	private void EnableUIElements(List<UIElement> _liUIElements)
	{
		foreach (UIElement _liUIElement in _liUIElements)
		{
			_liUIElement.EnableElement();
		}
	}

	public void ToGameOver()
	{
		SetUIState(EUIState.GameOver);
	}

	public void ToStartMatch()
	{
		if (TutorialManager.eTutorialState == TutorialManager.ETutorialState.DoneAndIgnore || LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			SetUIState(EUIState.InGamePlaying);
		}
		else
		{
			ToCamTutorial();
		}
	}

	public void ToCamTutorial()
	{
		SetUIState(EUIState.InCamTutorial);
	}

	public void ToMenuNoCurrent()
	{
		UiBuildingButtonManager.singleton.GoSelectedButton = null;
		SetUIState(EUIState.MenuNoCurrent);
	}

	public void ToMenuWithCurrent()
	{
		UiBuildingButtonManager.singleton.GoSelectedButton = null;
		SetUIState(EUIState.MenuWithCurrent);
	}

	public void ToNewThemeEvent()
	{
		SetUIState(EUIState.NewThemeEventPrompt);
	}

	public void ToUpdateThemeEvent()
	{
		SetUIState(EUIState.UpdateThemeEventPrompt);
	}

	public void ToArchiveIsland()
	{
		if (LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Default)
		{
			SetUIState(EUIState.ArchiveIslandHighscorePrompt);
		}
		else
		{
			SetUIState(EUIState.ArchiveIslandSandboxPrompt);
		}
	}

	public void ToArchiveIslandMaxFile()
	{
		SetUIState(EUIState.ArchiveIslandMaxFile);
	}

	public void ToArchiveIslandSavePrompt()
	{
		SetUIState(EUIState.ArchiveIslandSavePrompt);
	}

	public void ToArchiveIslandLoadPrompt()
	{
		SetUIState(EUIState.ArchiveIslandLoadPrompt);
	}

	public void ToArchiveIslandDeletePrompt()
	{
		SetUIState(EUIState.ArchiveIslandDeletePrompt);
	}

	public void ToArchiveIslandDeleteConfirmation()
	{
		SetUIState(EUIState.ArchiveislandDeleteConfirmation);
	}

	public void ToArchiveIslandLoseHighscore()
	{
		SetUIState(EUIState.ArchiveIslandLoseHighscore);
	}

	public void ToArchiveIslandLoseSandbox()
	{
		SetUIState(EUIState.ArchiveIslandLoseSandbox);
	}

	public void ToArchivePhotoPrompt()
	{
		SetUIState(EUIState.ArchiveIslandPhotoPrompt);
	}

	public void ToPickBuilding()
	{
		SetUIState(EUIState.InGamePicking);
	}

	public void ToNewIslandPrompt()
	{
		UiBuildingButtonManager.singleton.GoSelectedButton = null;
		SetUIState(EUIState.InGameNewIslandPrompt);
	}

	public void ToPrevious(bool addToStack)
	{
		if (m_PreviousStates.Count > 0)
		{
			EUIState newState = m_PreviousStates[m_PreviousStates.Count - 1];
			m_PreviousStates.RemoveAt(m_PreviousStates.Count - 1);
			SetUIState(newState, addToStack);
		}
		else
		{
			SetUIState(eUIStatePrevious, addToStack);
		}
	}

	public void ToPrevious()
	{
		ToPrevious(addToStack: true);
	}

	public void ToPreviousIngame()
	{
		SetUIState(eUIStatePreviousInGame);
	}

	public void ToTransition()
	{
		SetUIState(EUIState.InGameTransition);
	}

	public void ToAccountDisconnectionPrompt()
	{
		SetUIState(EUIState.AccountDisconnectionPrompt);
	}

	public void ShowRedefinePrompt()
	{
		uieRedefinePrompt.EnableElement();
	}

	public void HideRedefinePrompt()
	{
		uieRedefinePrompt.DisableElement();
	}

	public void ToRestoreKeyboardPrompt()
	{
		uieRestoreKeyboardPrompt.active = false;
		uieRestoreKeyboardPrompt.EnableElement();
	}

	public void ToRestoreGamepadPrompt()
	{
		uieRestoreGamepadPrompt.active = false;
		uieRestoreGamepadPrompt.EnableElement();
	}

	public void ToTitleScreen()
	{
		SetUIState(EUIState.TitleScreen);
	}

	public void ToSettings()
	{
		SetUIState(EUIState.Settings);
	}

	public void ToSettings(bool addToPreviousStack)
	{
		SetUIState(EUIState.Settings, addToPreviousStack);
	}

	internal void ToSigningPopup()
	{
		SetUIState(EUIState.SigningInPrompt);
	}

	public void ToLeaderboard()
	{
		SetUIState(EUIState.Leaderboard);
	}

	public void ToNewGamePrompt()
	{
		SetUIState(EUIState.NewGamePrompt);
	}

	public void ToNoControllerPrompt()
	{
		SetUIState(EUIState.NoControllerPrompt);
	}

	public void ToSandboxLimitPrompt()
	{
		SetUIState(EUIState.SandboxLimitPrompt);
	}

	public void ToScreenshotMode()
	{
		SetUIState(EUIState.ScreenshotMode, addToPreviousStack: false);
	}

	public void ToScreenshotSettings(bool addToPrev = true)
	{
		SetUIState(EUIState.ScreenshotMenu, addToPrev);
	}

	public void SwitchScreenshotMenu()
	{
		if (uieScreenshotMode.active)
		{
			isInScreenshotMode = false;
			ToScreenshotSettings(addToPrev: false);
		}
		else
		{
			isInScreenshotMode = true;
			ToScreenshotMode();
		}
	}

	public void ToNewSandbox()
	{
		SetUIState(EUIState.NewSandboxPrompt);
	}

	public void ToGameModeSelection()
	{
		if (DEBUG_BypassGameModeSelection || (StatsManager.statsMatch.iBuildingsBuilt == 0 && LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Default))
		{
			if (StatsManager.statsMatch.iBuildingsBuilt == 0 && !LocalGameManager.singleton.BPlayerHasAlreadyBeenInGame)
			{
				LocalGameManager.singleton.StartRound();
			}
			else
			{
				SetUIState(EUIState.GameModeSelection);
			}
		}
		else
		{
			SetUIState(EUIState.GameModeSelection);
		}
	}

	public void OutOfTransition()
	{
		cnvs.enabled = true;
		if (LocalGameManager.singleton.GameState == LocalGameManager.EGameState.InGame)
		{
			ToStartMatch();
		}
		else
		{
			ToMenuNoCurrent();
		}
	}

	public bool IsInScreenshotMode()
	{
		return isInScreenshotMode;
	}

	public void SetUIState(EUIState _newState, bool addToPreviousStack = true)
	{
		isInScreenshotMode = false;
		List<UIElement> list = new List<UIElement>();
		switch (_newState)
		{
		case EUIState.MenuNoCurrent:
			list.Add(uieOverlayMenu);
			break;
		case EUIState.MenuWithCurrent:
			list.Add(uieOverlayMenu);
			break;
		case EUIState.InGamePlaying:
			if (bShowBackButton)
			{
				list.Add(uieBackButton);
			}
			LocalGameManager.singleton.BPlayerHasAlreadyBeenInGame = true;
			if (LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Default)
			{
				list.Add(uieScore);
				list.Add(uieIslandButton);
				list.Add(uieBuildingButtons);
			}
			else
			{
				list.Add(uieSandboxBuildingButtons);
				DemolitionController.Unlock();
			}
			break;
		case EUIState.InGamePicking:
			list.Add(uieBuildingChoice);
			list.Add(uieBuildingButtons);
			break;
		case EUIState.InGameNewIslandPrompt:
			if (LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Default)
			{
				list.Add(uieNewIslandPrompt);
			}
			else
			{
				list.Add(uieNewIslandPromptSandbox);
			}
			break;
		case EUIState.GameOver:
			list.Add(uieGameOverScreen);
			break;
		case EUIState.Settings:
			list.Add(uieSettings);
			break;
		case EUIState.InCamTutorial:
			list.Add(uieCamTutorial);
			break;
		case EUIState.Leaderboard:
			list.Add(uieLeaderboard);
			break;
		case EUIState.NewGamePrompt:
			list.Add(uieNewGamePrompt);
			break;
		case EUIState.NoControllerPrompt:
			list.Add(uieNoControllerPrompt);
			break;
		case EUIState.SandboxLimitPrompt:
			list.Add(uieSandboxLimitPrompt);
			break;
		case EUIState.SigningInPrompt:
			list.Add(uieSigningInPrompt);
			break;
		case EUIState.AccountDisconnectionPrompt:
			list.Add(uieDisconnectionPrompt);
			break;
		case EUIState.RedefinePrompt:
			list.Add(uieSettings);
			list.Add(uieRedefinePrompt);
			break;
		case EUIState.RestoreKeyboardPrompt:
			list.Add(uieSettings);
			list.Add(uieRestoreKeyboardPrompt);
			break;
		case EUIState.RestoreGamepadPrompt:
			list.Add(uieSettings);
			list.Add(uieRestoreGamepadPrompt);
			break;
		case EUIState.ScreenshotMode:
			list.Add(uieScreenshotMode);
			isInScreenshotMode = true;
			break;
		case EUIState.ScreenshotMenu:
			list.Add(uieScreenshotMenu);
			break;
		case EUIState.GameModeSelection:
			list.Add(uieGameModeSelection);
			break;
		case EUIState.NewThemeEventPrompt:
			list.Add(uieNewThemeEventPrompt);
			break;
		case EUIState.UpdateThemeEventPrompt:
			list.Add(uieUpdateThemeEventPrompt);
			break;
		case EUIState.NewSandboxPrompt:
			list.Add(uieNewSandboxPrompt);
			break;
		case EUIState.ArchiveIslandSandboxPrompt:
			list.Add(uieArchiveIslandSandboxPrompt);
			break;
		case EUIState.ArchiveIslandHighscorePrompt:
			list.Add(uieArchiveIslandHighscorePrompt);
			break;
		case EUIState.ArchiveIslandMaxFile:
			list.Add(uieArchiveIslandMaxFile);
			break;
		case EUIState.ArchiveIslandSavePrompt:
			list.Add(uieArchiveIslandSavePrompt);
			break;
		case EUIState.ArchiveIslandLoadPrompt:
			list.Add(uieArchiveIslandLoadPrompt);
			break;
		case EUIState.ArchiveIslandDeletePrompt:
			list.Add(uieArchiveIslandDeletePrompt);
			break;
		case EUIState.ArchiveislandDeleteConfirmation:
			list.Add(uieArchiveIslandDeleteConfirmation);
			break;
		case EUIState.ArchiveIslandLoseHighscore:
			list.Add(uieArchiveIslandLoseHighscore);
			break;
		case EUIState.ArchiveIslandLoseSandbox:
			list.Add(uieArchiveIslandLoseSandbox);
			break;
		case EUIState.ArchiveIslandPhotoPrompt:
			list.Add(uieArchiveIslandPhotoPrompt);
			break;
		}
		List<UIElement> list2 = new List<UIElement>(liUiElements);
		foreach (UIElement item in list)
		{
			list2.Remove(item);
		}
		DisableUIElements(list2);
		EnableUIElements(list);
		UITooltip.Singleton.Disable();
		if (eUIState == EUIState.InGamePlaying || eUIState == EUIState.InGamePicking || eUIState == EUIState.InGameNewIslandPrompt || eUIState == EUIState.InGameTransition || eUIState == EUIState.InCamTutorial)
		{
			eUIStatePreviousInGame = eUIState;
		}
		if (IsValidPreviousState(eUIState))
		{
			eUIStatePrevious = eUIState;
			if (addToPreviousStack)
			{
				if (m_PreviousStates.Count > 5)
				{
					m_PreviousStates.RemoveAt(0);
				}
				m_PreviousStates.Add(eUIState);
			}
		}
		eUIState = _newState;
	}

	private bool IsValidPreviousState(EUIState eUIState)
	{
		if (!eUIState.Equals(EUIState.NewThemeEventPrompt) && !eUIState.Equals(EUIState.UpdateThemeEventPrompt) && !eUIState.Equals(EUIState.NewSandboxPrompt) && !eUIState.Equals(EUIState.ArchiveIslandMaxFile) && !eUIState.Equals(EUIState.ArchiveislandDeleteConfirmation) && !eUIState.Equals(EUIState.ArchiveIslandLoseHighscore) && !eUIState.Equals(EUIState.ArchiveIslandLoseSandbox) && !eUIState.Equals(EUIState.ArchiveIslandSavePrompt))
		{
			return !eUIState.Equals(EUIState.ArchiveIslandPhotoPrompt);
		}
		return false;
	}

	public void NewGame()
	{
		LocalGameManager.singleton.NewGame();
	}

	private void CheckForStateTransitions()
	{
		if (eUIState == EUIState.TitleScreen && !InputManager.s_DisableAllInput)
		{
			for (int i = 0; i < ReInput.controllers.controllerCount; i++)
			{
				if (ReInput.controllers.Controllers[i].GetAnyButtonDown())
				{
					Controller controller = ReInput.controllers.Controllers[i];
					for (int num = ReInput.controllers.controllerCount - 1; num >= 0; num--)
					{
						InputManager.Singleton.RewiredPlayer.controllers.RemoveController(ReInput.controllers.Controllers[num]);
					}
					InputManager.Singleton.RewiredPlayer.controllers.AddController(controller, removeFromOtherPlayers: true);
					SaveLoadManager.singleton.ConnectEngagedPlayer();
					return;
				}
			}
		}
		if (eUIState == EUIState.InGamePlaying && InputManager.Singleton.InputDataCurrent.bToggleMenu && !SandboxGenerationView.IsOpened())
		{
			ToggleMenu();
			return;
		}
		if (eUIState == EUIState.MenuNoCurrent || eUIState == EUIState.MenuWithCurrent)
		{
			if (InputManager.Singleton.InputDataCurrent.bOpenScreenshotMode)
			{
				if (AudioManager.singleton != null)
				{
					AudioManager.singleton.PlayMenuClick();
				}
				ToScreenshotSettings();
			}
			else if (InputManager.Singleton.InputDataCurrent.bOpenSettings)
			{
				ToSettings();
			}
		}
		if (eUIState == EUIState.MenuNoCurrent || eUIState == EUIState.MenuWithCurrent)
		{
			if (InputManager.Singleton.InputDataCurrent.bToggleLeaderboard)
			{
				ToggleLeaderboard();
			}
			else if (InputManager.Singleton.InputDataCurrent.bToggleArchiveIsland)
			{
				if (AudioManager.singleton != null)
				{
					AudioManager.singleton.PlayMenuClick();
				}
				ToArchiveIsland();
			}
			else if (InputManager.Singleton.InputDataCurrent.bToggleMenu)
			{
				ToggleMenu();
				return;
			}
		}
		if (eUIState == EUIState.ScreenshotMode)
		{
			if (InputManager.Singleton.InputDataCurrent.bUICancel || InputManager.Singleton.InputDataCurrent.bToggleMenu)
			{
				ToggleMenu();
			}
			if (InputManager.Singleton.InputDataCurrent.bUIConfirm)
			{
				SwitchScreenshotMenu();
			}
		}
		if ((eUIState == EUIState.ArchiveIslandSandboxPrompt || eUIState == EUIState.ArchiveIslandHighscorePrompt) && InputManager.Singleton.InputDataCurrent.bUICancel)
		{
			ToMenuWithCurrent();
		}
		if (eUIState == EUIState.NoControllerPrompt && InputManager.Singleton.InputDataCurrent.bUIConfirm)
		{
			ToPrevious(addToStack: false);
		}
		if (eUIState == EUIState.InGamePicking && InputManager.Singleton.InputDataCurrent.bUICancel)
		{
			ToPrevious();
		}
		if (eUIState == EUIState.ArchiveIslandPhotoPrompt && InputManager.Singleton.InputDataCurrent.bUICancel)
		{
			ToArchiveIslandSavePrompt();
		}
		if ((eUIState == EUIState.GameModeSelection || eUIState == EUIState.NewSandboxPrompt) && InputManager.Singleton.InputDataCurrent.bUICancel)
		{
			ToPrevious(addToStack: false);
		}
	}

	public bool IsArchiveIslandOpen()
	{
		if (UIState != EUIState.ArchiveIslandHighscorePrompt && UIState != EUIState.ArchiveIslandSandboxPrompt && UIState != EUIState.ArchiveIslandMaxFile && UIState != EUIState.ArchiveIslandSavePrompt && UIState != EUIState.ArchiveIslandLoadPrompt && UIState != EUIState.ArchiveIslandDeletePrompt && UIState != EUIState.ArchiveislandDeleteConfirmation && UIState != EUIState.ArchiveIslandLoseHighscore)
		{
			return UIState == EUIState.ArchiveIslandLoseSandbox;
		}
		return true;
	}

	public void ToggleLeaderboard()
	{
		if (eUIState == EUIState.Leaderboard)
		{
			ToPrevious(addToStack: false);
		}
		else
		{
			ToLeaderboard();
		}
	}

	public void ToggleMenu()
	{
		if (eUIState == EUIState.ScreenshotMode || eUIState == EUIState.ScreenshotMenu)
		{
			ToPrevious(addToStack: false);
			AudioManager.singleton.PlayMenuClick();
		}
		else if (LocalGameManager.singleton.GameState == LocalGameManager.EGameState.InGame && eUIState != EUIState.InCamTutorial && eUIState != EUIState.InGameTransition && eUIState != EUIState.InGamePicking)
		{
			if (eUIState == EUIState.MenuNoCurrent || eUIState == EUIState.MenuWithCurrent)
			{
				SetUIState(eUIStatePreviousInGame);
			}
			else
			{
				SetUIState(EUIState.MenuWithCurrent);
			}
		}
	}

	public void BlockInput(float seconds)
	{
		StartCoroutine(BlockInputRoutine(seconds));
	}

	private IEnumerator BlockInputRoutine(float seconds)
	{
		detectInput = false;
		yield return new WaitForSeconds(seconds);
		detectInput = true;
	}
}
