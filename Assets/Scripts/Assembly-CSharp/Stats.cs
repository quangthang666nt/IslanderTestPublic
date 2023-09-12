using System;
using System.Collections.Generic;
using System.Linq;
using FlatBuffers;
using Islanders;
using SCS.Gameplay;
using UnityEngine;

[Serializable]
public class Stats
{
	public enum EAchievement
	{
		SCORE_MATCH_500 = 0,
		SCORE_MATCH_1500 = 1,
		SCORE_MATCH_3000 = 2,
		ISLAND_REACH_2 = 3,
		ISLAND_REACH_3 = 4,
		ISLAND_REACH_4 = 5,
		BUILDING_WITH_25_PLUS_POINTS = 6,
		BUILDING_WITH_50_PLUS_POINTS = 7,
		BUILDING_WITH_75_PLUS_POINTS = 8,
		HAVE_12_OR_MORE_BUILDINGS_IN_INVENTORY = 9,
		FINISH_FIRST_MATCH = 10,
		SCORE_TOTAL_2000 = 11,
		SCORE_TOTAL_10000 = 12,
		SCORE_TOTAL_20000 = 13,
		BUILDINGS_TOTAL_100 = 14,
		BUILDINGS_TOTAL_500 = 15,
		BUILDINGS_TOTAL_1000 = 16,
		ISLANDS_TOTAL_10 = 17,
		ISLANDS_TOTAL_20 = 18,
		ISLANDS_TOTAL_30 = 19,
		SPEEDRUN_FIRST_ISLAND_IN_150_SEC = 20,
		SPEEDRUN_SECOND_ISLAND_IN_420_SEC = 21,
		SCORE_EXACTLY_100 = 22,
		GET_A_TOTAL_OF_100_MINUS_POINTS_IN_ONE_MATCH = 23,
		SCORE_1000_WITHOUT_MORE_THAN_6_BUILDINGS = 24,
		SCORE_FIRST_ISLAND_800 = 25,
		ISLAND_REACH_10 = 26,
		ISLAND_REACH_21 = 27,
		PHOTO_ALL_COSMETICS = 28,
		SCORE_MATCH_10000 = 29,
		ARCHIVE_ISLAND_10_BUILDINGS = 30,
		SCORE_15000_BEFORE_11_ISLAND = 31,
		BUILDINGS_200_TYPES_10_SANDBOX = 32,
		ALL_BUILDINGS_HIGHSCORE = 33,
		SHAMANS_3_ONE_NIGHT = 34,
		SCORE_TOTAL_30000 = 35,
		SCORE_2000_SNOW_ISLAND = 36,
		TOWERS_4_NO_LOSS_POINTS = 37
	}

	public bool bGlobalStats;

	private bool bSnowIsland;

	public int iHighscore;

	public int iMaxIslandsReachedInAnyOrThisRun;

	public int iMaxIslandsReachedInBestRun;

	public int iMaxBuildingsInInventoryInAnyOrThisRun;

	public int iTotalNegativePoints;

	public int iTotalScore;

	public int iTotalIslandsReached;

	public int iTotalBuildingsBuilt;

	public int iCurrentIslandStartScore;

	public float fTotalTimePlayed;

	public int iBuildingsBuilt;

	public int iReceivedBuildings;

	public int iBuildingPacksUnlocked;

	private int iLastNumberOfBuildings;

	private int iLastNumberOfIslands;

	private int iShamansPlacedThisNight;

	private int iTowersPlaced;

	public List<int> liIPlacedBuildings = new List<int>();

	public List<int> liIScorePerBuilding = new List<int>();

	public List<KeyValuePair<int, int>> liIScoreAdditionPerCloseBuilding = new List<KeyValuePair<int, int>>();

	public List<int> liIMilisecondsPerBuilding = new List<int>();

	public List<int> liIReceivedBuildings = new List<int>();

	public List<int> liIRequiredScoreForPack = new List<int>();

	public List<int> liIWentToNextIslandAtScores = new List<int>();

	public List<int> liIWentToNextIslandAtMiliseconds = new List<int>();

	private const int STATS_FILE_CHECK = 922992;

	private const int STATS_FILE_VERSION = 2;

	private const int kShamanID = 79;

	private const int kTowerID = 66;

	public bool SnowIsland
	{
		set
		{
			bSnowIsland = value;
			iCurrentIslandStartScore = iHighscore;
		}
	}

