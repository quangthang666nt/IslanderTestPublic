using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

[Serializable]
public class SteamLeaderboard
{
	[Serializable]
	public struct SteamLeaderboardEntry
	{
		public string playername;

		public int score;

		public int rank;

		public int id;

		public Texture2D avatar;

		public void GetAvatar(int id)
		{
			if (SteamUtils.GetImageSize(id, out var pnWidth, out var pnHeight) && pnWidth != 0 && pnHeight != 0)
			{
				int num = (int)(pnWidth * pnHeight * 4);
				byte[] array = new byte[num];
				Texture2D texture2D = new Texture2D((int)pnWidth, (int)pnHeight, TextureFormat.RGBA32, mipChain: false, linear: true);
				if (SteamUtils.GetImageRGBA(id, array, num))
				{
					texture2D.LoadRawTextureData(array);
					texture2D.Apply();
					avatar = texture2D;
				}
			}
		}

		public static SteamLeaderboardEntry GetEntryData(LeaderboardEntry_t entry, bool downloadAvatar = false)
		{
			SteamLeaderboardEntry result = default(SteamLeaderboardEntry);
			result.playername = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser);
			result.score = entry.m_nScore;
			result.rank = entry.m_nGlobalRank;
			result.id = (int)(ulong)entry.m_steamIDUser;
			if (downloadAvatar)
			{
				int largeFriendAvatar = SteamFriends.GetLargeFriendAvatar(entry.m_steamIDUser);
				result.GetAvatar(largeFriendAvatar);
			}
			return result;
		}

		public static LeaderboardEntry Convert(SteamLeaderboardEntry entry)
		{
			LeaderboardEntry result = default(LeaderboardEntry);
			result.GlobalRank = entry.rank;
			result.IsPlayer = (int)(ulong)SteamUser.GetSteamID() == entry.id;
			result.Name = entry.playername;
			result.Score = entry.score;
			return result;
		}

