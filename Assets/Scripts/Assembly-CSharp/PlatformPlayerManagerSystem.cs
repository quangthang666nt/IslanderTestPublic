using System;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class PlatformPlayerManagerSystem : MonoBehaviour
{
	private static PlatformPlayerManagerSystem s_instance;

	public bool m_Initialised;

	public bool m_Ready;

	public bool m_Connected;

	public bool m_VirtualKeyboardOpened;

	public Color[] m_PlayerColors = new Color[4];

	public string m_PlayerOneName = "P1";

	public string m_PlayerTwoName = "P2";

	public string m_PlayerThreeName = "P3";

	public string m_PlayerFourName = "P4";

	public string m_KeyboardLastText = "";

	public bool m_KeyboardLastCloseResult;

	public string m_PlatformName = "";

	public LoadResult m_LastLoadActionResult = LoadResult.FailOther;

	public int m_EngagedPlayerIndex = -1;

	private Controller m_LastActiveController;

	private bool m_FirstControllerCountChecked;

	private bool m_ControllerDetected = true;

	private float m_TimeSinceLastControllerDetected;

	private bool m_NoControllerEventSent;

	public static PlatformPlayerManagerSystem Instance
	{
		get
		{
			CreateInstance();
			return s_instance;
		}
	}

	public static bool IsReady
	{
		get
		{
			if (s_instance != null)
			{
				return s_instance.m_Ready;
			}
			return false;
		}
	}

	public PlatformPlayerManager PlatformPlayerManager { get; set; }

	public Controller LastActiveController => ReInput.controllers.GetLastActiveController();

	private event Action<Controller> m_OnLastActiveControllerUpdated;

	public event Action<Controller> OnLastActiveControllerUpdated
	{
		add
		{
			m_OnLastActiveControllerUpdated -= value;
			m_OnLastActiveControllerUpdated += value;
		}
		remove
		{
			m_OnLastActiveControllerUpdated -= value;
		}
	}

	private event Action m_OnNoControllerDetected;

	public event Action OnNoControllerDetected
	{
		add
		{
			m_OnNoControllerDetected -= value;
			m_OnNoControllerDetected += value;
		}
		remove
		{
			m_OnNoControllerDetected -= value;
		}
	}

	private event Action m_OnAttemptSavePopupRequest;

	public event Action OnAttemptSavePopupRequest
	{
		add
		{
			m_OnAttemptSavePopupRequest -= value;
			m_OnAttemptSavePopupRequest += value;
		}
		remove
		{
			m_OnAttemptSavePopupRequest -= value;
		}
	}

	private event Action<PlayerConnectionResult.ResultState> m_OnEngagedPlayerConnected;

	public event Action<PlayerConnectionResult.ResultState> OnEngagedPlayerConnected
	{
		add
		{
			m_OnEngagedPlayerConnected -= value;
			m_OnEngagedPlayerConnected += value;
		}
		remove
		{
			m_OnEngagedPlayerConnected -= value;
		}
	}

	private event Action<PlayerConnectionResult.ResultState> m_OnUnexpectedNewPlayerConnected;

	public event Action<PlayerConnectionResult.ResultState> OnUnexpectedNewPlayerConnected
	{
		add
		{
			m_OnUnexpectedNewPlayerConnected -= value;
			m_OnUnexpectedNewPlayerConnected += value;
		}
		remove
		{
			m_OnUnexpectedNewPlayerConnected -= value;
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void CreateInstance()
	{
		if (s_instance == null)
		{
			CommandLineHandler.Initialize();
			GameObject obj = new GameObject("PlatformPlayerManager");
			obj.AddComponent<PlatformPlayerManagerSystem>().Init();
			UnityEngine.Object.DontDestroyOnLoad(obj);
		}
	}

	public void Init()
	{
		if (!m_Initialised)
		{
			InputManager.DisableAllInput();
			s_instance = this;
			InitialisePlayerManager();
			m_Initialised = true;
		}
	}

	private void OnDestroy()
	{
		if (PlatformPlayerManager != null)
		{
			PlatformPlayerManager.OnSystemSuspended -= OnSystemSuspended;
			PlatformPlayerManager.OnSystemResumed -= OnSystemResumed;
			PlatformPlayerManager.OnSystemOutOfFocus -= OnSystemOutOfFocus;
			PlatformPlayerManager.OnSystemFocus -= OnSystemFocus;
			PlatformPlayerManager.OnSystemPanic -= OnSystemPanic;
			PlatformPlayerManager.OnControllerConnected -= OnControllerConnected;
			PlatformPlayerManager.OnControllerDisconnected -= OnControllerDisconnected;
			PlatformPlayerManager.Destroy();
			PlatformPlayerManager = null;
		}
		m_Initialised = false;
		s_instance = null;
	}

	public virtual void InitialisePlayerManager()
	{
		PlatformPlayerManager = new Steam_PlatformPlayerManager();
		PlatformPlayerManager.SetPlayerColors(m_PlayerColors);
		PlatformPlayerManager.Init();
		PlatformPlayerManager.OnSystemSuspended += OnSystemSuspended;
		PlatformPlayerManager.OnSystemResumed += OnSystemResumed;
		PlatformPlayerManager.OnSystemOutOfFocus += OnSystemOutOfFocus;
		PlatformPlayerManager.OnSystemFocus += OnSystemFocus;
		PlatformPlayerManager.OnSystemPanic += OnSystemPanic;
		PlatformPlayerManager.OnControllerConnected += OnControllerConnected;
		PlatformPlayerManager.OnControllerDisconnected += OnControllerDisconnected;
		PlatformPlayerManager.OnSystemNewUser += OnUnexpectedNewUser;
		m_Connected = !PlatformPlayerManager.NeedsPlayerConnection();
		m_Initialised = true;
	}

	public static bool RequiresEngagementScreen()
	{
		if (Instance != null && Instance.PlatformPlayerManager != null)
		{
			return Instance.PlatformPlayerManager.RequiresEngagementScreen();
		}
		return false;
	}

	public void ForceTriggerLastActiveControllerUpdate()
	{
		m_LastActiveController = LastActiveController;
		this.m_OnLastActiveControllerUpdated?.Invoke(LastActiveController);
	}

	private int CheckControllerCount()
	{
		int num = ReInput.controllers.joystickCount + 1;
		num = ReInput.players.GetPlayer(0).controllers.joystickCount;
		if (num == 0 && m_ControllerDetected)
		{
			m_ControllerDetected = false;
			m_NoControllerEventSent = false;
			m_TimeSinceLastControllerDetected = 0f;
		}
		else if (num > 0 && !m_ControllerDetected)
		{
			m_NoControllerEventSent = false;
			m_ControllerDetected = true;
			m_TimeSinceLastControllerDetected = 0f;
		}
		return num;
	}

	private void OnControllerConnected(ControllerStatusChangedEventArgs args)
	{
		CheckControllerCount();
		if (InputManager.Singleton != null)
		{
			InputManager.Singleton.LoadPlatformSpecificMaps(args.controller);
		}
	}

	private void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
	{
		CheckControllerCount();
	}

	private void OnSystemPanic()
	{
		Debug.Log("OnSystemPanic");
	}

	private void OnSystemOutOfFocus()
	{
		Debug.Log("OnSystemOutOfFocus");
	}

	private void OnSystemFocus()
	{
		Debug.Log("OnSystemFocus");
	}

	private void OnSystemSuspended()
	{
		Debug.Log("OnSystemSuspended");
	}

	private void OnSystemResumed()
	{
		Debug.Log("OnSystemResumed");
	}

	private void OnUnexpectedNewUser()
	{
		PlayerConnectionResult.ResultState obj = (GetEngagedPlayerName().Equals("") ? PlayerConnectionResult.ResultState.Failed : PlayerConnectionResult.ResultState.Success);
		if (this.m_OnUnexpectedNewPlayerConnected != null)
		{
			this.m_OnUnexpectedNewPlayerConnected(obj);
		}
		else
		{
			Debug.Log("OnEngagedPlayerConnected == false");
		}
		Debug.Log("NewUserNotSpected");
	}

	public void ResetPlayers()
	{
		PlatformPlayerManager.ResetPlayers();
		m_Connected = false;
	}

	public void SetEngagedPlayerIndex(int index)
	{
		m_EngagedPlayerIndex = index;
	}

	public void ConnectEngagedPlayer()
	{
		m_Ready = false;
		PlatformPlayerManager.ConnectEngagedPlayer(m_EngagedPlayerIndex, EngagedPlayerConnectedCallback);
	}

	private void Update()
	{
		if (PlatformPlayerManager != null)
		{
			PlatformPlayerManager.Update();
		}
		if (!ReInput.isReady)
		{
			return;
		}
		if (!m_FirstControllerCountChecked)
		{
			CheckControllerCount();
			m_FirstControllerCountChecked = true;
		}
		Controller lastActiveController = ReInput.controllers.GetLastActiveController();
		if (m_LastActiveController != lastActiveController)
		{
			m_LastActiveController = lastActiveController;
			if (this.m_OnLastActiveControllerUpdated != null)
			{
				this.m_OnLastActiveControllerUpdated(m_LastActiveController);
			}
			CursorManager.TryEnableCursor(m_LastActiveController.type == ControllerType.Keyboard || m_LastActiveController.type == ControllerType.Mouse);
		}
	}

	private void EngagedPlayerConnectedCallback(PlayerConnectionResult result)
	{
		if (result.Result == PlayerConnectionResult.ResultState.Success)
		{
			m_Connected = true;
			GetEngagedPlayerName();
		}
		else if (result.Result == PlayerConnectionResult.ResultState.Failed)
		{
			m_Connected = false;
		}
		if (this.m_OnEngagedPlayerConnected != null)
		{
			this.m_OnEngagedPlayerConnected(result.Result);
		}
		m_Ready = true;
	}

	public void ConnectPlayer(int index)
	{
		m_Ready = false;
		PlatformPlayerManager.ConnectPlayer(ref index, OnAdditionalPlayerConnected);
	}

	private void OnAdditionalPlayerConnected(PlayerConnectionResult result)
	{
		if (result.Result == PlayerConnectionResult.ResultState.Success)
		{
			GetPlayerName(result.Index);
		}
		m_Ready = true;
	}

	public string GetEngagedPlayerName()
	{
		m_PlayerOneName = PlatformPlayerManager.GetMainPlayerName();
		return m_PlayerOneName;
	}

	public string GetPlayerName(int index)
	{
		string playerName = PlatformPlayerManager.GetPlayerName(index);
		switch (index)
		{
		case 0:
			m_PlayerOneName = playerName;
			break;
		case 1:
			m_PlayerTwoName = playerName;
			break;
		case 2:
			m_PlayerThreeName = playerName;
			break;
		case 3:
			m_PlayerFourName = playerName;
			break;
		}
		return playerName;
	}

	public void UnlockAchievement(AchievementData achievement)
	{
		if (achievement != null)
		{
			PlatformPlayerManager.UnlockAchievement(achievement);
		}
	}

	public void ShowControllerAssignmentPanel()
	{
		PlatformPlayerManager.ShowControllerAssignmentPanel();
	}

	public LoadResult LoadData(string dataName, Action<string, LoadResult, byte[]> loadCallback, int slot = 0)
	{
		m_LastLoadActionResult = PlatformPlayerManager.LoadData(dataName, loadCallback, slot);
		return m_LastLoadActionResult;
	}

	public bool SaveData(string dataName, ref byte[] data, Action<string, bool> saveCallback, int slot = 0)
	{
		return PlatformPlayerManager.SaveData(dataName, data, saveCallback, slot);
	}

	public bool DeleteData(string dataName, Action<string, bool> deleteCallback, int slot = 0)
	{
		return PlatformPlayerManager.DeleteData(dataName, deleteCallback, slot);
	}

	public bool DoesPassProfanityFilter(string text)
	{
		return PlatformPlayerManager.DoesPassProfanityFilter(text);
	}

	public void OpenSystemVirtualKeyboard(string defaultText, string title, string subTitle, int maxTextLength = 25)
	{
		m_Ready = false;
		m_VirtualKeyboardOpened = true;
		PlatformPlayerManager.OpenSystemVirtualKeyboard(defaultText, title, subTitle, VirtualKeyboardClosedCallback, maxTextLength);
	}

	public void VirtualKeyboardClosedCallback(string data, bool completed)
	{
		m_Ready = true;
		m_VirtualKeyboardOpened = false;
		m_KeyboardLastCloseResult = completed;
		m_KeyboardLastText = data;
	}

	public float GetTargetFrameTime()
	{
		return PlatformPlayerManager.GetTargetFrameTime();
	}

	public Vector2 GetTargetResolution()
	{
		return PlatformPlayerManager.GetTargetResolution();
	}

	public bool IsNetworkAvailable(Action<bool> resultCallback = null)
	{
		return PlatformPlayerManager.IsNetworkAvailable(resultCallback);
	}

	public void TriggerAttemptSavePopup()
	{
		if (this.m_OnAttemptSavePopupRequest != null)
		{
			this.m_OnAttemptSavePopupRequest();
		}
	}

	public void OpenStorePageForPromotion()
	{
		if (PlatformPlayerManager != null)
		{
			PlatformPlayerManager.OpenStorePageForPromotion();
		}
	}

	public string GetDefaultLanguage()
	{
		Debug.Log("[Platform] Get Default Language...");
		if (PlatformPlayerManager != null)
		{
			return PlatformPlayerManager.GetDefaultLanguage();
		}
		Debug.Log("[Platform] Platform is not ready!");
		return "English";
	}

	public void SubmitLeaderboardScore(bool connectIfNecessary, int highScore, Action<bool> onHighScoreUpdated = null)
	{
		if (PlatformPlayerManager != null)
		{
			PlatformPlayerManager.SubmitLeaderboardScore(connectIfNecessary, highScore, onHighScoreUpdated);
		}
	}

	public void SetLeaderboardFilter(LeaderboardFilter filter)
	{
		if (PlatformPlayerManager != null)
		{
			PlatformPlayerManager.SetLeaderboardFilterMode(filter);
		}
	}

	public void AddPlayerStat(string stat, int value, Action<bool> onStatUpdated = null)
	{
		if (PlatformPlayerManager != null)
		{
			PlatformPlayerManager.AddPlayerStat(stat, value, onStatUpdated);
		}
	}

	public void UpdateLeaderboardEntriesAsync(bool connectIfNecessary, Action<bool, List<LeaderboardEntry>> entriesCallback)
	{
		if (PlatformPlayerManager != null)
		{
			PlatformPlayerManager.UpdateLeaderboardEntriesAsync(connectIfNecessary, entriesCallback);
		}
	}

	public void UpdatePlayerLeaderboardEntryAsync(bool connectIfNecessary, Action<bool, LeaderboardEntry> entryCallback)
	{
		if (PlatformPlayerManager != null)
		{
			PlatformPlayerManager.UpdatePlayerLeaderboardEntryAsync(connectIfNecessary, entryCallback);
		}
	}

	public void GetCachedLeaderboardEntries(out List<LeaderboardEntry> outEntries)
	{
		outEntries = null;
		if (PlatformPlayerManager != null)
		{
			PlatformPlayerManager.GetCachedLeaderboardEntries(out outEntries);
		}
	}

	public void GetCachedPlayerLeaderboardEntry(out LeaderboardEntry outEntry)
	{
		outEntry = default(LeaderboardEntry);
		if (PlatformPlayerManager != null)
		{
			PlatformPlayerManager.GetCachedPlayerLeaderboardEntry(out outEntry);
		}
	}

	public bool HasLeaderboardAccess()
	{
		if (PlatformPlayerManager != null)
		{
			return PlatformPlayerManager.HasLeaderboardAccess();
		}
		return false;
	}

	public string FormatDateWithCultureDatePattern(DateTime datetime)
	{
		if (PlatformPlayerManager != null)
		{
			return PlatformPlayerManager.FormatDateWithCultureDatePattern(datetime);
		}
		return string.Empty;
	}
}
