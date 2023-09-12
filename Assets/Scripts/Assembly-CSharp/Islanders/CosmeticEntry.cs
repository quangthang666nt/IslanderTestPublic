using System;
using System.Text;
using FlatBuffers;

namespace Islanders
{
	public struct CosmeticEntry : IFlatbufferObject
	{
		private Table __p;

		public ByteBuffer ByteBuffer => __p.bb;

		public string Keyval
		{
			get
			{
				int num = __p.__offset(4);
				if (num == 0)
				{
					return null;
				}
				return __p.__string(num + __p.bb_pos);
			}
		}

		public string Cosmetic
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

		public static void ValidateVersion()
		{
			FlatBufferConstants.FLATBUFFERS_1_12_0();
		}

		public static CosmeticEntry GetRootAsCosmeticEntry(ByteBuffer _bb)
		{
			return GetRootAsCosmeticEntry(_bb, default(CosmeticEntry));
		}

		public static CosmeticEntry GetRootAsCosmeticEntry(ByteBuffer _bb, CosmeticEntry obj)
		{
			return obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public void __init(int _i, ByteBuffer _bb)
		{
			__p = new Table(_i, _bb);
		}

		public CosmeticEntry __assign(int _i, ByteBuffer _bb)
		{
			__init(_i, _bb);
			return this;
		}

		public ArraySegment<byte>? GetKeyvalBytes()
		{
			return __p.__vector_as_arraysegment(4);
		}

		public byte[] GetKeyvalArray()
		{
			return __p.__vector_as_array<byte>(4);
		}

		public ArraySegment<byte>? GetCosmeticBytes()
		{
			return __p.__vector_as_arraysegment(6);
		}

		public byte[] GetCosmeticArray()
		{
			return __p.__vector_as_array<byte>(6);
		}

		public static Offset<CosmeticEntry> CreateCosmeticEntry(FlatBufferBuilder builder, StringOffset keyvalOffset = default(StringOffset), StringOffset cosmeticOffset = default(StringOffset))
		{
			builder.StartTable(2);
			AddCosmetic(builder, cosmeticOffset);
			AddKeyval(builder, keyvalOffset);
			return EndCosmeticEntry(builder);
		}

		public static void StartCosmeticEntry(FlatBufferBuilder builder)
		{
			builder.StartTable(2);
		}

		public static void AddKeyval(FlatBufferBuilder builder, StringOffset keyvalOffset)
		{
			builder.AddOffset(0, keyvalOffset.Value, 0);
		}

		public static void AddCosmetic(FlatBufferBuilder builder, StringOffset cosmeticOffset)
		{
			builder.AddOffset(1, cosmeticOffset.Value, 0);
		}

		public static Offset<CosmeticEntry> EndCosmeticEntry(FlatBufferBuilder builder)
		{
			int num = builder.EndTable();
			builder.Required(num, 4);
			return new Offset<CosmeticEntry>(num);
		}

		public static VectorOffset CreateSortedVectorOfCosmeticEntry(FlatBufferBuilder builder, Offset<CosmeticEntry>[] offsets)
		{
			Array.Sort(offsets, (Offset<CosmeticEntry> o1, Offset<CosmeticEntry> o2) => Table.CompareStrings(Table.__offset(4, o1.Value, builder.DataBuffer), Table.__offset(4, o2.Value, builder.DataBuffer), builder.DataBuffer));
			return builder.CreateVectorOfTables(offsets);
		}

		public static CosmeticEntry? __lookup_by_key(int vectorLocation, string key, ByteBuffer bb)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			int num = bb.GetInt(vectorLocation - 4);
			int num2 = 0;
			while (num != 0)
			{
				int num3 = num / 2;
				int num4 = Table.__indirect(vectorLocation + 4 * (num2 + num3), bb);
				int num5 = Table.CompareStrings(Table.__offset(4, bb.Length - num4, bb), bytes, bb);
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
				return default(CosmeticEntry).__assign(num4, bb);
			}
			return null;
		}
	}
}
