using System;
using FlatBuffers;

namespace Islanders
{
	public struct ArchiveEntry : IFlatbufferObject
	{
		private Table __p;

		public ByteBuffer ByteBuffer => __p.bb;

		public ushort Id
		{
			get
			{
				int num = __p.__offset(4);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetUshort(num + __p.bb_pos);
			}
		}

		public string Slot
		{
			get
			{
				int num = __p.__offset(6);
				if (num == 0)
				{
					return null;
				}
				return __p.__string(num + __p.bb_pos);
			}
		}

		public string Name
		{
			get
			{
				int num = __p.__offset(8);
				if (num == 0)
				{
					return null;
				}
				return __p.__string(num + __p.bb_pos);
			}
		}

		public string Datetime
		{
			get
			{
				int num = __p.__offset(10);
				if (num == 0)
				{
					return null;
				}
				return __p.__string(num + __p.bb_pos);
			}
		}

		public string Screenshot
		{
			get
			{
				int num = __p.__offset(12);
				if (num == 0)
				{
					return null;
				}
				return __p.__string(num + __p.bb_pos);
			}
		}

		public int TypesLength
		{
			get
			{
				int num = __p.__offset(14);
				if (num == 0)
				{
					return 0;
				}
				return __p.__vector_len(num);
			}
		}

		public ushort Biome
		{
			get
			{
				int num = __p.__offset(16);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetUshort(num + __p.bb_pos);
			}
		}

		public ushort Size
		{
			get
			{
				int num = __p.__offset(18);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetUshort(num + __p.bb_pos);
			}
		}

		public ushort FlowerWeight
		{
			get
			{
				int num = __p.__offset(20);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetUshort(num + __p.bb_pos);
			}
		}

		public ushort TreeWeight
		{
			get
			{
				int num = __p.__offset(22);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetUshort(num + __p.bb_pos);
			}
		}

		public ushort Weather
		{
			get
			{
				int num = __p.__offset(24);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetUshort(num + __p.bb_pos);
			}
		}

		public ushort WeatherWeight
		{
			get
			{
				int num = __p.__offset(26);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetUshort(num + __p.bb_pos);
			}
		}

		public ushort SelectedType
		{
			get
			{
				int num = __p.__offset(28);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetUshort(num + __p.bb_pos);
			}
		}

		public ushort SelectedIndex
		{
			get
			{
				int num = __p.__offset(30);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetUshort(num + __p.bb_pos);
			}
		}

