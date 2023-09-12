using FlatBuffers;

namespace Islanders
{
	public struct Archive : IFlatbufferObject
	{
		private Table __p;

		public ByteBuffer ByteBuffer => __p.bb;

		public ushort Current
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

		public bool CurrentUpdated
		{
			get
			{
				int num = __p.__offset(6);
				if (num == 0)
				{
					return false;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public int SandboxLength
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

		public static void ValidateVersion()
		{
			FlatBufferConstants.FLATBUFFERS_1_12_0();
		}

		public static Archive GetRootAsArchive(ByteBuffer _bb)
		{
			return GetRootAsArchive(_bb, default(Archive));
		}

		public static Archive GetRootAsArchive(ByteBuffer _bb, Archive obj)
		{
			return obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public static bool ArchiveBufferHasIdentifier(ByteBuffer _bb)
		{
			return Table.__has_identifier(_bb, "SCST");
		}

		public void __init(int _i, ByteBuffer _bb)
		{
			__p = new Table(_i, _bb);
		}

		public Archive __assign(int _i, ByteBuffer _bb)
		{
			__init(_i, _bb);
			return this;
		}

		public ArchiveEntry? Sandbox(int j)
		{
			int num = __p.__offset(8);
			if (num == 0)
			{
				return null;
			}
			return default(ArchiveEntry).__assign(__p.__indirect(__p.__vector(num) + j * 4), __p.bb);
		}

		public ArchiveEntry? SandboxByKey(ushort key)
		{
			int num = __p.__offset(8);
			if (num == 0)
			{
				return null;
			}
			return ArchiveEntry.__lookup_by_key(__p.__vector(num), key, __p.bb);
		}

		public static Offset<Archive> CreateArchive(FlatBufferBuilder builder, ushort current = 0, bool currentUpdated = false, VectorOffset sandboxOffset = default(VectorOffset))
		{
			builder.StartTable(3);
			AddSandbox(builder, sandboxOffset);
			AddCurrent(builder, current);
			AddCurrentUpdated(builder, currentUpdated);
			return EndArchive(builder);
		}

		public static void StartArchive(FlatBufferBuilder builder)
		{
			builder.StartTable(3);
		}

		public static void AddCurrent(FlatBufferBuilder builder, ushort current)
		{
			builder.AddUshort(0, current, 0);
		}

		public static void AddCurrentUpdated(FlatBufferBuilder builder, bool currentUpdated)
		{
			builder.AddBool(1, currentUpdated, d: false);
		}

		public static void AddSandbox(FlatBufferBuilder builder, VectorOffset sandboxOffset)
		{
			builder.AddOffset(2, sandboxOffset.Value, 0);
		}

		public static VectorOffset CreateSandboxVector(FlatBufferBuilder builder, Offset<ArchiveEntry>[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddOffset(data[num].Value);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateSandboxVectorBlock(FlatBufferBuilder builder, Offset<ArchiveEntry>[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartSandboxVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static Offset<Archive> EndArchive(FlatBufferBuilder builder)
		{
			return new Offset<Archive>(builder.EndTable());
		}

		public static void FinishArchiveBuffer(FlatBufferBuilder builder, Offset<Archive> offset)
		{
			builder.Finish(offset.Value, "SCST");
		}

		public static void FinishSizePrefixedArchiveBuffer(FlatBufferBuilder builder, Offset<Archive> offset)
		{
			builder.FinishSizePrefixed(offset.Value, "SCST");
		}
	}
}
