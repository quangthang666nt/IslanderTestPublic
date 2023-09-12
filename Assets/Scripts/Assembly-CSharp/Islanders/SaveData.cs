using System;
using FlatBuffers;

namespace Islanders
{
	public struct SaveData : IFlatbufferObject
	{
		private Table __p;

		public ByteBuffer ByteBuffer => __p.bb;

		public int IStructIdsLength
		{
			get
			{
				int num = __p.__offset(4);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int SeriTransformsOfStructIdsLength
		{
			get
			{
				int num = __p.__offset(6);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int IPlaceableIdsLength
		{
			get
			{
				int num = __p.__offset(8);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int SeriTransOfPlaceablesLength
		{
			get
			{
				int num = __p.__offset(10);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int DebugLinearIndexIslandGenerator
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

		public string RandomStateIslandCreation
		{
			get
			{
				int num = __p.__offset(14);
				if (num == 0)
				{
					return null;
				}
				return __p.__string(num + __p.bb_pos);
			}
		}

		public int IIslandsInThisRunLength
		{
			get
			{
				int num = __p.__offset(16);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int CurrentIslandGenID
		{
			get
			{
				int num = __p.__offset(18);
				if (num == 0)
				{
					return -1;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public string RandomStateBeforeColorGen
		{
			get
			{
				int num = __p.__offset(20);
				if (num == 0)
				{
					return null;
				}
				return __p.__string(num + __p.bb_pos);
			}
		}

		public int GameVersion
		{
			get
			{
				int num = __p.__offset(22);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int Score
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

		public int IBuildingInventoryLength
		{
			get
			{
				int num = __p.__offset(26);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int RequiredScoreForNextPack
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

		public int RequiredScoreForLastPack
		{
			get
			{
				int num = __p.__offset(30);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int UnlockedBoosterPacks
		{
			get
			{
				int num = __p.__offset(32);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int RequiredScoreForNextIsland
		{
			get
			{
				int num = __p.__offset(34);
				if (num == 0)
				{
					return -1;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int ScoreWhenEnteredThisIsland
		{
			get
			{
				int num = __p.__offset(36);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int CurrentActiveIsland
		{
			get
			{
				int num = __p.__offset(38);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int LastGottenBuildingAmountSave
		{
			get
			{
				int num = __p.__offset(40);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int IPlusBuildingButtonsIncludingBuildingCountsLength
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

		public int GameMode
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

		public bool ViewingArchivedIsland
		{
			get
			{
				int num = __p.__offset(46);
				if (num == 0)
				{
					return false;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public bool BBrainShuffled
		{
			get
			{
				int num = __p.__offset(48);
				if (num == 0)
				{
					return false;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public int IBBPackUnlockableRemainingLength
		{
			get
			{
				int num = __p.__offset(50);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int GoUnlockedBuildingsLength
		{
			get
			{
				int num = __p.__offset(52);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int GoRemainingLength
		{
			get
			{
				int num = __p.__offset(54);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int GoGuaranteedNextLength
		{
			get
			{
				int num = __p.__offset(56);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int IPlacedBuildingsLength
		{
			get
			{
				int num = __p.__offset(58);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int IPlacedBuildingsAmountLength
		{
			get
			{
				int num = __p.__offset(60);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public string RandomStateNextBuildingChoice
		{
			get
			{
				int num = __p.__offset(62);
				if (num == 0)
				{
					return null;
				}
				return __p.__string(num + __p.bb_pos);
			}
		}

		public int IBuildingButtonBuildingsLength
		{
			get
			{
				int num = __p.__offset(64);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int IBuildingButtonVariationsLength
		{
			get
			{
				int num = __p.__offset(66);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int IBuildingButtonVariationsNextLength
		{
			get
			{
				int num = __p.__offset(68);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public int IBBPacksCurrentChoiceLength
		{
			get
			{
				int num = __p.__offset(70);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public Stats? StatsMatch
		{
			get
			{
				int num = __p.__offset(72);
				if (num == 0)
				{
					return null;
				}
				return default(Stats).__assign(__p.__indirect(num + __p.bb_pos), __p.bb);
			}
		}

		public static void ValidateVersion()
		{
			FlatBufferConstants.FLATBUFFERS_1_12_0();
		}

		public static SaveData GetRootAsSaveData(ByteBuffer _bb)
		{
			return GetRootAsSaveData(_bb, default(SaveData));
		}

		public static SaveData GetRootAsSaveData(ByteBuffer _bb, SaveData obj)
		{
			return obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public static bool SaveDataBufferHasIdentifier(ByteBuffer _bb)
		{
			return Table.__has_identifier(_bb, "HTF0");
		}

		public void __init(int _i, ByteBuffer _bb)
		{
			__p = new Table(_i, _bb);
		}

		public SaveData __assign(int _i, ByteBuffer _bb)
		{
			__init(_i, _bb);
			return this;
		}

		public int IStructIds(int j)
		{
			int num = __p.__offset(4);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetIStructIdsBytes()
		{
			return __p.__vector_as_arraysegment(4);
		}

		public int[] GetIStructIdsArray()
		{
			return __p.__vector_as_array<int>(4);
		}

		public Transform? SeriTransformsOfStructIds(int j)
		{
			int num = __p.__offset(6);
			if (num == 0)
			{
				return null;
			}
			return default(Transform).__assign(__p.__vector(num) + j * 44, __p.bb);
		}

		public int IPlaceableIds(int j)
		{
			int num = __p.__offset(8);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetIPlaceableIdsBytes()
		{
			return __p.__vector_as_arraysegment(8);
		}

		public int[] GetIPlaceableIdsArray()
		{
			return __p.__vector_as_array<int>(8);
		}

		public Transform? SeriTransOfPlaceables(int j)
		{
			int num = __p.__offset(10);
			if (num == 0)
			{
				return null;
			}
			return default(Transform).__assign(__p.__vector(num) + j * 44, __p.bb);
		}

		public ArraySegment<byte>? GetRandomStateIslandCreationBytes()
		{
			return __p.__vector_as_arraysegment(14);
		}

		public byte[] GetRandomStateIslandCreationArray()
		{
			return __p.__vector_as_array<byte>(14);
		}

		public int IIslandsInThisRun(int j)
		{
			int num = __p.__offset(16);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetIIslandsInThisRunBytes()
		{
			return __p.__vector_as_arraysegment(16);
		}

		public int[] GetIIslandsInThisRunArray()
		{
			return __p.__vector_as_array<int>(16);
		}

		public ArraySegment<byte>? GetRandomStateBeforeColorGenBytes()
		{
			return __p.__vector_as_arraysegment(20);
		}

		public byte[] GetRandomStateBeforeColorGenArray()
		{
			return __p.__vector_as_array<byte>(20);
		}

		public int IBuildingInventory(int j)
		{
			int num = __p.__offset(26);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetIBuildingInventoryBytes()
		{
			return __p.__vector_as_arraysegment(26);
		}

		public int[] GetIBuildingInventoryArray()
		{
			return __p.__vector_as_array<int>(26);
		}

		public int IPlusBuildingButtonsIncludingBuildingCounts(int j)
		{
			int num = __p.__offset(42);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetIPlusBuildingButtonsIncludingBuildingCountsBytes()
		{
			return __p.__vector_as_arraysegment(42);
		}

		public int[] GetIPlusBuildingButtonsIncludingBuildingCountsArray()
		{
			return __p.__vector_as_array<int>(42);
		}

		public int IBBPackUnlockableRemaining(int j)
		{
			int num = __p.__offset(50);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetIBBPackUnlockableRemainingBytes()
		{
			return __p.__vector_as_arraysegment(50);
		}

		public int[] GetIBBPackUnlockableRemainingArray()
		{
			return __p.__vector_as_array<int>(50);
		}

		public int GoUnlockedBuildings(int j)
		{
			int num = __p.__offset(52);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetGoUnlockedBuildingsBytes()
		{
			return __p.__vector_as_arraysegment(52);
		}

		public int[] GetGoUnlockedBuildingsArray()
		{
			return __p.__vector_as_array<int>(52);
		}

		public int GoRemaining(int j)
		{
			int num = __p.__offset(54);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetGoRemainingBytes()
		{
			return __p.__vector_as_arraysegment(54);
		}

		public int[] GetGoRemainingArray()
		{
			return __p.__vector_as_array<int>(54);
		}

		public int GoGuaranteedNext(int j)
		{
			int num = __p.__offset(56);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetGoGuaranteedNextBytes()
		{
			return __p.__vector_as_arraysegment(56);
		}

		public int[] GetGoGuaranteedNextArray()
		{
			return __p.__vector_as_array<int>(56);
		}

		public int IPlacedBuildings(int j)
		{
			int num = __p.__offset(58);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetIPlacedBuildingsBytes()
		{
			return __p.__vector_as_arraysegment(58);
		}

		public int[] GetIPlacedBuildingsArray()
		{
			return __p.__vector_as_array<int>(58);
		}

		public int IPlacedBuildingsAmount(int j)
		{
			int num = __p.__offset(60);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetIPlacedBuildingsAmountBytes()
		{
			return __p.__vector_as_arraysegment(60);
		}

		public int[] GetIPlacedBuildingsAmountArray()
		{
			return __p.__vector_as_array<int>(60);
		}

		public ArraySegment<byte>? GetRandomStateNextBuildingChoiceBytes()
		{
			return __p.__vector_as_arraysegment(62);
		}

		public byte[] GetRandomStateNextBuildingChoiceArray()
		{
			return __p.__vector_as_array<byte>(62);
		}

		public int IBuildingButtonBuildings(int j)
		{
			int num = __p.__offset(64);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetIBuildingButtonBuildingsBytes()
		{
			return __p.__vector_as_arraysegment(64);
		}

		public int[] GetIBuildingButtonBuildingsArray()
		{
			return __p.__vector_as_array<int>(64);
		}

		public int IBuildingButtonVariations(int j)
		{
			int num = __p.__offset(66);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetIBuildingButtonVariationsBytes()
		{
			return __p.__vector_as_arraysegment(66);
		}

		public int[] GetIBuildingButtonVariationsArray()
		{
			return __p.__vector_as_array<int>(66);
		}

		public int IBuildingButtonVariationsNext(int j)
		{
			int num = __p.__offset(68);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetIBuildingButtonVariationsNextBytes()
		{
			return __p.__vector_as_arraysegment(68);
		}

		public int[] GetIBuildingButtonVariationsNextArray()
		{
			return __p.__vector_as_array<int>(68);
		}

		public int IBBPacksCurrentChoice(int j)
		{
			int num = __p.__offset(70);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetInt(__p.__vector(num) + j * 4);
		}

		public ArraySegment<byte>? GetIBBPacksCurrentChoiceBytes()
		{
			return __p.__vector_as_arraysegment(70);
		}

		public int[] GetIBBPacksCurrentChoiceArray()
		{
			return __p.__vector_as_array<int>(70);
		}

		public static Offset<SaveData> CreateSaveData(FlatBufferBuilder builder, VectorOffset IStructIdsOffset = default(VectorOffset), VectorOffset SeriTransformsOfStructIdsOffset = default(VectorOffset), VectorOffset IPlaceableIdsOffset = default(VectorOffset), VectorOffset SeriTransOfPlaceablesOffset = default(VectorOffset), int DebugLinearIndexIslandGenerator = 0, StringOffset randomStateIslandCreationOffset = default(StringOffset), VectorOffset IIslandsInThisRunOffset = default(VectorOffset), int CurrentIslandGenID = -1, StringOffset randomStateBeforeColorGenOffset = default(StringOffset), int GameVersion = 0, int Score = 0, VectorOffset IBuildingInventoryOffset = default(VectorOffset), int RequiredScoreForNextPack = 0, int RequiredScoreForLastPack = 0, int UnlockedBoosterPacks = 0, int RequiredScoreForNextIsland = -1, int ScoreWhenEnteredThisIsland = 0, int CurrentActiveIsland = 0, int LastGottenBuildingAmountSave = 0, VectorOffset IPlusBuildingButtonsIncludingBuildingCountsOffset = default(VectorOffset), int GameMode = 0, bool ViewingArchivedIsland = false, bool BBrainShuffled = false, VectorOffset IBBPackUnlockableRemainingOffset = default(VectorOffset), VectorOffset GoUnlockedBuildingsOffset = default(VectorOffset), VectorOffset GoRemainingOffset = default(VectorOffset), VectorOffset GoGuaranteedNextOffset = default(VectorOffset), VectorOffset IPlacedBuildingsOffset = default(VectorOffset), VectorOffset IPlacedBuildingsAmountOffset = default(VectorOffset), StringOffset randomStateNextBuildingChoiceOffset = default(StringOffset), VectorOffset IBuildingButtonBuildingsOffset = default(VectorOffset), VectorOffset IBuildingButtonVariationsOffset = default(VectorOffset), VectorOffset IBuildingButtonVariationsNextOffset = default(VectorOffset), VectorOffset IBBPacksCurrentChoiceOffset = default(VectorOffset), Offset<Stats> StatsMatchOffset = default(Offset<Stats>))
		{
			builder.StartTable(35);
			AddStatsMatch(builder, StatsMatchOffset);
			AddIBBPacksCurrentChoice(builder, IBBPacksCurrentChoiceOffset);
			AddIBuildingButtonVariationsNext(builder, IBuildingButtonVariationsNextOffset);
			AddIBuildingButtonVariations(builder, IBuildingButtonVariationsOffset);
			AddIBuildingButtonBuildings(builder, IBuildingButtonBuildingsOffset);
			AddRandomStateNextBuildingChoice(builder, randomStateNextBuildingChoiceOffset);
			AddIPlacedBuildingsAmount(builder, IPlacedBuildingsAmountOffset);
			AddIPlacedBuildings(builder, IPlacedBuildingsOffset);
			AddGoGuaranteedNext(builder, GoGuaranteedNextOffset);
			AddGoRemaining(builder, GoRemainingOffset);
			AddGoUnlockedBuildings(builder, GoUnlockedBuildingsOffset);
			AddIBBPackUnlockableRemaining(builder, IBBPackUnlockableRemainingOffset);
			AddGameMode(builder, GameMode);
			AddIPlusBuildingButtonsIncludingBuildingCounts(builder, IPlusBuildingButtonsIncludingBuildingCountsOffset);
			AddLastGottenBuildingAmountSave(builder, LastGottenBuildingAmountSave);
			AddCurrentActiveIsland(builder, CurrentActiveIsland);
			AddScoreWhenEnteredThisIsland(builder, ScoreWhenEnteredThisIsland);
			AddRequiredScoreForNextIsland(builder, RequiredScoreForNextIsland);
			AddUnlockedBoosterPacks(builder, UnlockedBoosterPacks);
			AddRequiredScoreForLastPack(builder, RequiredScoreForLastPack);
			AddRequiredScoreForNextPack(builder, RequiredScoreForNextPack);
			AddIBuildingInventory(builder, IBuildingInventoryOffset);
			AddScore(builder, Score);
			AddGameVersion(builder, GameVersion);
			AddRandomStateBeforeColorGen(builder, randomStateBeforeColorGenOffset);
			AddCurrentIslandGenID(builder, CurrentIslandGenID);
			AddIIslandsInThisRun(builder, IIslandsInThisRunOffset);
			AddRandomStateIslandCreation(builder, randomStateIslandCreationOffset);
			AddDebugLinearIndexIslandGenerator(builder, DebugLinearIndexIslandGenerator);
			AddSeriTransOfPlaceables(builder, SeriTransOfPlaceablesOffset);
			AddIPlaceableIds(builder, IPlaceableIdsOffset);
			AddSeriTransformsOfStructIds(builder, SeriTransformsOfStructIdsOffset);
			AddIStructIds(builder, IStructIdsOffset);
			AddBBrainShuffled(builder, BBrainShuffled);
			AddViewingArchivedIsland(builder, ViewingArchivedIsland);
			return EndSaveData(builder);
		}

		public static void StartSaveData(FlatBufferBuilder builder)
		{
			builder.StartTable(35);
		}

		public static void AddIStructIds(FlatBufferBuilder builder, VectorOffset IStructIdsOffset)
		{
			builder.AddOffset(0, IStructIdsOffset.Value, 0);
		}

		public static VectorOffset CreateIStructIdsVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateIStructIdsVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartIStructIdsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddSeriTransformsOfStructIds(FlatBufferBuilder builder, VectorOffset SeriTransformsOfStructIdsOffset)
		{
			builder.AddOffset(1, SeriTransformsOfStructIdsOffset.Value, 0);
		}

		public static void StartSeriTransformsOfStructIdsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(44, numElems, 4);
		}

		public static void AddIPlaceableIds(FlatBufferBuilder builder, VectorOffset IPlaceableIdsOffset)
		{
			builder.AddOffset(2, IPlaceableIdsOffset.Value, 0);
		}

		public static VectorOffset CreateIPlaceableIdsVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateIPlaceableIdsVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartIPlaceableIdsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddSeriTransOfPlaceables(FlatBufferBuilder builder, VectorOffset SeriTransOfPlaceablesOffset)
		{
			builder.AddOffset(3, SeriTransOfPlaceablesOffset.Value, 0);
		}

		public static void StartSeriTransOfPlaceablesVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(44, numElems, 4);
		}

		public static void AddDebugLinearIndexIslandGenerator(FlatBufferBuilder builder, int DebugLinearIndexIslandGenerator)
		{
			builder.AddInt(4, DebugLinearIndexIslandGenerator, 0);
		}

		public static void AddRandomStateIslandCreation(FlatBufferBuilder builder, StringOffset randomStateIslandCreationOffset)
		{
			builder.AddOffset(5, randomStateIslandCreationOffset.Value, 0);
		}

		public static void AddIIslandsInThisRun(FlatBufferBuilder builder, VectorOffset IIslandsInThisRunOffset)
		{
			builder.AddOffset(6, IIslandsInThisRunOffset.Value, 0);
		}

		public static VectorOffset CreateIIslandsInThisRunVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateIIslandsInThisRunVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartIIslandsInThisRunVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddCurrentIslandGenID(FlatBufferBuilder builder, int CurrentIslandGenID)
		{
			builder.AddInt(7, CurrentIslandGenID, -1);
		}

		public static void AddRandomStateBeforeColorGen(FlatBufferBuilder builder, StringOffset randomStateBeforeColorGenOffset)
		{
			builder.AddOffset(8, randomStateBeforeColorGenOffset.Value, 0);
		}

		public static void AddGameVersion(FlatBufferBuilder builder, int GameVersion)
		{
			builder.AddInt(9, GameVersion, 0);
		}

		public static void AddScore(FlatBufferBuilder builder, int Score)
		{
			builder.AddInt(10, Score, 0);
		}

		public static void AddIBuildingInventory(FlatBufferBuilder builder, VectorOffset IBuildingInventoryOffset)
		{
			builder.AddOffset(11, IBuildingInventoryOffset.Value, 0);
		}

		public static VectorOffset CreateIBuildingInventoryVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateIBuildingInventoryVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartIBuildingInventoryVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddRequiredScoreForNextPack(FlatBufferBuilder builder, int RequiredScoreForNextPack)
		{
			builder.AddInt(12, RequiredScoreForNextPack, 0);
		}

		public static void AddRequiredScoreForLastPack(FlatBufferBuilder builder, int RequiredScoreForLastPack)
		{
			builder.AddInt(13, RequiredScoreForLastPack, 0);
		}

		public static void AddUnlockedBoosterPacks(FlatBufferBuilder builder, int UnlockedBoosterPacks)
		{
			builder.AddInt(14, UnlockedBoosterPacks, 0);
		}

		public static void AddRequiredScoreForNextIsland(FlatBufferBuilder builder, int RequiredScoreForNextIsland)
		{
			builder.AddInt(15, RequiredScoreForNextIsland, -1);
		}

		public static void AddScoreWhenEnteredThisIsland(FlatBufferBuilder builder, int ScoreWhenEnteredThisIsland)
		{
			builder.AddInt(16, ScoreWhenEnteredThisIsland, 0);
		}

		public static void AddCurrentActiveIsland(FlatBufferBuilder builder, int CurrentActiveIsland)
		{
			builder.AddInt(17, CurrentActiveIsland, 0);
		}

		public static void AddLastGottenBuildingAmountSave(FlatBufferBuilder builder, int LastGottenBuildingAmountSave)
		{
			builder.AddInt(18, LastGottenBuildingAmountSave, 0);
		}

		public static void AddIPlusBuildingButtonsIncludingBuildingCounts(FlatBufferBuilder builder, VectorOffset IPlusBuildingButtonsIncludingBuildingCountsOffset)
		{
			builder.AddOffset(19, IPlusBuildingButtonsIncludingBuildingCountsOffset.Value, 0);
		}

		public static VectorOffset CreateIPlusBuildingButtonsIncludingBuildingCountsVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateIPlusBuildingButtonsIncludingBuildingCountsVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartIPlusBuildingButtonsIncludingBuildingCountsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddGameMode(FlatBufferBuilder builder, int GameMode)
		{
			builder.AddInt(20, GameMode, 0);
		}

		public static void AddViewingArchivedIsland(FlatBufferBuilder builder, bool ViewingArchivedIsland)
		{
			builder.AddBool(21, ViewingArchivedIsland, d: false);
		}

		public static void AddBBrainShuffled(FlatBufferBuilder builder, bool BBrainShuffled)
		{
			builder.AddBool(22, BBrainShuffled, d: false);
		}

		public static void AddIBBPackUnlockableRemaining(FlatBufferBuilder builder, VectorOffset IBBPackUnlockableRemainingOffset)
		{
			builder.AddOffset(23, IBBPackUnlockableRemainingOffset.Value, 0);
		}

		public static VectorOffset CreateIBBPackUnlockableRemainingVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateIBBPackUnlockableRemainingVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartIBBPackUnlockableRemainingVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddGoUnlockedBuildings(FlatBufferBuilder builder, VectorOffset GoUnlockedBuildingsOffset)
		{
			builder.AddOffset(24, GoUnlockedBuildingsOffset.Value, 0);
		}

		public static VectorOffset CreateGoUnlockedBuildingsVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateGoUnlockedBuildingsVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartGoUnlockedBuildingsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddGoRemaining(FlatBufferBuilder builder, VectorOffset GoRemainingOffset)
		{
			builder.AddOffset(25, GoRemainingOffset.Value, 0);
		}

		public static VectorOffset CreateGoRemainingVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateGoRemainingVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartGoRemainingVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddGoGuaranteedNext(FlatBufferBuilder builder, VectorOffset GoGuaranteedNextOffset)
		{
			builder.AddOffset(26, GoGuaranteedNextOffset.Value, 0);
		}

		public static VectorOffset CreateGoGuaranteedNextVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateGoGuaranteedNextVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartGoGuaranteedNextVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddIPlacedBuildings(FlatBufferBuilder builder, VectorOffset IPlacedBuildingsOffset)
		{
			builder.AddOffset(27, IPlacedBuildingsOffset.Value, 0);
		}

		public static VectorOffset CreateIPlacedBuildingsVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateIPlacedBuildingsVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartIPlacedBuildingsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddIPlacedBuildingsAmount(FlatBufferBuilder builder, VectorOffset IPlacedBuildingsAmountOffset)
		{
			builder.AddOffset(28, IPlacedBuildingsAmountOffset.Value, 0);
		}

		public static VectorOffset CreateIPlacedBuildingsAmountVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateIPlacedBuildingsAmountVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartIPlacedBuildingsAmountVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddRandomStateNextBuildingChoice(FlatBufferBuilder builder, StringOffset randomStateNextBuildingChoiceOffset)
		{
			builder.AddOffset(29, randomStateNextBuildingChoiceOffset.Value, 0);
		}

		public static void AddIBuildingButtonBuildings(FlatBufferBuilder builder, VectorOffset IBuildingButtonBuildingsOffset)
		{
			builder.AddOffset(30, IBuildingButtonBuildingsOffset.Value, 0);
		}

		public static VectorOffset CreateIBuildingButtonBuildingsVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateIBuildingButtonBuildingsVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartIBuildingButtonBuildingsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddIBuildingButtonVariations(FlatBufferBuilder builder, VectorOffset IBuildingButtonVariationsOffset)
		{
			builder.AddOffset(31, IBuildingButtonVariationsOffset.Value, 0);
		}

		public static VectorOffset CreateIBuildingButtonVariationsVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateIBuildingButtonVariationsVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartIBuildingButtonVariationsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddIBuildingButtonVariationsNext(FlatBufferBuilder builder, VectorOffset IBuildingButtonVariationsNextOffset)
		{
			builder.AddOffset(32, IBuildingButtonVariationsNextOffset.Value, 0);
		}

		public static VectorOffset CreateIBuildingButtonVariationsNextVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateIBuildingButtonVariationsNextVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartIBuildingButtonVariationsNextVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddIBBPacksCurrentChoice(FlatBufferBuilder builder, VectorOffset IBBPacksCurrentChoiceOffset)
		{
			builder.AddOffset(33, IBBPacksCurrentChoiceOffset.Value, 0);
		}

		public static VectorOffset CreateIBBPacksCurrentChoiceVector(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddInt(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateIBBPacksCurrentChoiceVectorBlock(FlatBufferBuilder builder, int[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartIBBPacksCurrentChoiceVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddStatsMatch(FlatBufferBuilder builder, Offset<Stats> StatsMatchOffset)
		{
			builder.AddOffset(34, StatsMatchOffset.Value, 0);
		}

		public static Offset<SaveData> EndSaveData(FlatBufferBuilder builder)
		{
			return new Offset<SaveData>(builder.EndTable());
		}

		public static void FinishSaveDataBuffer(FlatBufferBuilder builder, Offset<SaveData> offset)
		{
			builder.Finish(offset.Value, "HTF0");
		}

		public static void FinishSizePrefixedSaveDataBuffer(FlatBufferBuilder builder, Offset<SaveData> offset)
		{
			builder.FinishSizePrefixed(offset.Value, "HTF0");
		}
	}
}
