using System;
using System.Collections.Generic;
using I2.Loc;
using Rewired;
using UnityEngine;

public abstract class PlatformPlayerManager
{
	protected Color[] m_PlayerColors;

	protected event Action m_OnSystemSuspended;

	public event Action OnSystemSuspended
	{
		add
		{
			m_OnSystemSuspended -= value;
			m_OnSystemSuspended += value;
		}
		remove
		{
			m_OnSystemSuspended -= value;
		}
	}

	protected event Action m_OnSystemResumed;

	public event Action OnSystemResumed
	{
		add
		{
			m_OnSystemResumed -= value;
			m_OnSystemResumed += value;
		}
		remove
		{
			m_OnSystemResumed -= value;
		}
	}

	protected event Action m_OnSystemOutOfFocus;

	public event Action OnSystemOutOfFocus
	{
		add
		{
			m_OnSystemOutOfFocus -= value;
			m_OnSystemOutOfFocus += value;
		}
		remove
		{
			m_OnSystemOutOfFocus -= value;
		}
	}

	protected event Action m_OnSystemFocus;

	public event Action OnSystemFocus
	{
		add
		{
			m_OnSystemFocus -= value;
			m_OnSystemFocus += value;
		}
		remove
		{
			m_OnSystemFocus -= value;
		}
	}

	protected event Action m_OnSystemPanic;

	public event Action OnSystemPanic
	{
		add
		{
			m_OnSystemPanic -= value;
			m_OnSystemPanic += value;
		}
		remove
		{
			m_OnSystemPanic -= value;
		}
	}

	protected event Action<ControllerStatusChangedEventArgs> m_OnControllerConnected;

	public event Action<ControllerStatusChangedEventArgs> OnControllerConnected
	{
		add
		{
			m_OnControllerConnected -= value;
			m_OnControllerConnected += value;
		}
		remove
		{
			m_OnControllerConnected -= value;
		}
	}

	protected event Action<ControllerStatusChangedEventArgs> m_OnControllerDisconnected;

	public event Action<ControllerStatusChangedEventArgs> OnControllerDisconnected
	{
		add
		{
			m_OnControllerDisconnected -= value;
			m_OnControllerDisconnected += value;
		}
		remove
		{
			m_OnControllerDisconnected -= value;
		}
	}

	protected event Action m_OnSystemNewUser;

	public event Action OnSystemNewUser
	{
		add
		{
			m_OnSystemNewUser -= value;
			m_OnSystemNewUser += value;
		}
		remove
		{
			m_OnSystemNewUser -= value;
		}
	}

	public void SetPlayerColors(Color[] playerColors)
	{
		m_PlayerColors = playerColors;
	}

	public virtual void Destroy()
	{
		ReInput.ControllerConnectedEvent -= OnRewiredControllerConnected;
		ReInput.ControllerDisconnectedEvent -= OnRewiredControllerDisconnected;
	}

	public virtual string GetDefaultLanguage()
	{
		return LocalizationManager.GetCurrentDeviceLanguage();
	}

	public virtual string GetPlayerName(int index)
	{
		return "PlaceholderName" + index;
	}

	public virtual string GetMainPlayerName()
	{
		return "PlaceholderName0";
	}

	public virtual string GetPlatformString()
	{
		return "None";
	}

	public virtual string GetOnlinePlatformString()
	{
		return "none";
	}

	public virtual float GetTargetFrameTime()
	{
		return 16.666f;
	}

	public virtual Vector2 GetTargetResolution()
	{
		return new Vector2(1920f, 1080f);
	}

	public virtual void UnlockAchievement(AchievementData info)
	{
	}

	public virtual void ResetPlayers()
	{
	}

	public virtual bool NeedsPlayerConnection()
	{
		return false;
	}

	public virtual bool IsUserGuest()
	{
		return false;
	}

	protected virtual void OnRewiredControllerConnected(ControllerStatusChangedEventArgs controllerStatusChangedArgs)
	{
		if (this.m_OnControllerConnected != null)
		{
			this.m_OnControllerConnected(controllerStatusChangedArgs);
		}
	}

	protected virtual void OnRewiredControllerDisconnected(ControllerStatusChangedEventArgs controllerStatusChangedArgs)
	{
		if (this.m_OnControllerDisconnected != null)
		{
			this.m_OnControllerDisconnected(controllerStatusChangedArgs);
		}
	}