	public Stats(bool _bGlobalStats = false)
	{
		bGlobalStats = _bGlobalStats;
		if (!_bGlobalStats)
		{
			iMaxIslandsReachedInAnyOrThisRun = 1;
			iTotalIslandsReached = 1;
		}
	}
	public void MergeWithStats(Stats _stats)
	{
		if (_stats.iHighscore > iHighscore)
		{
			iHighscore = _stats.iHighscore;
			liIPlacedBuildings = new List<int>(_stats.liIPlacedBuildings);
			liIScorePerBuilding = new List<int>(_stats.liIScorePerBuilding);
			liIMilisecondsPerBuilding = new List<int>(_stats.liIMilisecondsPerBuilding);
			liIReceivedBuildings = new List<int>(_stats.liIReceivedBuildings);
			liIRequiredScoreForPack = new List<int>(_stats.liIRequiredScoreForPack);
			liIWentToNextIslandAtScores = new List<int>(_stats.liIWentToNextIslandAtScores);
			liIWentToNextIslandAtMiliseconds = new List<int>(_stats.liIWentToNextIslandAtMiliseconds);
			iReceivedBuildings = _stats.iReceivedBuildings;
			iBuildingPacksUnlocked = _stats.iBuildingPacksUnlocked;
			iMaxIslandsReachedInBestRun = _stats.iMaxIslandsReachedInAnyOrThisRun;
			iBuildingsBuilt = _stats.iBuildingsBuilt;
		}
		iMaxIslandsReachedInAnyOrThisRun = Mathf.Max(iMaxIslandsReachedInAnyOrThisRun, _stats.iMaxIslandsReachedInAnyOrThisRun);
		iMaxBuildingsInInventoryInAnyOrThisRun = Mathf.Max(iMaxBuildingsInInventoryInAnyOrThisRun, _stats.iMaxBuildingsInInventoryInAnyOrThisRun);
		iTotalScore += _stats.iTotalScore;
		iTotalIslandsReached += _stats.iTotalIslandsReached;
		iTotalBuildingsBuilt += _stats.iTotalBuildingsBuilt;
		fTotalTimePlayed += _stats.fTotalTimePlayed;
		iTotalNegativePoints += _stats.iTotalNegativePoints;
	}

