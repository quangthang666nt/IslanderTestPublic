using System;
using FlatBuffers;

namespace Islanders
{
	public struct Cosmetics : IFlatbufferObject
	{
		private Table __p;

		public ByteBuffer ByteBuffer => __p.bb;

		public int EntriesLength
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

		public string Theme
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

		public bool Ask
		{
			get
			{
				int num = __p.__offset(8);
				if (num == 0)
				{
					return false;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public string Asked
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

		public bool ThemePlaylist
		{
			get
			{
				int num = __p.__offset(12);
				if (num == 0)
				{
					return false;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public int PlaylistsLength
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

		public bool ControllerSpeakerDisabled
		{
			get
			{
				int num = __p.__offset(16);
				if (num == 0)
				{
					return false;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public static void ValidateVersion()
		{
			FlatBufferConstants.FLATBUFFERS_1_12_0();
		}

		public static Cosmetics GetRootAsCosmetics(ByteBuffer _bb)
		{
			return GetRootAsCosmetics(_bb, default(Cosmetics));
		}

		public static Cosmetics GetRootAsCosmetics(ByteBuffer _bb, Cosmetics obj)
		{
			return obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public static bool CosmeticsBufferHasIdentifier(ByteBuffer _bb)
		{
			return Table.__has_identifier(_bb, "SCST");
		}

		public void __init(int _i, ByteBuffer _bb)
		{
			__p = new Table(_i, _bb);
		}

		public Cosmetics __assign(int _i, ByteBuffer _bb)
		{
			__init(_i, _bb);
			return this;
		}

		public CosmeticEntry? Entries(int j)
		{
			int num = __p.__offset(4);
			if (num == 0)
			{
				return null;
			}
			return default(CosmeticEntry).__assign(__p.__indirect(__p.__vector(num) + j * 4), __p.bb);
		}

		public CosmeticEntry? EntriesByKey(string key)
		{
			int num = __p.__offset(4);
			if (num == 0)
			{
				return null;
			}
			return CosmeticEntry.__lookup_by_key(__p.__vector(num), key, __p.bb);
		}

		public ArraySegment<byte>? GetThemeBytes()
		{
			return __p.__vector_as_arraysegment(6);
		}

		public byte[] GetThemeArray()
		{
			return __p.__vector_as_array<byte>(6);
		}

		public ArraySegment<byte>? GetAskedBytes()
		{
			return __p.__vector_as_arraysegment(10);
		}

		public byte[] GetAskedArray()
		{
			return __p.__vector_as_array<byte>(10);
		}

		public string Playlists(int j)
		{
			int num = __p.__offset(14);
			if (num == 0)
			{
				return null;
			}
			return __p.__string(__p.__vector(num) + j * 4);
		}

		public static Offset<Cosmetics> CreateCosmetics(FlatBufferBuilder builder, VectorOffset entriesOffset = default(VectorOffset), StringOffset themeOffset = default(StringOffset), bool ask = false, StringOffset askedOffset = default(StringOffset), bool themePlaylist = false, VectorOffset playlistsOffset = default(VectorOffset), bool controllerSpeakerDisabled = false)
		{
			builder.StartTable(7);
			AddPlaylists(builder, playlistsOffset);
			AddAsked(builder, askedOffset);
			AddTheme(builder, themeOffset);
			AddEntries(builder, entriesOffset);
			AddControllerSpeakerDisabled(builder, controllerSpeakerDisabled);
			AddThemePlaylist(builder, themePlaylist);
			AddAsk(builder, ask);
			return EndCosmetics(builder);
		}

		public static void StartCosmetics(FlatBufferBuilder builder)
		{
			builder.StartTable(7);
		}

		public static void AddEntries(FlatBufferBuilder builder, VectorOffset entriesOffset)
		{
			builder.AddOffset(0, entriesOffset.Value, 0);
		}

		public static VectorOffset CreateEntriesVector(FlatBufferBuilder builder, Offset<CosmeticEntry>[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddOffset(data[num].Value);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreateEntriesVectorBlock(FlatBufferBuilder builder, Offset<CosmeticEntry>[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartEntriesVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddTheme(FlatBufferBuilder builder, StringOffset themeOffset)
		{
			builder.AddOffset(1, themeOffset.Value, 0);
		}

		public static void AddAsk(FlatBufferBuilder builder, bool ask)
		{
			builder.AddBool(2, ask, d: false);
		}

		public static void AddAsked(FlatBufferBuilder builder, StringOffset askedOffset)
		{
			builder.AddOffset(3, askedOffset.Value, 0);
		}

		public static void AddThemePlaylist(FlatBufferBuilder builder, bool themePlaylist)
		{
			builder.AddBool(4, themePlaylist, d: false);
		}

		public static void AddPlaylists(FlatBufferBuilder builder, VectorOffset playlistsOffset)
		{
			builder.AddOffset(5, playlistsOffset.Value, 0);
		}

		public static VectorOffset CreatePlaylistsVector(FlatBufferBuilder builder, StringOffset[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int num = data.Length - 1; num >= 0; num--)
			{
				builder.AddOffset(data[num].Value);
			}
			return builder.EndVector();
		}

		public static VectorOffset CreatePlaylistsVectorBlock(FlatBufferBuilder builder, StringOffset[] data)
		{
			builder.StartVector(4, data.Length, 4);
			builder.Add(data);
			return builder.EndVector();
		}

		public static void StartPlaylistsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddControllerSpeakerDisabled(FlatBufferBuilder builder, bool controllerSpeakerDisabled)
		{
			builder.AddBool(6, controllerSpeakerDisabled, d: false);
		}

		public static Offset<Cosmetics> EndCosmetics(FlatBufferBuilder builder)
		{
			return new Offset<Cosmetics>(builder.EndTable());
		}

		public static void FinishCosmeticsBuffer(FlatBufferBuilder builder, Offset<Cosmetics> offset)
		{
			builder.Finish(offset.Value, "SCST");
		}

		public static void FinishSizePrefixedCosmeticsBuffer(FlatBufferBuilder builder, Offset<Cosmetics> offset)
		{
			builder.FinishSizePrefixed(offset.Value, "SCST");
		}
	}
}