	public virtual void Init()
	{
		ReInput.ControllerConnectedEvent += OnRewiredControllerConnected;
		ReInput.ControllerDisconnectedEvent += OnRewiredControllerDisconnected;
	}

	public virtual bool ConnectEngagedPlayer(int index, Action<PlayerConnectionResult> connectedCallback)
	{
		connectedCallback(new PlayerConnectionResult
		{
			Result = PlayerConnectionResult.ResultState.Success
		});
		return true;
	}

	public virtual bool ConnectPlayer(ref int index, Action<PlayerConnectionResult> connectedCallback)
	{
		connectedCallback(new PlayerConnectionResult
		{
			Result = PlayerConnectionResult.ResultState.Success
		});
		return true;
	}

	public virtual void OpenSystemVirtualKeyboard(string defaultText, string title, string subTitle, Action<string, bool> callback, int maxTextLength = 25)
	{
		callback?.Invoke("", arg2: true);
	}

	public virtual void ShowControllerAssignmentPanel()
	{
	}

	public virtual LoadResult LoadData(string dataName, Action<string, LoadResult, byte[]> loadCallback, int slot = 0)
	{
		loadCallback?.Invoke(dataName, LoadResult.FailOther, null);
		return LoadResult.FailOther;
	}

	public virtual bool SaveData(string dataName, byte[] data, Action<string, bool> saveCallback, int slot = 0)
	{
		saveCallback?.Invoke(dataName, arg2: false);
		return false;
	}

	public virtual bool DeleteData(string dataName, Action<string, bool> deleteCallback, int slot = 0)
	{
		throw new NotImplementedException();
	}

	public virtual bool DoesPassProfanityFilter(string text)
	{
		return true;
	}

	public virtual void Update()
	{
	}

	protected void OnSystemSuspendedCall()
	{
		if (this.m_OnSystemSuspended != null)
		{
			this.m_OnSystemSuspended();
		}
	}

	protected void OnSystemResumedCall()
	{
		if (this.m_OnSystemResumed != null)
		{
			this.m_OnSystemResumed();
		}
	}

	public virtual bool RequiresEngagementScreen()
	{
		return false;
	}

	protected void OnSystemOutOfFocusCall()
	{
		if (this.m_OnSystemOutOfFocus != null)
		{
			this.m_OnSystemOutOfFocus();
		}
	}

	protected void OnSystemFocusCall()
	{
		if (this.m_OnSystemFocus != null)
		{
			this.m_OnSystemFocus();
		}
	}

	protected virtual void OnSystemPanicCall()
	{
		if (this.m_OnSystemPanic != null)
		{
			this.m_OnSystemPanic();
		}
	}

	protected virtual void OnSystemNewUserCall()
	{
		if (this.m_OnSystemNewUser != null)
		{
			this.m_OnSystemNewUser();
		}
	}

	public virtual bool IsNetworkAvailable(Action<bool> result = null)
	{
		result?.Invoke(obj: true);
		return true;
	}

	public virtual void OpenStorePageForPromotion()
	{
	}

	public virtual void SubmitLeaderboardScore(bool connectIfNecessary, int highScore, Action<bool> onHighScoreUpdated)
	{
	}

	public virtual void SetLeaderboardFilterMode(LeaderboardFilter filter)
	{
	}

	public virtual void AddPlayerStat(string stat, int value, Action<bool> onStatUpdated)
	{
	}

	public virtual void UpdateLeaderboardEntriesAsync(bool connectIfNecessary, Action<bool, List<LeaderboardEntry>> entriesCallback)
	{
	}

	public virtual void UpdatePlayerLeaderboardEntryAsync(bool connectIfNecessary, Action<bool, LeaderboardEntry> entryCallback)
	{
	}

	public virtual void GetCachedLeaderboardEntries(out List<LeaderboardEntry> outEntries)
	{
		outEntries = null;
	}

	public virtual void GetCachedPlayerLeaderboardEntry(out LeaderboardEntry outEntry)
	{
		outEntry = default(LeaderboardEntry);
	}

	public virtual bool HasLeaderboardAccess()
	{
		return false;
	}

	public virtual string FormatDateWithCultureDatePattern(DateTime datetime)
	{
		return string.Empty;
	}
}