	public void CheckForAchievements()
	{
		if (iTotalScore >= 2000)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_TOTAL_2000);
		}
		if (iTotalScore >= 10000)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_TOTAL_10000);
		}
		if (iTotalScore >= 20000)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_TOTAL_20000);
		}
		if (iTotalScore >= 30000)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_TOTAL_30000);
		}
		if (iTotalBuildingsBuilt >= 100)
		{
			StatsManager.UnlockAchievementTest(EAchievement.BUILDINGS_TOTAL_100);
		}
		if (iTotalBuildingsBuilt >= 500)
		{
			StatsManager.UnlockAchievementTest(EAchievement.BUILDINGS_TOTAL_500);
		}
		if (iTotalBuildingsBuilt >= 1000)
		{
			StatsManager.UnlockAchievementTest(EAchievement.BUILDINGS_TOTAL_1000);
		}
		if (iTotalIslandsReached >= 10)
		{
			StatsManager.UnlockAchievementTest(EAchievement.ISLANDS_TOTAL_10);
		}
		if (iTotalIslandsReached >= 20)
		{
			StatsManager.UnlockAchievementTest(EAchievement.ISLANDS_TOTAL_20);
		}
		if (iTotalIslandsReached >= 30)
		{
			StatsManager.UnlockAchievementTest(EAchievement.ISLANDS_TOTAL_30);
		}
		if (bGlobalStats)
		{
			return;
		}
		bool num = iLastNumberOfBuildings < liIScorePerBuilding.Count;
		if (iLastNumberOfIslands != iMaxIslandsReachedInAnyOrThisRun)
		{
			iTowersPlaced = 0;
			iShamansPlacedThisNight = 0;
		}
		if (iHighscore >= 500)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_MATCH_500);
		}
		if (iHighscore >= 1500)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_MATCH_1500);
		}
		if (iHighscore >= 3000)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_MATCH_3000);
		}
		if (iHighscore >= 10000)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_MATCH_10000);
		}
		if (iHighscore - iCurrentIslandStartScore >= 2000 && bSnowIsland)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_2000_SNOW_ISLAND);
		}
		if (iHighscore >= 15000 && iMaxIslandsReachedInAnyOrThisRun < 11)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_15000_BEFORE_11_ISLAND);
		}
		if (iMaxIslandsReachedInAnyOrThisRun >= 2 && fTotalTimePlayed <= 150f)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SPEEDRUN_FIRST_ISLAND_IN_150_SEC);
		}
		if (iMaxIslandsReachedInAnyOrThisRun >= 3 && fTotalTimePlayed <= 420f)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SPEEDRUN_SECOND_ISLAND_IN_420_SEC);
		}
		if (iMaxIslandsReachedInAnyOrThisRun >= 2)
		{
			StatsManager.UnlockAchievementTest(EAchievement.ISLAND_REACH_2);
		}
		if (iMaxIslandsReachedInAnyOrThisRun >= 3)
		{
			StatsManager.UnlockAchievementTest(EAchievement.ISLAND_REACH_3);
		}
		if (iMaxIslandsReachedInAnyOrThisRun >= 4)
		{
			StatsManager.UnlockAchievementTest(EAchievement.ISLAND_REACH_4);
		}
		if (iMaxIslandsReachedInAnyOrThisRun >= 10)
		{
			StatsManager.UnlockAchievementTest(EAchievement.ISLAND_REACH_10);
		}
		if (iMaxIslandsReachedInAnyOrThisRun >= 21)
		{
			StatsManager.UnlockAchievementTest(EAchievement.ISLAND_REACH_21);
		}
		if (iHighscore == 100)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_EXACTLY_100);
		}
		if (iTotalNegativePoints >= 100)
		{
			StatsManager.UnlockAchievementTest(EAchievement.GET_A_TOTAL_OF_100_MINUS_POINTS_IN_ONE_MATCH);
		}
		if (num && DayNightCycle.Instance != null && (DayNightCycle.Instance.DayState == DNCycleParameters.EDayState.Night || DayNightCycle.Instance.DayState == DNCycleParameters.EDayState.Sunset))
		{
			if (liIPlacedBuildings[liIPlacedBuildings.Count - 1] == 79)
			{
				iShamansPlacedThisNight++;
				if (iShamansPlacedThisNight == 3)
				{
					StatsManager.UnlockAchievementTest(EAchievement.SHAMANS_3_ONE_NIGHT);
				}
			}
		}
		else
		{
			iShamansPlacedThisNight = 0;
		}
		if (num && liIPlacedBuildings[liIPlacedBuildings.Count - 1] == 66)
		{
			bool flag = true;
			foreach (KeyValuePair<int, int> item in liIScoreAdditionPerCloseBuilding)
			{
				if (item.Key == 66 && item.Value < 0)
				{
					flag = false;
				}
			}
			if (flag)
			{
				iTowersPlaced++;
			}
			if (iTowersPlaced == 4)
			{
				StatsManager.UnlockAchievementTest(EAchievement.TOWERS_4_NO_LOSS_POINTS);
			}
		}
		if (StatsManager.BuildingsIDs.Count != 0 && StatsManager.BuildingsIDs.Count <= liIPlacedBuildings.Count)
		{
			bool flag2 = true;
			foreach (int buildingsID in StatsManager.BuildingsIDs)
			{
				if (!liIPlacedBuildings.Contains(buildingsID))
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				StatsManager.UnlockAchievementTest(EAchievement.ALL_BUILDINGS_HIGHSCORE);
			}
		}
		if (liIScorePerBuilding.Count > 0 && liIScorePerBuilding[liIScorePerBuilding.Count - 1] != -1)
		{
			if (liIScorePerBuilding[liIScorePerBuilding.Count - 1] >= 25)
			{
				StatsManager.UnlockAchievementTest(EAchievement.BUILDING_WITH_25_PLUS_POINTS);
			}
			if (liIScorePerBuilding[liIScorePerBuilding.Count - 1] >= 50)
			{
				StatsManager.UnlockAchievementTest(EAchievement.BUILDING_WITH_50_PLUS_POINTS);
			}
			if (liIScorePerBuilding[liIScorePerBuilding.Count - 1] >= 75)
			{
				StatsManager.UnlockAchievementTest(EAchievement.BUILDING_WITH_75_PLUS_POINTS);
			}
		}
		if (iMaxBuildingsInInventoryInAnyOrThisRun >= 12)
		{
			StatsManager.UnlockAchievementTest(EAchievement.HAVE_12_OR_MORE_BUILDINGS_IN_INVENTORY);
		}
		if (iHighscore >= 1000 && iMaxBuildingsInInventoryInAnyOrThisRun <= 6)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_1000_WITHOUT_MORE_THAN_6_BUILDINGS);
		}
		if (iHighscore >= 800 && iMaxIslandsReachedInAnyOrThisRun <= 1)
		{
			StatsManager.UnlockAchievementTest(EAchievement.SCORE_FIRST_ISLAND_800);
		}
		iLastNumberOfBuildings = liIScorePerBuilding.Count;
		iLastNumberOfIslands = iMaxIslandsReachedInAnyOrThisRun;
	}

	public void CheckForAchievementsSandbox()
	{
		Debug.Log("[Stats] - CheckForAchievementsSandbox Count: " + liIPlacedBuildings.Count);
		if (liIPlacedBuildings.Count > 200 && (from id in liIPlacedBuildings
			group id by id).Count() >= 10)
		{
			StatsManager.UnlockAchievementTest(EAchievement.BUILDINGS_200_TYPES_10_SANDBOX);
		}
	}

	public void OnSavedArchive()
	{
		Debug.Log("[Stats] - On Saved Archive Count: " + liIPlacedBuildings.Count);
		if (liIPlacedBuildings.Count >= 10)
		{
			StatsManager.UnlockAchievementTest(EAchievement.ARCHIVE_ISLAND_10_BUILDINGS);
		}
	}

	public void CheckPhotoModeAchievements(string filter, string frame, string print)
	{
		if (!filter.Equals("None") && !filter.Equals("") && !frame.Equals("None") && !frame.Equals("") && !print.Equals("None") && !print.Equals(""))
		{
			StatsManager.UnlockAchievementTest(EAchievement.PHOTO_ALL_COSMETICS);
		}
	}

	public Offset<Islanders.Stats> ToFlatBuffer(FlatBufferBuilder builder)
	{
		VectorOffset? vectorOffset = SaveGame.WriteIntVectorToFlatBuffer(builder, liIPlacedBuildings, Islanders.Stats.StartPlacedBuildingsVector);
		VectorOffset? vectorOffset2 = SaveGame.WriteIntVectorToFlatBuffer(builder, liIScorePerBuilding, Islanders.Stats.StartScorePerBuildingVector);
		VectorOffset? vectorOffset3 = SaveGame.WriteIntVectorToFlatBuffer(builder, liIMilisecondsPerBuilding, Islanders.Stats.StartMilisecondsPerBuildingVector);
		VectorOffset? vectorOffset4 = SaveGame.WriteIntVectorToFlatBuffer(builder, liIReceivedBuildings, Islanders.Stats.StartReceivedBuildingsVector);
		VectorOffset? vectorOffset5 = SaveGame.WriteIntVectorToFlatBuffer(builder, liIRequiredScoreForPack, Islanders.Stats.StartRequiredScoreForPackVector);
		VectorOffset? vectorOffset6 = SaveGame.WriteIntVectorToFlatBuffer(builder, liIWentToNextIslandAtScores, Islanders.Stats.StartWentToNextIslandAtScoresVector);
		VectorOffset? vectorOffset7 = SaveGame.WriteIntVectorToFlatBuffer(builder, liIWentToNextIslandAtMiliseconds, Islanders.Stats.StartWentToNextIslandAtMilisecondsVector);
		Islanders.Stats.StartStats(builder);
		Islanders.Stats.AddGlobalStats(builder, bGlobalStats);
		Islanders.Stats.AddHighScore(builder, iHighscore);
		Islanders.Stats.AddMaxIslandersReachedInAnyOrThisRun(builder, iMaxIslandsReachedInAnyOrThisRun);
		Islanders.Stats.AddMaxIslandsReachedInBestRun(builder, iMaxIslandsReachedInBestRun);
		Islanders.Stats.AddMaxBuildingsInInventoryInAnyOrThisRun(builder, iMaxBuildingsInInventoryInAnyOrThisRun);
		Islanders.Stats.AddTotalNegativePoints(builder, iTotalNegativePoints);
		Islanders.Stats.AddTotalScore(builder, iTotalScore);
		Islanders.Stats.AddTotalIslandsReached(builder, iTotalIslandsReached);
		Islanders.Stats.AddTotalBuildingsBuilt(builder, iTotalBuildingsBuilt);
		Islanders.Stats.AddFTotalTimePlayed(builder, fTotalTimePlayed);
		Islanders.Stats.AddBuildingsBuilt(builder, iBuildingsBuilt);
		Islanders.Stats.AddReceivedBuildingsCount(builder, iReceivedBuildings);
		Islanders.Stats.AddBuildingPacksUnlocked(builder, iBuildingPacksUnlocked);
		if (vectorOffset.HasValue)
		{
			Islanders.Stats.AddPlacedBuildings(builder, vectorOffset.Value);
		}
		if (vectorOffset2.HasValue)
		{
			Islanders.Stats.AddScorePerBuilding(builder, vectorOffset2.Value);
		}
		if (vectorOffset3.HasValue)
		{
			Islanders.Stats.AddMilisecondsPerBuilding(builder, vectorOffset3.Value);
		}
		if (vectorOffset4.HasValue)
		{
			Islanders.Stats.AddReceivedBuildings(builder, vectorOffset4.Value);
		}
		if (vectorOffset5.HasValue)
		{
			Islanders.Stats.AddRequiredScoreForPack(builder, vectorOffset5.Value);
		}
		if (vectorOffset6.HasValue)
		{
			Islanders.Stats.AddWentToNextIslandAtScores(builder, vectorOffset6.Value);
		}
		if (vectorOffset7.HasValue)
		{
			Islanders.Stats.AddWentToNextIslandAtMiliseconds(builder, vectorOffset7.Value);
		}
		Islanders.Stats.AddCurrentIslandScore(builder, iCurrentIslandStartScore);
		Islanders.Stats.AddCurrentTowerWellPlaced(builder, iTowersPlaced);
		return Islanders.Stats.EndStats(builder);
	}

	public void FromFlatBuffer(Islanders.Stats statsBuffer)
	{
		bGlobalStats = statsBuffer.GlobalStats;
		iHighscore = statsBuffer.HighScore;
		iMaxIslandsReachedInAnyOrThisRun = statsBuffer.MaxIslandersReachedInAnyOrThisRun;
		iMaxIslandsReachedInBestRun = statsBuffer.MaxIslandsReachedInBestRun;
		iMaxBuildingsInInventoryInAnyOrThisRun = statsBuffer.MaxBuildingsInInventoryInAnyOrThisRun;
		iTotalNegativePoints = statsBuffer.TotalNegativePoints;
		iTotalScore = statsBuffer.TotalScore;
		iTotalIslandsReached = statsBuffer.TotalIslandsReached;
		iTotalBuildingsBuilt = statsBuffer.TotalBuildingsBuilt;
		fTotalTimePlayed = statsBuffer.FTotalTimePlayed;
		iBuildingsBuilt = statsBuffer.BuildingsBuilt;
		iReceivedBuildings = statsBuffer.ReceivedBuildingsCount;
		iBuildingPacksUnlocked = statsBuffer.BuildingPacksUnlocked;
		iCurrentIslandStartScore = statsBuffer.CurrentIslandScore;
		iTowersPlaced = statsBuffer.CurrentTowersWellPlaced;
		iLastNumberOfIslands = iMaxIslandsReachedInAnyOrThisRun;
		SaveGame.ReadIntVectorFromFlatBuffer(ref liIPlacedBuildings, statsBuffer.PlacedBuildingsLength, ((Islanders.Stats)statsBuffer).PlacedBuildings);
		SaveGame.ReadIntVectorFromFlatBuffer(ref liIScorePerBuilding, statsBuffer.ScorePerBuildingLength, ((Islanders.Stats)statsBuffer).ScorePerBuilding);
		SaveGame.ReadIntVectorFromFlatBuffer(ref liIMilisecondsPerBuilding, statsBuffer.MilisecondsPerBuildingLength, ((Islanders.Stats)statsBuffer).MilisecondsPerBuilding);
		SaveGame.ReadIntVectorFromFlatBuffer(ref liIReceivedBuildings, statsBuffer.ReceivedBuildingsLength, ((Islanders.Stats)statsBuffer).ReceivedBuildings);
		SaveGame.ReadIntVectorFromFlatBuffer(ref liIRequiredScoreForPack, statsBuffer.RequiredScoreForPackLength, ((Islanders.Stats)statsBuffer).RequiredScoreForPack);
		SaveGame.ReadIntVectorFromFlatBuffer(ref liIWentToNextIslandAtScores, statsBuffer.WentToNextIslandAtScoresLength, ((Islanders.Stats)statsBuffer).WentToNextIslandAtScores);
		SaveGame.ReadIntVectorFromFlatBuffer(ref liIWentToNextIslandAtMiliseconds, statsBuffer.WentToNextIslandAtMilisecondsLength, ((Islanders.Stats)statsBuffer).WentToNextIslandAtMiliseconds);
		CheckEnhancedLists();
	}

	private void CheckEnhancedLists()
	{
		if (liIMilisecondsPerBuilding.Count == 0)
		{
			for (int i = 0; i < liIPlacedBuildings.Count; i++)
			{
				liIMilisecondsPerBuilding.Add(-1);
			}
		}
		if (liIWentToNextIslandAtMiliseconds.Count == 0)
		{
			for (int j = 0; j < liIWentToNextIslandAtScores.Count; j++)
			{
				liIWentToNextIslandAtMiliseconds.Add(-1);
			}
		}
	}
}