		public static List<LeaderboardEntry> GetConversion(SteamLeaderboardEntry[] entries)
		{
			List<LeaderboardEntry> list = new List<LeaderboardEntry>();
			for (int i = 0; i < entries.Length; i++)
			{
				LeaderboardEntry item = Convert(entries[i]);
				list.Add(item);
			}
			return list;
		}
	}

	private const string LEADERBOARD_ID = "LB_HIGHEST_SCORE";

	[Header("Config")]
	[SerializeField]
	private int minVal = -4;

	[SerializeField]
	private int maxVal = 5;

	[Header("Table data")]
	[SerializeField]
	private ELeaderboardDisplayType displayType;

	[SerializeField]
	private ELeaderboardSortMethod shortMethod;

	[SerializeField]
	private SteamLeaderboardEntry playerEntry;

	[SerializeField]
	private List<SteamLeaderboardEntry> entriesPlayers;

	private SteamLeaderboard_t leaderboard;

	private CallResult<LeaderboardFindResult_t> leaderboardCallback;

	private CallResult<LeaderboardScoresDownloaded_t> leaderboardDownload;

	private CallResult<LeaderboardScoresDownloaded_t> playerDownload;

	private CallResult<LeaderboardScoreUploaded_t> leaderboardUploaded;

	public Action<bool, List<LeaderboardEntry>> OnRangeUpdated;

	public Action<bool, LeaderboardEntry> OnPlayerEntryUpdate;

	public Action<bool> OnSubmitPlayerScore;

	private SteamAPICall_t downloadCallback;

	~SteamLeaderboard()
	{
		if (leaderboardCallback != null)
		{
			leaderboardCallback.Cancel();
			leaderboardCallback.Dispose();
		}
		if (leaderboardDownload != null)
		{
			leaderboardDownload.Cancel();
			leaderboardDownload.Dispose();
		}
		if (playerDownload != null)
		{
			playerDownload.Cancel();
			playerDownload.Dispose();
		}
		if (leaderboardUploaded != null)
		{
			leaderboardUploaded.Cancel();
			leaderboardUploaded.Dispose();
		}
	}

	public void DownloadLeaderboard()
	{
		leaderboardCallback = CallResult<LeaderboardFindResult_t>.Create(OnFindedLeaderboardResults);
		SteamAPICall_t hAPICall = SteamUserStats.FindLeaderboard("LB_HIGHEST_SCORE");
		leaderboardCallback.Set(hAPICall);
	}

	public void DownloadEntryData()
	{
		UpdateLeaderboardEntries();
		UpdatePlayerEntry();
	}

	public void SubmitScore(int newScore)
	{
		SteamAPICall_t steamCallback = SteamUserStats.UploadLeaderboardScore(leaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, newScore, null, 0);
		UploadHandler(steamCallback);
	}

	public void SetScoreZero()
	{
		SteamAPICall_t steamCallback = SteamUserStats.UploadLeaderboardScore(leaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, 0, null, 0);
		UploadHandler(steamCallback);
	}

	public void DebugReset()
	{
		SteamUserStats.ResetAllStats(bAchievementsToo: true);
		DownloadEntryData();
	}

	public void SetLeaderboardFilterMode(LeaderboardFilter filter)
	{
		switch (filter)
		{
		case LeaderboardFilter.YourScore:
			downloadCallback = GetEntriesAroundPlayer();
			break;
		case LeaderboardFilter.TopScores:
			downloadCallback = GetTopEntries();
			break;
		case LeaderboardFilter.FriendScore:
			downloadCallback = GetFriendsEntries();
			break;
		}
		UpdateLeaderboardEntries();
	}

	private SteamAPICall_t GetEntriesAroundPlayer()
	{
		return SteamUserStats.DownloadLeaderboardEntries(leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, minVal, maxVal);
	}

	private SteamAPICall_t GetTopEntries(int count = 11)
	{
		return SteamUserStats.DownloadLeaderboardEntries(leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, count);
	}

	private SteamAPICall_t GetFriendsEntries()
	{
		return SteamUserStats.DownloadLeaderboardEntries(leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends, minVal, maxVal);
	}

	private void UpdateLeaderboardEntries()
	{
		leaderboardDownload = CallResult<LeaderboardScoresDownloaded_t>.Create(OnDownloadedScores);
		_ = downloadCallback;
		SteamAPICall_t hAPICall = downloadCallback;
		leaderboardDownload.Set(hAPICall);
	}

	private void UpdatePlayerEntry()
	{
		playerDownload = CallResult<LeaderboardScoresDownloaded_t>.Create(OnPlayerDownloadScore);
		SteamAPICall_t hAPICall = SteamUserStats.DownloadLeaderboardEntriesForUsers(leaderboard, new CSteamID[1] { SteamUser.GetSteamID() }, 1);
		playerDownload.Set(hAPICall);
	}

	private void UploadHandler(SteamAPICall_t steamCallback)
	{
		if (leaderboardUploaded != null && leaderboardUploaded.IsActive())
		{
			leaderboardUploaded.Dispose();
		}
		leaderboardUploaded = CallResult<LeaderboardScoreUploaded_t>.Create(OnSubmitScore);
		leaderboardUploaded.Set(steamCallback);
	}

	private void OnSubmitScore(LeaderboardScoreUploaded_t param, bool bIOFailure)
	{
		if (leaderboardDownload != null && leaderboardDownload.IsActive())
		{
			leaderboardDownload.Dispose();
		}
		leaderboardDownload = CallResult<LeaderboardScoresDownloaded_t>.Create(OnDownloadedScores);
		SteamAPICall_t hAPICall = SteamUserStats.DownloadLeaderboardEntries(leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, -4, 5);
		leaderboardDownload.Set(hAPICall);
		LeaderboardEntry leaderboardEntry = default(LeaderboardEntry);
		leaderboardEntry.GlobalRank = param.m_nGlobalRankNew;
		leaderboardEntry.IsPlayer = true;
		leaderboardEntry.Name = SteamFriends.GetPersonaName();
		leaderboardEntry.Score = param.m_nScore;
		LeaderboardEntry arg = leaderboardEntry;
		OnPlayerEntryUpdate?.Invoke(!bIOFailure, arg);
		OnSubmitPlayerScore?.Invoke(!bIOFailure);
	}

	private void OnFindedLeaderboardResults(LeaderboardFindResult_t param, bool bIOFailure)
	{
		leaderboard = param.m_hSteamLeaderboard;
		shortMethod = SteamUserStats.GetLeaderboardSortMethod(leaderboard);
		displayType = SteamUserStats.GetLeaderboardDisplayType(leaderboard);
		UpdatePlayerEntry();
	}

	private void OnPlayerDownloadScore(LeaderboardScoresDownloaded_t param, bool bIOFailure)
	{
		if (!bIOFailure)
		{
			if (SteamUserStats.GetDownloadedLeaderboardEntry(param.m_hSteamLeaderboardEntries, 0, out var pLeaderboardEntry, null, 0))
			{
				playerEntry = SteamLeaderboardEntry.GetEntryData(pLeaderboardEntry, downloadAvatar: true);
			}
			OnPlayerEntryUpdate?.Invoke(!bIOFailure, SteamLeaderboardEntry.Convert(playerEntry));
		}
	}

	private void OnDownloadedScores(LeaderboardScoresDownloaded_t param, bool bIOFailure)
	{
		if (bIOFailure || param.m_cEntryCount <= 0)
		{
			OnRangeUpdated?.Invoke(arg1: false, null);
			Debug.LogError($"Failed downloading scores failed? {bIOFailure} is not failed player doesn't have a entry");
			return;
		}
		entriesPlayers = new List<SteamLeaderboardEntry>();
		int cEntryCount = param.m_cEntryCount;
		for (int i = 0; i < cEntryCount; i++)
		{
			if (SteamUserStats.GetDownloadedLeaderboardEntry(param.m_hSteamLeaderboardEntries, i, out var pLeaderboardEntry, null, 0))
			{
				SteamLeaderboardEntry entryData = SteamLeaderboardEntry.GetEntryData(pLeaderboardEntry);
				entriesPlayers.Add(entryData);
				if (entryData.id == playerEntry.id)
				{
					playerEntry = entryData;
				}
			}
			else
			{
				Debug.Log($"entry not found index {i}");
			}
		}
		OnRangeUpdated?.Invoke(arg1: true, SteamLeaderboardEntry.GetConversion(entriesPlayers.ToArray()));
	}
}
