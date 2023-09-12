using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FlatBuffers
{
	public class ByteBuffer
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct ConversionUnion
		{
			[FieldOffset(0)]
			public int intValue;

			[FieldOffset(0)]
			public float floatValue;
		}

		private ByteBufferAllocator _buffer;

		private int _pos;

		private static Dictionary<Type, int> genericSizes = new Dictionary<Type, int>
		{
			{
				typeof(bool),
				1
			},
			{
				typeof(float),
				4
			},
			{
				typeof(double),
				8
			},
			{
				typeof(sbyte),
				1
			},
			{
				typeof(byte),
				1
			},
			{
				typeof(short),
				2
			},
			{
				typeof(ushort),
				2
			},
			{
				typeof(int),
				4
			},
			{
				typeof(uint),
				4
			},
			{
				typeof(ulong),
				8
			},
			{
				typeof(long),
				8
			}
		};

		public int Position
		{
			get
			{
				return _pos;
			}
			set
			{
				_pos = value;
			}
		}

		public int Length => _buffer.Length;

		public ByteBuffer(ByteBufferAllocator allocator, int position)
		{
			_buffer = allocator;
			_pos = position;
		}

		public ByteBuffer(int size)
			: this(new byte[size])
		{
		}

		public ByteBuffer(byte[] buffer)
			: this(buffer, 0)
		{
		}

		public ByteBuffer(byte[] buffer, int pos)
		{
			_buffer = new ByteArrayAllocator(buffer);
			_pos = pos;
		}

		public void Reset()
		{
			_pos = 0;
		}

		public ByteBuffer Duplicate()
		{
			return new ByteBuffer(_buffer, Position);
		}

		public void GrowFront(int newSize)
		{
			_buffer.GrowFront(newSize);
		}

		public byte[] ToArray(int pos, int len)
		{
			return ToArray<byte>(pos, len);
		}

		public static int SizeOf<T>()
		{
			return genericSizes[typeof(T)];
		}

		public static bool IsSupportedType<T>()
		{
			return genericSizes.ContainsKey(typeof(T));
		}

		public static int ArraySize<T>(T[] x)
		{
			return SizeOf<T>() * x.Length;
		}

		public T[] ToArray<T>(int pos, int len) where T : struct
		{
			AssertOffsetAndLength(pos, len);
			T[] array = new T[len];
			Buffer.BlockCopy(_buffer.Buffer, pos, array, 0, ArraySize(array));
			return array;
		}

		public byte[] ToSizedArray()
		{
			return ToArray<byte>(Position, Length - Position);
		}

		public byte[] ToFullArray()
		{
			return ToArray<byte>(0, Length);
		}

		public ArraySegment<byte> ToArraySegment(int pos, int len)
		{
			return new ArraySegment<byte>(_buffer.Buffer, pos, len);
		}

		public MemoryStream ToMemoryStream(int pos, int len)
		{
			return new MemoryStream(_buffer.Buffer, pos, len);
		}

		public static ushort ReverseBytes(ushort input)
		{
			return (ushort)((uint)((input & 0xFF) << 8) | ((uint)(input & 0xFF00) >> 8));
		}

		public static uint ReverseBytes(uint input)
		{
			return ((input & 0xFF) << 24) | ((input & 0xFF00) << 8) | ((input & 0xFF0000) >> 8) | ((input & 0xFF000000u) >> 24);
		}

		public static ulong ReverseBytes(ulong input)
		{
			return ((input & 0xFF) << 56) | ((input & 0xFF00) << 40) | ((input & 0xFF0000) << 24) | ((input & 0xFF000000u) << 8) | ((input & 0xFF00000000L) >> 8) | ((input & 0xFF0000000000L) >> 24) | ((input & 0xFF000000000000L) >> 40) | ((input & 0xFF00000000000000uL) >> 56);
		}

		protected void WriteLittleEndian(int offset, int count, ulong data)
		{
			if (BitConverter.IsLittleEndian)
			{
				for (int i = 0; i < count; i++)
				{
					_buffer.Buffer[offset + i] = (byte)(data >> i * 8);
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					_buffer.Buffer[offset + count - 1 - j] = (byte)(data >> j * 8);
				}
			}
		}

		protected ulong ReadLittleEndian(int offset, int count)
		{
			AssertOffsetAndLength(offset, count);
			ulong num = 0uL;
			if (BitConverter.IsLittleEndian)
			{
				for (int i = 0; i < count; i++)
				{
					num |= (ulong)_buffer.Buffer[offset + i] << i * 8;
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					num |= (ulong)_buffer.Buffer[offset + count - 1 - j] << j * 8;
				}
			}
			return num;
		}

		private void AssertOffsetAndLength(int offset, int length)
		{
			if (offset < 0 || offset > _buffer.Length - length)
			{
				throw new ArgumentOutOfRangeException();
			}
		}

		public void PutSbyte(int offset, sbyte value)
		{
			AssertOffsetAndLength(offset, 1);
			_buffer.Buffer[offset] = (byte)value;
		}

		public void PutByte(int offset, byte value)
		{
			AssertOffsetAndLength(offset, 1);
			_buffer.Buffer[offset] = value;
		}

		public void PutByte(int offset, byte value, int count)
		{
			AssertOffsetAndLength(offset, count);
			for (int i = 0; i < count; i++)
			{
				_buffer.Buffer[offset + i] = value;
			}
		}

		public void Put(int offset, byte value)
		{
			PutByte(offset, value);
		}

		public void PutStringUTF8(int offset, string value)
		{
			AssertOffsetAndLength(offset, value.Length);
			Encoding.UTF8.GetBytes(value, 0, value.Length, _buffer.Buffer, offset);
		}

		public void PutShort(int offset, short value)
		{
			AssertOffsetAndLength(offset, 2);
			WriteLittleEndian(offset, 2, (ulong)value);
		}

		public void PutUshort(int offset, ushort value)
		{
			AssertOffsetAndLength(offset, 2);
			WriteLittleEndian(offset, 2, value);
		}

		public void PutInt(int offset, int value)
		{
			AssertOffsetAndLength(offset, 4);
			WriteLittleEndian(offset, 4, (ulong)value);
		}

		public void PutUint(int offset, uint value)
		{
			AssertOffsetAndLength(offset, 4);
			WriteLittleEndian(offset, 4, value);
		}

		public void PutLong(int offset, long value)
		{
			AssertOffsetAndLength(offset, 8);
			WriteLittleEndian(offset, 8, (ulong)value);
		}

		public void PutUlong(int offset, ulong value)
		{
			AssertOffsetAndLength(offset, 8);
			WriteLittleEndian(offset, 8, value);
		}

		public void PutFloat(int offset, float value)
		{
			AssertOffsetAndLength(offset, 4);
			ConversionUnion conversionUnion = default(ConversionUnion);
			conversionUnion.intValue = 0;
			conversionUnion.floatValue = value;
			WriteLittleEndian(offset, 4, (ulong)conversionUnion.intValue);
		}

		public void PutDouble(int offset, double value)
		{
			AssertOffsetAndLength(offset, 8);
			WriteLittleEndian(offset, 8, (ulong)BitConverter.DoubleToInt64Bits(value));
		}

		public sbyte GetSbyte(int index)
		{
			AssertOffsetAndLength(index, 1);
			return (sbyte)_buffer.Buffer[index];
		}

		public byte Get(int index)
		{
			AssertOffsetAndLength(index, 1);
			return _buffer.Buffer[index];
		}

		public string GetStringUTF8(int startPos, int len)
		{
			return Encoding.UTF8.GetString(_buffer.Buffer, startPos, len);
		}

		public short GetShort(int index)
		{
			return (short)ReadLittleEndian(index, 2);
		}

		public ushort GetUshort(int index)
		{
			return (ushort)ReadLittleEndian(index, 2);
		}

		public int GetInt(int index)
		{
			return (int)ReadLittleEndian(index, 4);
		}

		public uint GetUint(int index)
		{
			return (uint)ReadLittleEndian(index, 4);
		}

		public long GetLong(int index)
		{
			return (long)ReadLittleEndian(index, 8);
		}

		public ulong GetUlong(int index)
		{
			return ReadLittleEndian(index, 8);
		}

		public float GetFloat(int index)
		{
			ConversionUnion conversionUnion = default(ConversionUnion);
			conversionUnion.floatValue = 0f;
			conversionUnion.intValue = (int)ReadLittleEndian(index, 4);
			return conversionUnion.floatValue;
		}

		public double GetDouble(int index)
		{
			return BitConverter.Int64BitsToDouble((long)ReadLittleEndian(index, 8));
		}

		public int Put<T>(int offset, T[] x) where T : struct
		{
			if (x == null)
			{
				throw new ArgumentNullException("Cannot put a null array");
			}
			if (x.Length == 0)
			{
				throw new ArgumentException("Cannot put an empty array");
			}
			if (!IsSupportedType<T>())
			{
				throw new ArgumentException("Cannot put an array of type " + typeof(T)?.ToString() + " into this buffer");
			}
			if (BitConverter.IsLittleEndian)
			{
				int num = ArraySize(x);
				offset -= num;
				AssertOffsetAndLength(offset, num);
				Buffer.BlockCopy(x, 0, _buffer.Buffer, offset, num);
				return offset;
			}
			throw new NotImplementedException("Big Endian Support not implemented yet for putting typed arrays");
		}
	}
}
