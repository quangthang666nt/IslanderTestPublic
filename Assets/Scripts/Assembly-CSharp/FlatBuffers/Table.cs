using System;

namespace FlatBuffers
{
	public struct Table
	{
		public int bb_pos { get; private set; }

		public ByteBuffer bb { get; private set; }

		public ByteBuffer ByteBuffer => bb;

		public Table(int _i, ByteBuffer _bb)
		{
			this = default(Table);
			bb = _bb;
			bb_pos = _i;
		}

		public int __offset(int vtableOffset)
		{
			int num = bb_pos - bb.GetInt(bb_pos);
			if (vtableOffset >= bb.GetShort(num))
			{
				return 0;
			}
			return bb.GetShort(num + vtableOffset);
		}

		public static int __offset(int vtableOffset, int offset, ByteBuffer bb)
		{
			int num = bb.Length - offset;
			return bb.GetShort(num + vtableOffset - bb.GetInt(num)) + num;
		}

		public int __indirect(int offset)
		{
			return offset + bb.GetInt(offset);
		}

		public static int __indirect(int offset, ByteBuffer bb)
		{
			return offset + bb.GetInt(offset);
		}

		public string __string(int offset)
		{
			offset += bb.GetInt(offset);
			int @int = bb.GetInt(offset);
			int startPos = offset + 4;
			return bb.GetStringUTF8(startPos, @int);
		}

		public int __vector_len(int offset)
		{
			offset += bb_pos;
			offset += bb.GetInt(offset);
			return bb.GetInt(offset);
		}

		public int __vector(int offset)
		{
			offset += bb_pos;
			return offset + bb.GetInt(offset) + 4;
		}

		public ArraySegment<byte>? __vector_as_arraysegment(int offset)
		{
			int num = __offset(offset);
			if (num == 0)
			{
				return null;
			}
			int pos = __vector(num);
			int len = __vector_len(num);
			return bb.ToArraySegment(pos, len);
		}

		public T[] __vector_as_array<T>(int offset) where T : struct
		{
			if (!BitConverter.IsLittleEndian)
			{
				throw new NotSupportedException("Getting typed arrays on a Big Endian system is not support");
			}
			int num = __offset(offset);
			if (num == 0)
			{
				return null;
			}
			int pos = __vector(num);
			int len = __vector_len(num);
			return bb.ToArray<T>(pos, len);
		}

		public T __union<T>(int offset) where T : struct, IFlatbufferObject
		{
			T result = new T();
			result.__init(__indirect(offset), bb);
			return result;
		}

		public static bool __has_identifier(ByteBuffer bb, string ident)
		{
			if (ident.Length != 4)
			{
				throw new ArgumentException("FlatBuffers: file identifier must be length " + 4, "ident");
			}
			for (int i = 0; i < 4; i++)
			{
				if (ident[i] != bb.Get(bb.Position + 4 + i))
				{
					return false;
				}
			}
			return true;
		}

		public static int CompareStrings(int offset_1, int offset_2, ByteBuffer bb)
		{
			offset_1 += bb.GetInt(offset_1);
			offset_2 += bb.GetInt(offset_2);
			int @int = bb.GetInt(offset_1);
			int int2 = bb.GetInt(offset_2);
			int num = offset_1 + 4;
			int num2 = offset_2 + 4;
			int num3 = Math.Min(@int, int2);
			for (int i = 0; i < num3; i++)
			{
				byte b = bb.Get(i + num);
				byte b2 = bb.Get(i + num2);
				if (b != b2)
				{
					return b - b2;
				}
			}
			return @int - int2;
		}

		public static int CompareStrings(int offset_1, byte[] key, ByteBuffer bb)
		{
			offset_1 += bb.GetInt(offset_1);
			int @int = bb.GetInt(offset_1);
			int num = key.Length;
			int num2 = offset_1 + 4;
			int num3 = Math.Min(@int, num);
			for (int i = 0; i < num3; i++)
			{
				byte b = bb.Get(i + num2);
				if (b != key[i])
				{
					return b - key[i];
				}
			}
			return @int - num;
		}
	}
}