		public bool PlayerData
		{
			get
			{
				int num = __p.__offset(32);
				if (num == 0)
				{
					return false;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public bool MatchTutorialDone
		{
			get
			{
				int num = __p.__offset(34);
				if (num == 0)
				{
					return false;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public ushort MatchBuildsPlaced
		{
			get
			{
				int num = __p.__offset(36);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetUshort(num + __p.bb_pos);
			}
		}

		public static void ValidateVersion()
		{
			FlatBufferConstants.FLATBUFFERS_1_12_0();
		}

		public static ArchiveEntry GetRootAsArchiveEntry(ByteBuffer _bb)
		{
			return GetRootAsArchiveEntry(_bb, default(ArchiveEntry));
		}

		public static ArchiveEntry GetRootAsArchiveEntry(ByteBuffer _bb, ArchiveEntry obj)
		{
			return obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public void __init(int _i, ByteBuffer _bb)
		{
			__p = new Table(_i, _bb);
		}

		public ArchiveEntry __assign(int _i, ByteBuffer _bb)
		{
			__init(_i, _bb);
			return this;
		}

		public ArraySegment<byte>? GetSlotBytes()
		{
			return __p.__vector_as_arraysegment(6);
		}

		public byte[] GetSlotArray()
		{
			return __p.__vector_as_array<byte>(6);
		}

		public ArraySegment<byte>? GetNameBytes()
		{
			return __p.__vector_as_arraysegment(8);
		}

		public byte[] GetNameArray()
		{
			return __p.__vector_as_array<byte>(8);
		}

		public ArraySegment<byte>? GetDatetimeBytes()
		{
			return __p.__vector_as_arraysegment(10);
		}

		public byte[] GetDatetimeArray()
		{
			return __p.__vector_as_array<byte>(10);
		}

		public ArraySegment<byte>? GetScreenshotBytes()
		{
			return __p.__vector_as_arraysegment(12);
		}

		public byte[] GetScreenshotArray()
		{
			return __p.__vector_as_array<byte>(12);
		}

		public ushort Types(int j)
		{
			int num = __p.__offset(14);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetUshort(__p.__vector(num) + j * 2);
		}

		public ArraySegment<byte>? GetTypesBytes()
		{
			return __p.__vector_as_arraysegment(14);
		}

		public ushort[] GetTypesArray()
		{
			return __p.__vector_as_array<ushort>(14);
		}

		public static Offset<ArchiveEntry> CreateArchiveEntry(FlatBufferBuilder builder, ushort id = 0, StringOffset slotOffset = default(StringOffset), StringOffset nameOffset = default(StringOffset), StringOffset datetimeOffset = default(StringOffset), StringOffset screenshotOffset = default(StringOffset), VectorOffset typesOffset = default(VectorOffset), ushort biome = 0, ushort size = 0, ushort flowerWeight = 0, ushort treeWeight = 0, ushort weather = 0, ushort weatherWeight = 0, ushort selectedType = 0, ushort selectedIndex = 0, bool playerData = false, bool matchTutorialDone = false, ushort matchBuildsPlaced = 0)
		{
			builder.StartTable(17);
			AddTypes(builder, typesOffset);
			AddScreenshot(builder, screenshotOffset);
			AddDatetime(builder, datetimeOffset);
			AddName(builder, nameOffset);
			AddSlot(builder, slotOffset);
			AddMatchBuildsPlaced(builder, matchBuildsPlaced);
			AddSelectedIndex(builder, selectedIndex);
			AddSelectedType(builder, selectedType);
			AddWeatherWeight(builder, weatherWeight);
			AddWeather(builder, weather);
			AddTreeWeight(builder, treeWeight);
			AddFlowerWeight(builder, flowerWeight);
			AddSize(builder, size);
			AddBiome(builder, biome);
			AddId(builder, id);
			AddMatchTutorialDone(builder, matchTutorialDone);
			AddPlayerData(builder, playerData);
			return EndArchiveEntry(builder);
		}

		public static void StartArchiveEntry(FlatBufferBuilder builder)
		{
			builder.StartTable(17);
		}

		public static void AddId(FlatBufferBuilder builder, ushort id)
		{
			builder.AddUshort(0, id, 0);
		}

		public static void AddSlot(FlatBufferBuilder builder, StringOffset slotOffset)
		{
			builder.AddOffset(1, slotOffset.Value, 0);
		}

		public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset)
		{
			builder.AddOffset(2, nameOffset.Value, 0);
		}

		public static void AddDatetime(FlatBufferBuilder builder, StringOffset datetimeOffset)
		{
			builder.AddOffset(3, datetimeOffset.Value, 0);
		}

		public static void AddScreenshot(FlatBufferBuilder builder, StringOffset screenshotOffset)
		{
			builder.AddOffset(4, screenshotOffset.Value, 0);
		}

		public static void AddTypes(FlatBufferBuilder builder, VectorOffset typesOffset)
		{
			builder.AddOffset(5, typesOffset.Value, 0);
		}

		public static VectorOffset CreateTypesVector(FlatBufferBuilder builder, ushort[] data)
		{
			builder.StartVector(2, data.Length, 2);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddUshort(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateTypesVectorBlock(FlatBufferBuilder builder, ushort[] data)
		{
			builder.StartVector(2, data.Length, 2);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartTypesVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(2, numElems, 2);
		}

		public static void AddBiome(FlatBufferBuilder builder, ushort biome)
		{
			builder.AddUshort(6, biome, 0);
		}

		public static void AddSize(FlatBufferBuilder builder, ushort size)
		{
			builder.AddUshort(7, size, 0);
		}

		public static void AddFlowerWeight(FlatBufferBuilder builder, ushort flowerWeight)
		{
			builder.AddUshort(8, flowerWeight, 0);
		}

		public static void AddTreeWeight(FlatBufferBuilder builder, ushort treeWeight)
		{
			builder.AddUshort(9, treeWeight, 0);
		}

		public static void AddWeather(FlatBufferBuilder builder, ushort weather)
		{
			builder.AddUshort(10, weather, 0);
		}

		public static void AddWeatherWeight(FlatBufferBuilder builder, ushort weatherWeight)
		{
			builder.AddUshort(11, weatherWeight, 0);
		}

		public static void AddSelectedType(FlatBufferBuilder builder, ushort selectedType)
		{
			builder.AddUshort(12, selectedType, 0);
		}

		public static void AddSelectedIndex(FlatBufferBuilder builder, ushort selectedIndex)
		{
			builder.AddUshort(13, selectedIndex, 0);
		}

		public static void AddPlayerData(FlatBufferBuilder builder, bool playerData)
		{
			builder.AddBool(14, playerData, d: false);
		}

		public static void AddMatchTutorialDone(FlatBufferBuilder builder, bool matchTutorialDone)
		{
			builder.AddBool(15, matchTutorialDone, d: false);
		}

		public static void AddMatchBuildsPlaced(FlatBufferBuilder builder, ushort matchBuildsPlaced)
		{
			builder.AddUshort(16, matchBuildsPlaced, 0);
		}

		public static Offset<ArchiveEntry> EndArchiveEntry(FlatBufferBuilder builder)
		{
			return new Offset<ArchiveEntry>(builder.EndTable());
		}

		public static VectorOffset CreateSortedVectorOfArchiveEntry(FlatBufferBuilder builder, Offset<ArchiveEntry>[] offsets)
		{
			Array.Sort(offsets, (Offset<ArchiveEntry> o1, Offset<ArchiveEntry> o2) => builder.DataBuffer.GetUshort(Table.__offset(4, o1.Value, builder.DataBuffer)).CompareTo(builder.DataBuffer.GetUshort(Table.__offset(4, o2.Value, builder.DataBuffer))));
			return builder.CreateVectorOfTables(offsets);
		}

		public static ArchiveEntry? __lookup_by_key(int vectorLocation, ushort key, ByteBuffer bb)
		{
			int num = bb.GetInt(vectorLocation - 4);
			int num2 = 0;
			while (num != 0)
			{
				int num3 = num / 2;
				int num4 = Table.__indirect(vectorLocation + 4 * (num2 + num3), bb);
				int num5 = bb.GetUshort(Table.__offset(4, bb.Length - num4, bb)).CompareTo(key);
				if (num5 > 0)
				{
					num = num3;
					continue;
				}
				if (num5 < 0)
				{
					num3++;
					num2 += num3;
					num -= num3;
					continue;
				}
				return default(ArchiveEntry).__assign(num4, bb);
			}
			return null;
		}
	}
}
