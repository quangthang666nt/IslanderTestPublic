using System;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardSystem
{
	private class ScoreSubmissionAttempt
	{
		public bool InProgress;

		public int Score = -1;

		public List<Action<bool>> OnSubmittedCallbacks = new List<Action<bool>>();
	}

	private class PlayerEntryUpdateAttempt
	{
		public bool InProgress;

		public List<Action<bool, LeaderboardEntry>> OnPlayerEntryUpdatedCallback = new List<Action<bool, LeaderboardEntry>>();
	}

	private class RangeEntriesUpdateAttempt
	{
		public bool InProgress;

		public List<Action<bool, List<LeaderboardEntry>>> OnRangeEntriesUpdatedCallback = new List<Action<bool, List<LeaderboardEntry>>>();
	}

	private static LeaderboardSystem s_Instance;

	private ScoreSubmissionAttempt m_ScoreSubmissionAttempt = new ScoreSubmissionAttempt();

	private PlayerEntryUpdateAttempt m_PlayerEntryUpdateAttempt = new PlayerEntryUpdateAttempt();

	private RangeEntriesUpdateAttempt m_RangeEntriesUpdateAttempt = new RangeEntriesUpdateAttempt();

	private int m_LastScoreSubmitted = -1;

	private float m_MinimumDelayBetweenRangeEntriesRequest = 10f;

	private float m_MinimumDelayBetweenPlayerEntryRequest = 10f;

	private float m_LastTimeRangeEntriesUpdate = -100f;

	private float m_LastTimePlayerEntryUpdated = -100f;

	private bool m_RequestDelayEnabled;

	private LeaderboardEntry m_LastPlayerEntryUpdate;

	private List<LeaderboardEntry> m_LastLeaderboardEntriesUpdate = new List<LeaderboardEntry>();

	private static LeaderboardSystem Instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = new LeaderboardSystem();
			}
			return s_Instance;
		}
	}

	private LeaderboardSystem()
	{
	}

	public static void SubmitScore(bool connectIfNecessary, int highScore, Action<bool> onSubmitted)
	{
		if (Instance.m_LastScoreSubmitted >= highScore)
		{
			onSubmitted?.Invoke(obj: true);
			return;
		}
		if (onSubmitted != null)
		{
			Instance.m_ScoreSubmissionAttempt.OnSubmittedCallbacks.Add(onSubmitted);
		}
		if (!Instance.m_ScoreSubmissionAttempt.InProgress)
		{
			Instance.m_ScoreSubmissionAttempt.InProgress = true;
			Instance.m_ScoreSubmissionAttempt.Score = highScore;
			if (PlatformPlayerManagerSystem.Instance != null)
			{
				PlatformPlayerManagerSystem.Instance.SubmitLeaderboardScore(connectIfNecessary, highScore, OnScoreSubmitted);
			}
		}
	}

	public static void ResetInstance()
	{
		s_Instance = new LeaderboardSystem();
	}

	private static void OnScoreSubmitted(bool success)
	{
		Instance.m_ScoreSubmissionAttempt.InProgress = false;
		if (success)
		{
			Instance.m_LastScoreSubmitted = Instance.m_ScoreSubmissionAttempt.Score;
			for (int i = 0; i < Instance.m_ScoreSubmissionAttempt.OnSubmittedCallbacks.Count; i++)
			{
				Instance.m_ScoreSubmissionAttempt.OnSubmittedCallbacks[i](obj: true);
			}
		}
		else
		{
			for (int j = 0; j < Instance.m_ScoreSubmissionAttempt.OnSubmittedCallbacks.Count; j++)
			{
				Instance.m_ScoreSubmissionAttempt.OnSubmittedCallbacks[j](obj: false);
			}
		}
		Instance.m_ScoreSubmissionAttempt.OnSubmittedCallbacks.Clear();
	}

	public static void GetRangeEntries(bool connectIfNecessary, Action<bool, List<LeaderboardEntry>> onEntriesUpdated)
	{
		if (Instance.m_RequestDelayEnabled && Time.timeSinceLevelLoad - Instance.m_LastTimeRangeEntriesUpdate <= Instance.m_MinimumDelayBetweenRangeEntriesRequest)
		{
			onEntriesUpdated?.Invoke(arg1: true, Instance.m_LastLeaderboardEntriesUpdate);
			return;
		}
		if (onEntriesUpdated != null)
		{
			Instance.m_RangeEntriesUpdateAttempt.OnRangeEntriesUpdatedCallback.Add(onEntriesUpdated);
		}
		if (!Instance.m_RangeEntriesUpdateAttempt.InProgress)
		{
			Instance.m_RangeEntriesUpdateAttempt.InProgress = true;
			if (PlatformPlayerManagerSystem.Instance != null)
			{
				PlatformPlayerManagerSystem.Instance.UpdateLeaderboardEntriesAsync(connectIfNecessary, OnRangeEntriesUpdated);
			}
		}
	}

	private static void OnRangeEntriesUpdated(bool success, List<LeaderboardEntry> rangeEntries)
	{
		Instance.m_RangeEntriesUpdateAttempt.InProgress = false;
		if (success)
		{
			Instance.m_LastLeaderboardEntriesUpdate.Clear();
			for (int i = 0; i < rangeEntries.Count; i++)
			{
				Instance.m_LastLeaderboardEntriesUpdate.Add(rangeEntries[i]);
			}
			Instance.m_LastTimeRangeEntriesUpdate = Time.timeSinceLevelLoad;
			for (int j = 0; j < Instance.m_RangeEntriesUpdateAttempt.OnRangeEntriesUpdatedCallback.Count; j++)
			{
				Instance.m_RangeEntriesUpdateAttempt.OnRangeEntriesUpdatedCallback[j](arg1: true, Instance.m_LastLeaderboardEntriesUpdate);
			}
		}
		else
		{
			for (int k = 0; k < Instance.m_RangeEntriesUpdateAttempt.OnRangeEntriesUpdatedCallback.Count; k++)
			{
				Instance.m_RangeEntriesUpdateAttempt.OnRangeEntriesUpdatedCallback[k](arg1: false, Instance.m_LastLeaderboardEntriesUpdate);
			}
		}
		Instance.m_RangeEntriesUpdateAttempt.OnRangeEntriesUpdatedCallback.Clear();
	}

	public static void GetPlayerEntry(bool connectIfNecessary, Action<bool, LeaderboardEntry> onPlayerEntryUpdated)
	{
		if (Time.timeSinceLevelLoad - Instance.m_LastTimePlayerEntryUpdated <= Instance.m_MinimumDelayBetweenPlayerEntryRequest)
		{
			onPlayerEntryUpdated?.Invoke(arg1: true, Instance.m_LastPlayerEntryUpdate);
			return;
		}
		if (onPlayerEntryUpdated != null)
		{
			Instance.m_PlayerEntryUpdateAttempt.OnPlayerEntryUpdatedCallback.Add(onPlayerEntryUpdated);
		}
		if (!Instance.m_PlayerEntryUpdateAttempt.InProgress)
		{
			Instance.m_PlayerEntryUpdateAttempt.InProgress = true;
			if (PlatformPlayerManagerSystem.Instance != null)
			{
				PlatformPlayerManagerSystem.Instance.UpdatePlayerLeaderboardEntryAsync(connectIfNecessary, OnPlayerEntryUpdated);
			}
		}
	}

	private static void OnPlayerEntryUpdated(bool success, LeaderboardEntry playerEntry)
	{
		Instance.m_PlayerEntryUpdateAttempt.InProgress = false;
		if (success)
		{
			Instance.m_LastPlayerEntryUpdate = playerEntry;
			Instance.m_LastTimePlayerEntryUpdated = Time.timeSinceLevelLoad;
			for (int i = 0; i < Instance.m_PlayerEntryUpdateAttempt.OnPlayerEntryUpdatedCallback.Count; i++)
			{
				Instance.m_PlayerEntryUpdateAttempt.OnPlayerEntryUpdatedCallback[i](arg1: true, Instance.m_LastPlayerEntryUpdate);
			}
		}
		else
		{
			for (int j = 0; j < Instance.m_PlayerEntryUpdateAttempt.OnPlayerEntryUpdatedCallback.Count; j++)
			{
				Instance.m_PlayerEntryUpdateAttempt.OnPlayerEntryUpdatedCallback[j](arg1: false, Instance.m_LastPlayerEntryUpdate);
			}
		}
		Instance.m_PlayerEntryUpdateAttempt.OnPlayerEntryUpdatedCallback.Clear();
	}
}
