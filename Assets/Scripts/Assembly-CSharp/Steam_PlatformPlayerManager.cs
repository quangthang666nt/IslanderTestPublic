using System;
using System.Collections.Generic;
using System.Globalization;
using Steamworks;
using UnityEngine;

public class Steam_PlatformPlayerManager : PlatformPlayerManager
{
	private enum RequestState
	{
		Off = 0,
		InProgress = 1,
		Done = 2,
		Failed = 3
	}

	public const uint STEAM_APP_ID = 1046030u;

	public const uint STEAM_STORE_DEMO_APP_ID = 1046030u;

	private const string LEADERBOARD_NAME = "LB_HIGHEST_SCORE";

	private bool m_Initialised;

	private bool m_MustStoreStats;

	[SerializeField]
	private SteamLeaderboard m_leaderboard = new SteamLeaderboard();

	private List<StandalonePlatformHelpers.LoadRequest> m_LoadRequests = new List<StandalonePlatformHelpers.LoadRequest>();

	private List<StandalonePlatformHelpers.SaveRequest> m_SaveRequests = new List<StandalonePlatformHelpers.SaveRequest>();

	private LeaderboardEntry m_PlayerEntry;

	private List<LeaderboardEntry> m_RangeEntries = new List<LeaderboardEntry>();

	public bool SteamWorksEnabled => true;

	public Steam_PlatformPlayerManager()
	{
		m_Initialised = false;
		if (SteamWorksEnabled)
		{
			m_Initialised = SteamManager.Initialized;
			if (!m_Initialised)
			{
				Debug.Log("SteamManager cannot initialize");
			}
			else
			{
				m_leaderboard.DownloadLeaderboard();
			}
		}
	}

	public override void Destroy()
	{
		_ = m_Initialised;
	}

	public override float GetTargetFrameTime()
	{
		return 16.666f;
	}

	public override Vector2 GetTargetResolution()
	{
		return new Vector2(1920f, 1080f);
	}

	public override string GetPlayerName(int index)
	{
		if (index == 0)
		{
			return GetSteamUserName();
		}
		return GetSteamUserName() + "(" + (index + 1) + ")";
	}

	public override string GetMainPlayerName()
	{
		return GetSteamUserName();
	}

	private string GetSteamUserName()
	{
		if (m_Initialised)
		{
			return SteamFriends.GetPersonaName();
		}
		return "SteamPlayerNameNotFound";
	}

	public override string GetPlatformString()
	{
		return "STEAM";
	}

	public override string GetOnlinePlatformString()
	{
		return "steam";
	}

	public override void Update()
	{
		if (m_Initialised && m_MustStoreStats)
		{
			SteamUserStats.StoreStats();
			m_MustStoreStats = false;
		}
		for (int num = m_LoadRequests.Count - 1; num >= 0; num--)
		{
			if (m_LoadRequests[num].CurrentResult != LoadResult.InProgress)
			{
				Action<string, LoadResult, byte[]> loadCallback = m_LoadRequests[num].LoadCallback;
				string fileName = m_LoadRequests[num].FileName;
				LoadResult currentResult = m_LoadRequests[num].CurrentResult;
				byte[] data = m_LoadRequests[num].Data;
				m_LoadRequests[num].Reset();
				m_LoadRequests.RemoveAt(num);
				loadCallback?.Invoke(fileName, currentResult, data);
			}
		}
		for (int num2 = m_SaveRequests.Count - 1; num2 >= 0; num2--)
		{
			if (m_SaveRequests[num2].Cancelled)
			{
				m_SaveRequests[num2].Reset();
				m_SaveRequests.RemoveAt(num2);
			}
			else if (m_SaveRequests[num2].Done)
			{
				Action<string, bool> saveCallback = m_SaveRequests[num2].SaveCallback;
				string fileName2 = m_SaveRequests[num2].FileName;
				bool done = m_SaveRequests[num2].Done;
				m_SaveRequests[num2].Reset();
				m_SaveRequests.RemoveAt(num2);
				saveCallback?.Invoke(fileName2, done);
			}
		}
	}

