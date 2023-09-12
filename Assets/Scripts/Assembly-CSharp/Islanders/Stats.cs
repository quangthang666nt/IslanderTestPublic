using System;
using FlatBuffers;

namespace Islanders
{
	public struct Stats : IFlatbufferObject
	{
		private Table __p;

		public ByteBuffer ByteBuffer => __p.bb;

		public bool GlobalStats
		{
			get
			{
				int num = __p.__offset(4);
				if (num == 0)
				{
					return false;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public int HighScore
		{
			get
			{
				int num = __p.__offset(6);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int MaxIslandersReachedInAnyOrThisRun
		{
			get
			{
				int num = __p.__offset(8);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int MaxIslandsReachedInBestRun
		{
			get
			{
				int num = __p.__offset(10);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int MaxBuildingsInInventoryInAnyOrThisRun
		{
			get
			{
				int num = __p.__offset(12);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int TotalNegativePoints
		{
			get
			{
				int num = __p.__offset(14);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int TotalScore
		{
			get
			{
				int num = __p.__offset(16);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int TotalIslandsReached
		{
			get
			{
				int num = __p.__offset(18);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int TotalBuildingsBuilt
		{
			get
			{
				int num = __p.__offset(20);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public float FTotalTimePlayed
		{
			get
			{
				int num = __p.__offset(22);
				if (num == 0)
				{
					return 0f;
				}
				return __p.bb.GetFloat(num + __p.bb_pos);
			}
		}

		public int BuildingsBuilt
		{
			get
			{
				int num = __p.__offset(24);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int ReceivedBuildingsCount
		{
			get
			{
				int num = __p.__offset(26);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int BuildingPacksUnlocked
		{
			get
			{
				int num = __p.__offset(28);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int PlacedBuildingsLength
		{
			get
			{
				int num = __p.__offset(30);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int ScorePerBuildingLength
		{
			get
			{
				int num = __p.__offset(32);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int ReceivedBuildingsLength
		{
			get
			{
				int num = __p.__offset(34);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int RequiredScoreForPackLength
		{
			get
			{
				int num = __p.__offset(36);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int WentToNextIslandAtScoresLength
		{
			get
			{
				int num = __p.__offset(38);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int MilisecondsPerBuildingLength
		{
			get
			{
				int num = __p.__offset(40);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int WentToNextIslandAtMilisecondsLength
		{
			get
			{
				int num = __p.__offset(42);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int CurrentIslandScore
		{
			get
			{
				int num = __p.__offset(44);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int CurrentTowersWellPlaced
		{
			get
			{
				int num = __p.__offset(46);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public static void ValidateVersion()
		{
			FlatBufferConstants.FLATBUFFERS_1_12_0();
		}

		public static Stats GetRootAsStats(ByteBuffer _bb)
		{
			return GetRootAsStats(_bb, default(Stats));
		}

		public static Stats GetRootAsStats(ByteBuffer _bb, Stats obj)
		{
			return obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public static bool StatsBufferHasIdentifier(ByteBuffer _bb)
		{
			return Table.__has_identifier(_bb, "HTF0");
		}

		public void __init(int _i, ByteBuffer _bb)
		{
			__p = new Table(_i, _bb);
		}

		public Stats __assign(int _i, ByteBuffer _bb)
		{
			__init(_i, _bb);
			return this;
		}

		public int PlacedBuildings(int j)
		{
			int num = __p.__offset(30);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetPlacedBuildingsBytes()
		{
			return __p.__vector_as_arraysegment(30);
		}

		public int[] GetPlacedBuildingsArray()
		{
			return __p.__vector_as_array<int>(30);
		}

		public int ScorePerBuilding(int j)
		{
			int num = __p.__offset(32);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetScorePerBuildingBytes()
		{
			return __p.__vector_as_arraysegment(32);
		}

		public int[] GetScorePerBuildingArray()
		{
			return __p.__vector_as_array<int>(32);
		}

		public int ReceivedBuildings(int j)
		{
			int num = __p.__offset(34);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetReceivedBuildingsBytes()
		{
			return __p.__vector_as_arraysegment(34);
		}

		public int[] GetReceivedBuildingsArray()
		{
			return __p.__vector_as_array<int>(34);
		}

		public int RequiredScoreForPack(int j)
		{
			int num = __p.__offset(36);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetRequiredScoreForPackBytes()
		{
			return __p.__vector_as_arraysegment(36);
		}

		public int[] GetRequiredScoreForPackArray()
		{
			return __p.__vector_as_array<int>(36);
		}

		public int WentToNextIslandAtScores(int j)
		{
			int num = __p.__offset(38);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetWentToNextIslandAtScoresBytes()
		{
			return __p.__vector_as_arraysegment(38);
		}

		public int[] GetWentToNextIslandAtScoresArray()
		{
			return __p.__vector_as_array<int>(38);
		}

		public int MilisecondsPerBuilding(int j)
		{
			int num = __p.__offset(40);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public int WentToNextIslandAtMiliseconds(int j)
		{
			int num = __p.__offset(42);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public static Offset<Stats> CreateStats(FlatBufferBuilder builder, bool GlobalStats = false, int HighScore = 0, int MaxIslandersReachedInAnyOrThisRun = 0, int MaxIslandsReachedInBestRun = 0, int MaxBuildingsInInventoryInAnyOrThisRun = 0, int TotalNegativePoints = 0, int TotalScore = 0, int TotalIslandsReached = 0, int TotalBuildingsBuilt = 0, float fTotalTimePlayed = 0f, int BuildingsBuilt = 0, int ReceivedBuildingsCount = 0, int BuildingPacksUnlocked = 0, int CurrentIslandScore = 0, int CurrentTowersWellPlaced = 0, VectorOffset PlacedBuildingsOffset = default(VectorOffset), VectorOffset ScorePerBuildingOffset = default(VectorOffset), VectorOffset MilisecondsPerBuildingOffset = default(VectorOffset), VectorOffset ReceivedBuildingsOffset = default(VectorOffset), VectorOffset RequiredScoreForPackOffset = default(VectorOffset), VectorOffset WentToNextIslandAtScoresOffset = default(VectorOffset), VectorOffset WentToNextIslandAtMilisecondsOffset = default(VectorOffset))
		{
			builder.StartTable(22);
			AddWentToNextIslandAtMiliseconds(builder, WentToNextIslandAtMilisecondsOffset);
			AddWentToNextIslandAtScores(builder, WentToNextIslandAtScoresOffset);
			AddRequiredScoreForPack(builder, RequiredScoreForPackOffset);
			AddReceivedBuildings(builder, ReceivedBuildingsOffset);
			AddMilisecondsPerBuilding(builder, MilisecondsPerBuildingOffset);
			AddScorePerBuilding(builder, ScorePerBuildingOffset);
			AddPlacedBuildings(builder, PlacedBuildingsOffset);
			AddBuildingPacksUnlocked(builder, BuildingPacksUnlocked);
			AddReceivedBuildingsCount(builder, ReceivedBuildingsCount);
			AddBuildingsBuilt(builder, BuildingsBuilt);
			AddFTotalTimePlayed(builder, fTotalTimePlayed);
			AddTotalBuildingsBuilt(builder, TotalBuildingsBuilt);
			AddTotalIslandsReached(builder, TotalIslandsReached);
			AddTotalScore(builder, TotalScore);
			AddTotalNegativePoints(builder, TotalNegativePoints);
			AddMaxBuildingsInInventoryInAnyOrThisRun(builder, MaxBuildingsInInventoryInAnyOrThisRun);
			AddMaxIslandsReachedInBestRun(builder, MaxIslandsReachedInBestRun);
			AddMaxIslandersReachedInAnyOrThisRun(builder, MaxIslandersReachedInAnyOrThisRun);
			AddHighScore(builder, HighScore);
			AddGlobalStats(builder, GlobalStats);
			AddCurrentIslandScore(builder, CurrentIslandScore);
			AddCurrentTowerWellPlaced(builder, CurrentTowersWellPlaced);
			return EndStats(builder);
		}

		public static void StartStats(FlatBufferBuilder builder)
		{
			builder.StartTable(22);
		}

		public static void AddGlobalStats(FlatBufferBuilder builder, bool GlobalStats)
		{
			builder.AddBool(0, GlobalStats, d: false);
		}

		public static void AddHighScore(FlatBufferBuilder builder, int HighScore)
		{
			builder.AddInt(1, HighScore, 0);
		}

		public static void AddMaxIslandersReachedInAnyOrThisRun(FlatBufferBuilder builder, int MaxIslandersReachedInAnyOrThisRun)
		{
			builder.AddInt(2, MaxIslandersReachedInAnyOrThisRun, 0);
		}

		public static void AddMaxIslandsReachedInBestRun(FlatBufferBuilder builder, int MaxIslandsReachedInBestRun)
		{
			builder.AddInt(3, MaxIslandsReachedInBestRun, 0);
		}

		public static void AddMaxBuildingsInInventoryInAnyOrThisRun(FlatBufferBuilder builder, int MaxBuildingsInInventoryInAnyOrThisRun)
		{
			builder.AddInt(4, MaxBuildingsInInventoryInAnyOrThisRun, 0);
		}

		public static void AddTotalNegativePoints(FlatBufferBuilder builder, int TotalNegativePoints)
		{
			builder.AddInt(5, TotalNegativePoints, 0);
		}

		public static void AddTotalScore(FlatBufferBuilder builder, int TotalScore)
		{
			builder.AddInt(6, TotalScore, 0);
		}

		public static void AddTotalIslandsReached(FlatBufferBuilder builder, int TotalIslandsReached)
		{
			builder.AddInt(7, TotalIslandsReached, 0);
		}

		public static void AddTotalBuildingsBuilt(FlatBufferBuilder builder, int TotalBuildingsBuilt)
		{
			builder.AddInt(8, TotalBuildingsBuilt, 0);
		}

		public static void AddFTotalTimePlayed(FlatBufferBuilder builder, float fTotalTimePlayed)
		{
			builder.AddFloat(9, fTotalTimePlayed, 0.0);
		}

		public static void AddBuildingsBuilt(FlatBufferBuilder builder, int BuildingsBuilt)
		{
			builder.AddInt(10, BuildingsBuilt, 0);
		}

		public static void AddReceivedBuildingsCount(FlatBufferBuilder builder, int ReceivedBuildingsCount)
		{
			builder.AddInt(11, ReceivedBuildingsCount, 0);
		}

		public static void AddBuildingPacksUnlocked(FlatBufferBuilder builder, int BuildingPacksUnlocked)
		{
			builder.AddInt(12, BuildingPacksUnlocked, 0);
		}

		public static void AddPlacedBuildings(FlatBufferBuilder builder, VectorOffset PlacedBuildingsOffset)
		{
			builder.AddOffset(13, PlacedBuildingsOffset.Value, 0);
		}

		public static VectorOffset CreatePlacedBuildingsVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreatePlacedBuildingsVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartPlacedBuildingsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddScorePerBuilding(FlatBufferBuilder builder, VectorOffset ScorePerBuildingOffset)
		{
			builder.AddOffset(14, ScorePerBuildingOffset.Value, 0);
		}

		public static VectorOffset CreateScorePerBuildingVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateScorePerBuildingVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartScorePerBuildingVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddReceivedBuildings(FlatBufferBuilder builder, VectorOffset ReceivedBuildingsOffset)
		{
			builder.AddOffset(15, ReceivedBuildingsOffset.Value, 0);
		}

		public static VectorOffset CreateReceivedBuildingsVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateReceivedBuildingsVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartReceivedBuildingsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddRequiredScoreForPack(FlatBufferBuilder builder, VectorOffset RequiredScoreForPackOffset)
		{
			builder.AddOffset(16, RequiredScoreForPackOffset.Value, 0);
		}

		public static VectorOffset CreateRequiredScoreForPackVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateRequiredScoreForPackVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartRequiredScoreForPackVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddWentToNextIslandAtScores(FlatBufferBuilder builder, VectorOffset WentToNextIslandAtScoresOffset)
		{
			builder.AddOffset(17, WentToNextIslandAtScoresOffset.Value, 0);
		}

		public static VectorOffset CreateWentToNextIslandAtScoresVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateWentToNextIslandAtScoresVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartWentToNextIslandAtScoresVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void StartMilisecondsPerBuildingVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddMilisecondsPerBuilding(FlatBufferBuilder builder, VectorOffset MilisecondsPerBuildingOffset)
		{
			builder.AddOffset(18, MilisecondsPerBuildingOffset.Value, 0);
		}

		public static void StartWentToNextIslandAtMilisecondsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddWentToNextIslandAtMiliseconds(FlatBufferBuilder builder, VectorOffset WentToNextIslandAtMilisecondsOffset)
		{
			builder.AddOffset(19, WentToNextIslandAtMilisecondsOffset.Value, 0);
		}

		public static void AddCurrentIslandScore(FlatBufferBuilder builder, int CurrentIslandScore)
		{
			builder.AddInt(20, CurrentIslandScore, 0);
		}

		public static void AddCurrentTowerWellPlaced(FlatBufferBuilder builder, int CurrentTowerWellPlaced)
		{
			builder.AddInt(21, CurrentTowerWellPlaced, 0);
		}

		public static Offset<Stats> EndStats(FlatBufferBuilder builder)
		{
			return new Offset<Stats>(builder.EndTable());
		}

		public static void FinishStatsBuffer(FlatBufferBuilder builder, Offset<Stats> offset)
		{
			builder.Finish(offset.Value, "HTF0");
		}

		public static void FinishSizePrefixedStatsBuffer(FlatBufferBuilder builder, Offset<Stats> offset)
		{
			builder.FinishSizePrefixed(offset.Value, "HTF0");
		}
	}
}
