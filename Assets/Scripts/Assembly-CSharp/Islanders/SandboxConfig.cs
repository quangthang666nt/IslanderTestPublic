using System;
using FlatBuffers;

namespace Islanders
{
	public struct SandboxConfig : IFlatbufferObject
	{
		private Table __p;

		public ByteBuffer ByteBuffer => __p.bb;

		public int TypesLength
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

		public ushort Biome
		{
			get
			{
				int num = __p.__offset(6);
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
				int num = __p.__offset(8);
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
				int num = __p.__offset(10);
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
				int num = __p.__offset(12);
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
				int num = __p.__offset(14);
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
				int num = __p.__offset(16);
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
				int num = __p.__offset(18);
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
				int num = __p.__offset(20);
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
				int num = __p.__offset(22);
				if (num == 0)
				{
					return false;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public bool GlobalTutorialDone
		{
			get
			{
				int num = __p.__offset(24);
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
				int num = __p.__offset(26);
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
				int num = __p.__offset(28);
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

		public static SandboxConfig GetRootAsSandboxConfig(ByteBuffer _bb)
		{
			return GetRootAsSandboxConfig(_bb, default(SandboxConfig));
		}

		public static SandboxConfig GetRootAsSandboxConfig(ByteBuffer _bb, SandboxConfig obj)
		{
			return obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public static bool SandboxConfigBufferHasIdentifier(ByteBuffer _bb)
		{
			return Table.__has_identifier(_bb, "SCST");
		}

		public void __init(int _i, ByteBuffer _bb)
		{
			__p = new Table(_i, _bb);
		}

		public SandboxConfig __assign(int _i, ByteBuffer _bb)
		{
			__init(_i, _bb);
			return this;
		}

		public ushort Types(int j)
		{
			int num = __p.__offset(4);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.GetUshort(__p.__vector(num) + j * 2);
		}

		public ArraySegment<byte>? GetTypesBytes()
		{
			return __p.__vector_as_arraysegment(4);
		}

		public ushort[] GetTypesArray()
		{
			return __p.__vector_as_array<ushort>(4);
		}

		public static Offset<SandboxConfig> CreateSandboxConfig(FlatBufferBuilder builder, VectorOffset typesOffset = default(VectorOffset), ushort biome = 0, ushort size = 0, ushort flowerWeight = 0, ushort treeWeight = 0, ushort weather = 0, ushort weatherWeight = 0, ushort selectedType = 0, ushort selectedIndex = 0, bool playerData = false, bool globalTutorialDone = false, bool matchTutorialDone = false, ushort matchBuildsPlaced = 0)
		{
			builder.StartTable(13);
			AddTypes(builder, typesOffset);
			AddMatchBuildsPlaced(builder, matchBuildsPlaced);
			AddSelectedIndex(builder, selectedIndex);
			AddSelectedType(builder, selectedType);
			AddWeatherWeight(builder, weatherWeight);
			AddWeather(builder, weather);
			AddTreeWeight(builder, treeWeight);
			AddFlowerWeight(builder, flowerWeight);
			AddSize(builder, size);
			AddBiome(builder, biome);
			AddMatchTutorialDone(builder, matchTutorialDone);
			AddGlobalTutorialDone(builder, globalTutorialDone);
			AddPlayerData(builder, playerData);
			return EndSandboxConfig(builder);
		}

		public static void StartSandboxConfig(FlatBufferBuilder builder)
		{
			builder.StartTable(13);
		}

		public static void AddTypes(FlatBufferBuilder builder, VectorOffset typesOffset)
		{
			builder.AddOffset(0, typesOffset.Value, 0);
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
			builder.AddUshort(1, biome, 0);
		}

		public static void AddSize(FlatBufferBuilder builder, ushort size)
		{
			builder.AddUshort(2, size, 0);
		}

		public static void AddFlowerWeight(FlatBufferBuilder builder, ushort flowerWeight)
		{
			builder.AddUshort(3, flowerWeight, 0);
		}

		public static void AddTreeWeight(FlatBufferBuilder builder, ushort treeWeight)
		{
			builder.AddUshort(4, treeWeight, 0);
		}

		public static void AddWeather(FlatBufferBuilder builder, ushort weather)
		{
			builder.AddUshort(5, weather, 0);
		}

		public static void AddWeatherWeight(FlatBufferBuilder builder, ushort weatherWeight)
		{
			builder.AddUshort(6, weatherWeight, 0);
		}

		public static void AddSelectedType(FlatBufferBuilder builder, ushort selectedType)
		{
			builder.AddUshort(7, selectedType, 0);
		}

		public static void AddSelectedIndex(FlatBufferBuilder builder, ushort selectedIndex)
		{
			builder.AddUshort(8, selectedIndex, 0);
		}

		public static void AddPlayerData(FlatBufferBuilder builder, bool playerData)
		{
			builder.AddBool(9, playerData, d: false);
		}

		public static void AddGlobalTutorialDone(FlatBufferBuilder builder, bool globalTutorialDone)
		{
			builder.AddBool(10, globalTutorialDone, d: false);
		}

		public static void AddMatchTutorialDone(FlatBufferBuilder builder, bool matchTutorialDone)
		{
			builder.AddBool(11, matchTutorialDone, d: false);
		}

		public static void AddMatchBuildsPlaced(FlatBufferBuilder builder, ushort matchBuildsPlaced)
		{
			builder.AddUshort(12, matchBuildsPlaced, 0);
		}

		public static Offset<SandboxConfig> EndSandboxConfig(FlatBufferBuilder builder)
		{
			return new Offset<SandboxConfig>(builder.EndTable());
		}

		public static void FinishSandboxConfigBuffer(FlatBufferBuilder builder, Offset<SandboxConfig> offset)
		{
			builder.Finish(offset.Value, "SCST");
		}

		public static void FinishSizePrefixedSandboxConfigBuffer(FlatBufferBuilder builder, Offset<SandboxConfig> offset)
		{
			builder.FinishSizePrefixed(offset.Value, "SCST");
		}
	}
}