	public override void UnlockAchievement(AchievementData achievementData)
	{
		if (m_Initialised)
		{
			SteamUserStats.SetAchievement(achievementData.SteamID);
			SteamUserStats.StoreStats();
		}
	}

	public static void DebugClearAllAchievement()
	{
		if (PlatformPlayerManagerSystem.Instance.PlatformPlayerManager is Steam_PlatformPlayerManager steam_PlatformPlayerManager && steam_PlatformPlayerManager.m_Initialised)
		{
			SteamUserStats.ResetAllStats(bAchievementsToo: true);
		}
	}

	public override bool SaveData(string dataName, byte[] data, Action<string, bool> saveCallback, int slot = 0)
	{
		for (int num = m_SaveRequests.Count - 1; num >= 0; num--)
		{
			if (m_SaveRequests[num].FileName == dataName)
			{
				return true;
			}
		}
		StandalonePlatformHelpers.SaveRequest saveRequest = StandalonePlatformHelpers.SaveData(dataName, data, saveCallback);
		if (!saveRequest.Done)
		{
			m_SaveRequests.Add(saveRequest);
			return m_SaveRequests[m_SaveRequests.Count - 1].Done;
		}
		if (saveRequest.SaveCallback != null)
		{
			saveRequest.SaveCallback(saveRequest.FileName, saveRequest.Done);
		}
		return true;
	}

	public override LoadResult LoadData(string dataName, Action<string, LoadResult, byte[]> loadCallback, int slot = 0)
	{
		StandalonePlatformHelpers.LoadRequest loadRequest = StandalonePlatformHelpers.LoadData(dataName, loadCallback);
		if (loadRequest.CurrentResult == LoadResult.InProgress)
		{
			m_LoadRequests.Add(loadRequest);
			return m_LoadRequests[m_LoadRequests.Count - 1].CurrentResult;
		}
		if (loadRequest.LoadCallback != null)
		{
			loadRequest.LoadCallback(loadRequest.FileName, loadRequest.CurrentResult, loadRequest.Data);
		}
		return loadRequest.CurrentResult;
	}

	public override bool DeleteData(string dataName, Action<string, bool> deleteCallback, int slot = 0)
	{
		return StandalonePlatformHelpers.DeleteData(dataName, deleteCallback);
	}

	public override void OpenStorePageForPromotion()
	{
		Debug.Log("StorePageForPromotion not implemented for steam!");
		SteamFriends.ActivateGameOverlayToStore((AppId_t)1046030u, EOverlayToStoreFlag.k_EOverlayToStoreFlag_None);
	}

	public override void SetLeaderboardFilterMode(LeaderboardFilter filter)
	{
		if (m_leaderboard != null)
		{
			m_leaderboard.SetLeaderboardFilterMode(filter);
		}
	}

	public override void SubmitLeaderboardScore(bool connectIfNecessary, int highScore, Action<bool> onHighScoreUpdated)
	{
		if (m_leaderboard != null)
		{
			m_leaderboard.OnSubmitPlayerScore = onHighScoreUpdated;
			m_leaderboard.SubmitScore(highScore);
		}
	}

	public override void UpdateLeaderboardEntriesAsync(bool connectIfNecessary, Action<bool, List<LeaderboardEntry>> entriesCallback)
	{
		if (m_leaderboard != null)
		{
			m_leaderboard.OnRangeUpdated = entriesCallback;
			m_leaderboard.DownloadEntryData();
		}
	}

	public override void UpdatePlayerLeaderboardEntryAsync(bool connectIfNecessary, Action<bool, LeaderboardEntry> entryCallback)
	{
		if (m_leaderboard != null)
		{
			m_leaderboard.OnPlayerEntryUpdate = entryCallback;
			m_leaderboard.DownloadEntryData();
		}
	}

	public override void GetCachedLeaderboardEntries(out List<LeaderboardEntry> outEntries)
	{
		outEntries = m_RangeEntries;
	}

	public override void GetCachedPlayerLeaderboardEntry(out LeaderboardEntry outEntry)
	{
		outEntry = m_PlayerEntry;
	}

	public override bool HasLeaderboardAccess()
	{
		return m_Initialised;
	}

	public override string FormatDateWithCultureDatePattern(DateTime datetime)
	{
		return datetime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
	}
}
