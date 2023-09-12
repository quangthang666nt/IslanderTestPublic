using System;
using System.Collections.Generic;
using FlatBuffers;
using Islanders;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
	private const int LEADERBOARD_SCORE_LIMIT = 10000000;

	public static Stats statsMatch = new Stats();

	public static Stats statsGlobal = new Stats(_bGlobalStats: true);

	[SerializeField]
	private GoList buildingsInGame;

	private static List<int> buildingsIDs = new List<int>();

	[SerializeField]
	private EventObject filterNameEventObject;

	[SerializeField]
	private EventObject frameNameEventObject;

	[SerializeField]
	private EventObject printNameEventObject;

	private string currentFilter = "None";

	private string currentFrame = "None";

	private string currentPrint = "None";

	private static string FILE_EXTENSION = ".stats";

	private static string FILE_NAME = "StatsV06";

	[Tooltip("These statistics are saved in the normal save file together with everything else. They reset whenever you start a new game.")]
	public Stats statsTestDisplayMatch = statsMatch;

	[Tooltip("These statistics are saved in a seperate file and are never reset. They accumulate over time.")]
	public Stats statsTestDisplayGlobal = statsGlobal;

	private static List<Stats.EAchievement> liITestSaveUnlockedAchievements = new List<Stats.EAchievement>();

	public static List<int> BuildingsIDs => buildingsIDs;

	public static void OnGameStart()
	{
	}

	public static void OnMatchEnd()
	{
		statsGlobal.MergeWithStats(statsMatch);
		statsMatch.CheckForAchievements();
		statsGlobal.CheckForAchievements();
		if (statsMatch.iTotalScore > 0)
		{
			UnlockAchievementTest(Stats.EAchievement.FINISH_FIRST_MATCH);
		}
		SaveGlobalStats();
	}

	private static void SaveGlobalStats()
	{
		FlatBufferBuilder flatBufferBuilder = SaveLoadManager.singleton.m_FlatBufferBuilder;
		flatBufferBuilder.Clear();
		Offset<Islanders.Stats> offset = statsGlobal.ToFlatBuffer(flatBufferBuilder);
		Islanders.Stats.FinishStatsBuffer(flatBufferBuilder, offset);
		byte[] data = flatBufferBuilder.DataBuffer.ToSizedArray();
		PlatformPlayerManagerSystem.Instance.SaveData(StrSaveFileNameToFullPath(FILE_NAME), ref data, OnGlobalStatsSaved);
	}

	private static void OnGlobalStatsSaved(string fileName, bool result)
	{
		if (result)
		{
			Debug.Log("[StatsManager] Saved " + fileName + " successfully!");
		}
		else
		{
			Debug.Log("[StatsManager] Failed to saved " + fileName + "!");
		}
	}

	public static void LoadGlobalStats()
	{
		PlatformPlayerManagerSystem.Instance.LoadData(StrSaveFileNameToFullPath(FILE_NAME), OnGlobalStatsLoaded);
	}

	private static void OnGlobalStatsLoaded(string fileName, LoadResult result, byte[] data)
	{
		if (result == LoadResult.Success)
		{
			ByteBuffer bb = new ByteBuffer(data);
			if (!SaveData.SaveDataBufferHasIdentifier(bb))
			{
				Debug.LogWarning("[StatsManager] Couldn't find identifier in stats buffer!");
				return;
			}
			Islanders.Stats rootAsStats = Islanders.Stats.GetRootAsStats(bb);
			statsGlobal.FromFlatBuffer(rootAsStats);
		}
		else
		{
			statsGlobal = new Stats();
		}
		TutorialManager.AfterLoad();
	}

	public static bool BDoStatsSurviveSanityCheck(Stats _statsCheck = null)
	{
		if (_statsCheck == null)
		{
			_statsCheck = statsMatch;
			return true;
		}
		return _statsCheck.iHighscore != statsMatch.iHighscore;
	}

	public static bool CheckHighScoreValidity(int score)
	{
		if (score >= 10000000)
		{
			return false;
		}
		return true;
	}

	public static string StrSaveFileNameToFullPath(string _strName)
	{
		return _strName + FILE_EXTENSION;
	}

	private void Awake()
	{
		foreach (GameObject item in buildingsInGame.liGo)
		{
			StructureID component = item.GetComponent<StructureID>();
			if (component != null && !buildingsIDs.Contains(component.iID))
			{
				buildingsIDs.Add(component.iID);
			}
		}
		EventObject eventObject = filterNameEventObject;
		eventObject.objectEvent = (EventObject.ObjectEvent)Delegate.Combine(eventObject.objectEvent, new EventObject.ObjectEvent(OnNewFilterEvent));
		EventObject eventObject2 = frameNameEventObject;
		eventObject2.objectEvent = (EventObject.ObjectEvent)Delegate.Combine(eventObject2.objectEvent, new EventObject.ObjectEvent(OnNewFrameEvent));
		EventObject eventObject3 = printNameEventObject;
		eventObject3.objectEvent = (EventObject.ObjectEvent)Delegate.Combine(eventObject3.objectEvent, new EventObject.ObjectEvent(OnNewPrintEvent));
	}

	private void Update()
	{
		statsTestDisplayMatch = statsMatch;
		statsTestDisplayGlobal = statsGlobal;
	}

	public static void UnlockAchievementTest(Stats.EAchievement _eAchievement)
	{
		AchievementData achievementData = null;
		for (int i = 0; i < AchievementData.AllAchievements.Length; i++)
		{
			if (AchievementData.AllAchievements[i].Achievement == _eAchievement)
			{
				achievementData = AchievementData.AllAchievements[i];
				break;
			}
		}
		if (achievementData != null)
		{
			PlatformPlayerManagerSystem.Instance.UnlockAchievement(achievementData);
			if (!liITestSaveUnlockedAchievements.Contains(_eAchievement))
			{
				liITestSaveUnlockedAchievements.Add(_eAchievement);
			}
		}
	}

	private void OnNewFilterEvent(object newFilterName)
	{
		currentFilter = newFilterName as string;
		statsGlobal.CheckPhotoModeAchievements(currentFilter, currentFrame, currentPrint);
	}

	private void OnNewFrameEvent(object newFrameName)
	{
		currentFrame = newFrameName as string;
		statsGlobal.CheckPhotoModeAchievements(currentFilter, currentFrame, currentPrint);
	}

	private void OnNewPrintEvent(object newPrintName)
	{
		currentPrint = newPrintName as string;
		statsGlobal.CheckPhotoModeAchievements(currentFilter, currentFrame, currentPrint);
	}
}
