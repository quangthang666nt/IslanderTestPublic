using System;
using FlatBuffers;

namespace Islanders
{
	public struct IslandGallery : IFlatbufferObject
	{
		private Table __p;

		public ByteBuffer ByteBuffer => __p.bb;

		public int HighresLength
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

		public int LowresLength
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

		public static void ValidateVersion()
		{
			FlatBufferConstants.FLATBUFFERS_1_12_0();
		}

		public static IslandGallery GetRootAsIslandGallery(ByteBuffer _bb)
		{
			return GetRootAsIslandGallery(_bb, default(IslandGallery));
		}

		public static IslandGallery GetRootAsIslandGallery(ByteBuffer _bb, IslandGallery obj)
		{
			return obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public static bool IslandGalleryBufferHasIdentifier(ByteBuffer _bb)
		{
			return Table.__has_identifier(_bb, "SCST");
		}

		public void __init(int _i, ByteBuffer _bb)
		{
			__p = new Table(_i, _bb);
		}

		public IslandGallery __assign(int _i, ByteBuffer _bb)
		{
			__init(_i, _bb);
			return this;
		}

		public byte Highres(int j)
		{
			int num = __p.__offset(4);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.Get(__p.__vector(num) + j);
		}

		public ArraySegment<byte>? GetHighresBytes()
		{
			return __p.__vector_as_arraysegment(4);
		}

		public byte[] GetHighresArray()
		{
			return __p.__vector_as_array<byte>(4);
		}

		public byte Lowres(int j)
		{
			int num = __p.__offset(6);
			if (num == 0)
			{
				return 0;
			}
			return __p.bb.Get(__p.__vector(num) + j);
		}

		public ArraySegment<byte>? GetLowresBytes()
		{
			return __p.__vector_as_arraysegment(6);
		}

		public byte[] GetLowresArray()
		{
			return __p.__vector_as_array<byte>(6);
		}

		public static Offset<IslandGallery> CreateIslandGallery(FlatBufferBuilder builder, VectorOffset highresOffset = default(VectorOffset), VectorOffset lowresOffset = default(VectorOffset))
		{
			builder.StartTable(2);
			AddLowres(builder, lowresOffset);
			AddHighres(builder, highresOffset);
			return EndIslandGallery(builder);
		}

		public static void StartIslandGallery(FlatBufferBuilder builder)
		{
			builder.StartTable(2);
		}

		public static void AddHighres(FlatBufferBuilder builder, VectorOffset highresOffset)
		{
			builder.AddOffset(0, highresOffset.Value, 0);
		}

		public static VectorOffset CreateHighresVector(FlatBufferBuilder builder, byte[] data)
		{
			builder.StartVector(1, data.Length, 1);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddByte(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateHighresVectorBlock(FlatBufferBuilder builder, byte[] data)
		{
			builder.StartVector(1, data.Length, 1);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartHighresVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(1, numElems, 1);
		}

		public static void AddLowres(FlatBufferBuilder builder, VectorOffset lowresOffset)
		{
			builder.AddOffset(1, lowresOffset.Value, 0);
		}

		public static VectorOffset CreateLowresVector(FlatBufferBuilder builder, byte[] data)
		{
			builder.StartVector(1, data.Length, 1);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddByte(data[num]);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateLowresVectorBlock(FlatBufferBuilder builder, byte[] data)
		{
			builder.StartVector(1, data.Length, 1);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartLowresVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(1, numElems, 1);
		}

		public static Offset<IslandGallery> EndIslandGallery(FlatBufferBuilder builder)
		{
			return new Offset<IslandGallery>(builder.EndTable());
		}

		public static void FinishIslandGalleryBuffer(FlatBufferBuilder builder, Offset<IslandGallery> offset)
		{
			builder.Finish(offset.Value, "SCST");
		}

		public static void FinishSizePrefixedIslandGalleryBuffer(FlatBufferBuilder builder, Offset<IslandGallery> offset)
		{
			builder.FinishSizePrefixed(offset.Value, "SCST");
		}
	}
}
