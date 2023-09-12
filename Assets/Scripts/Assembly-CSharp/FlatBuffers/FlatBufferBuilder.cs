using System;
using System.Collections.Generic;
using System.Text;

namespace FlatBuffers
{
	public class FlatBufferBuilder
	{
		private int _space;

		private ByteBuffer _bb;

		private int _minAlign = 1;

		private int[] _vtable = new int[16];

		private int _vtableSize = -1;

		private int _objectStart;

		private int[] _vtables = new int[16];

		private int _numVtables;

		private int _vectorNumElems;

		private Dictionary<string, StringOffset> _sharedStringMap;

		public bool ForceDefaults { get; set; }

		public int Offset => _bb.Length - _space;

		public ByteBuffer DataBuffer => _bb;

		public FlatBufferBuilder(int initialSize)
		{
			if (initialSize <= 0)
			{
				throw new ArgumentOutOfRangeException("initialSize", initialSize, "Must be greater than zero");
			}
			_space = initialSize;
			_bb = new ByteBuffer(initialSize);
		}

		public FlatBufferBuilder(ByteBuffer buffer)
		{
			_bb = buffer;
			_space = buffer.Length;
			buffer.Reset();
		}

		public void Clear()
		{
			_space = _bb.Length;
			_bb.Reset();
			_minAlign = 1;
			while (_vtableSize > 0)
			{
				_vtable[--_vtableSize] = 0;
			}
			_vtableSize = -1;
			_objectStart = 0;
			_numVtables = 0;
			_vectorNumElems = 0;
		}

		public void Pad(int size)
		{
			_bb.PutByte(_space -= size, 0, size);
		}

		private void GrowBuffer()
		{
			_bb.GrowFront(_bb.Length << 1);
		}

		public void Prep(int size, int additionalBytes)
		{
			if (size > _minAlign)
			{
				_minAlign = size;
			}
			int num = (~(_bb.Length - _space + additionalBytes) + 1) & (size - 1);
			while (_space < num + size + additionalBytes)
			{
				int length = _bb.Length;
				GrowBuffer();
				_space += _bb.Length - length;
			}
			if (num > 0)
			{
				Pad(num);
			}
		}

		public void PutBool(bool x)
		{
			_bb.PutByte(--_space, (byte)(x ? 1u : 0u));
		}

		public void PutSbyte(sbyte x)
		{
			_bb.PutSbyte(--_space, x);
		}

		public void PutByte(byte x)
		{
			_bb.PutByte(--_space, x);
		}

		public void PutShort(short x)
		{
			_bb.PutShort(_space -= 2, x);
		}

		public void PutUshort(ushort x)
		{
			_bb.PutUshort(_space -= 2, x);
		}

		public void PutInt(int x)
		{
			_bb.PutInt(_space -= 4, x);
		}

		public void PutUint(uint x)
		{
			_bb.PutUint(_space -= 4, x);
		}

		public void PutLong(long x)
		{
			_bb.PutLong(_space -= 8, x);
		}

		public void PutUlong(ulong x)
		{
			_bb.PutUlong(_space -= 8, x);
		}

		public void PutFloat(float x)
		{
			_bb.PutFloat(_space -= 4, x);
		}

		public void Put<T>(T[] x) where T : struct
		{
			_space = _bb.Put(_space, x);
		}

		public void PutDouble(double x)
		{
			_bb.PutDouble(_space -= 8, x);
		}

		public void AddBool(bool x)
		{
			Prep(1, 0);
			PutBool(x);
		}

		public void AddSbyte(sbyte x)
		{
			Prep(1, 0);
			PutSbyte(x);
		}

		public void AddByte(byte x)
		{
			Prep(1, 0);
			PutByte(x);
		}

		public void AddShort(short x)
		{
			Prep(2, 0);
			PutShort(x);
		}

		public void AddUshort(ushort x)
		{
			Prep(2, 0);
			PutUshort(x);
		}

		public void AddInt(int x)
		{
			Prep(4, 0);
			PutInt(x);
		}

		public void AddUint(uint x)
		{
			Prep(4, 0);
			PutUint(x);
		}

		public void AddLong(long x)
		{
			Prep(8, 0);
			PutLong(x);
		}

		public void AddUlong(ulong x)
		{
			Prep(8, 0);
			PutUlong(x);
		}

		public void AddFloat(float x)
		{
			Prep(4, 0);
			PutFloat(x);
		}

		public void Add<T>(T[] x) where T : struct
		{
			if (x == null)
			{
				throw new ArgumentNullException("Cannot add a null array");
			}
			if (x.Length != 0)
			{
				if (!ByteBuffer.IsSupportedType<T>())
				{
					throw new ArgumentException("Cannot add this Type array to the builder");
				}
				int num = ByteBuffer.SizeOf<T>();
				Prep(num, num * (x.Length - 1));
				Put(x);
			}
		}

		public void AddDouble(double x)
		{
			Prep(8, 0);
			PutDouble(x);
		}

		public void AddOffset(int off)
		{
			Prep(4, 0);
			if (off > Offset)
			{
				throw new ArgumentException();
			}
			off = Offset - off + 4;
			PutInt(off);
		}

		public void StartVector(int elemSize, int count, int alignment)
		{
			NotNested();
			_vectorNumElems = count;
			Prep(4, elemSize * count);
			Prep(alignment, elemSize * count);
		}

		public VectorOffset EndVector()
		{
			PutInt(_vectorNumElems);
			return new VectorOffset(Offset);
		}

		public VectorOffset CreateVectorOfTables<T>(Offset<T>[] offsets) where T : struct
		{
			NotNested();
			StartVector(4, offsets.Length, 4);
			for (int num = offsets.Length - 1; num >= 0; num--)
			{
				AddOffset(offsets[num].Value);
			}
			return EndVector();
		}

		public void Nested(int obj)
		{
			if (obj != Offset)
			{
				throw new Exception("FlatBuffers: struct must be serialized inline.");
			}
		}

		public void NotNested()
		{
			if (_vtableSize >= 0)
			{
				throw new Exception("FlatBuffers: object serialization must not be nested.");
			}
		}

		public void StartTable(int numfields)
		{
			if (numfields < 0)
			{
				throw new ArgumentOutOfRangeException("Flatbuffers: invalid numfields");
			}
			NotNested();
			if (_vtable.Length < numfields)
			{
				_vtable = new int[numfields];
			}
			_vtableSize = numfields;
			_objectStart = Offset;
		}

		public void Slot(int voffset)
		{
			if (voffset >= _vtableSize)
			{
				throw new IndexOutOfRangeException("Flatbuffers: invalid voffset");
			}
			_vtable[voffset] = Offset;
		}

		public void AddBool(int o, bool x, bool d)
		{
			if (ForceDefaults || x != d)
			{
				AddBool(x);
				Slot(o);
			}
		}

		public void AddSbyte(int o, sbyte x, sbyte d)
		{
			if (ForceDefaults || x != d)
			{
				AddSbyte(x);
				Slot(o);
			}
		}

		public void AddByte(int o, byte x, byte d)
		{
			if (ForceDefaults || x != d)
			{
				AddByte(x);
				Slot(o);
			}
		}

		public void AddShort(int o, short x, int d)
		{
			if (ForceDefaults || x != d)
			{
				AddShort(x);
				Slot(o);
			}
		}

		public void AddUshort(int o, ushort x, ushort d)
		{
			if (ForceDefaults || x != d)
			{
				AddUshort(x);
				Slot(o);
			}
		}

		public void AddInt(int o, int x, int d)
		{
			if (ForceDefaults || x != d)
			{
				AddInt(x);
				Slot(o);
			}
		}

		public void AddUint(int o, uint x, uint d)
		{
			if (ForceDefaults || x != d)
			{
				AddUint(x);
				Slot(o);
			}
		}

		public void AddLong(int o, long x, long d)
		{
			if (ForceDefaults || x != d)
			{
				AddLong(x);
				Slot(o);
			}
		}

		public void AddUlong(int o, ulong x, ulong d)
		{
			if (ForceDefaults || x != d)
			{
				AddUlong(x);
				Slot(o);
			}
		}

		public void AddFloat(int o, float x, double d)
		{
			if (ForceDefaults || (double)x != d)
			{
				AddFloat(x);
				Slot(o);
			}
		}

		public void AddDouble(int o, double x, double d)
		{
			if (ForceDefaults || x != d)
			{
				AddDouble(x);
				Slot(o);
			}
		}

		public void AddOffset(int o, int x, int d)
		{
			if (x != d)
			{
				AddOffset(x);
				Slot(o);
			}
		}

		public StringOffset CreateString(string s)
		{
			NotNested();
			AddByte(0);
			int byteCount = Encoding.UTF8.GetByteCount(s);
			StartVector(1, byteCount, 1);
			_bb.PutStringUTF8(_space -= byteCount, s);
			return new StringOffset(EndVector().Value);
		}

		public StringOffset CreateSharedString(string s)
		{
			if (_sharedStringMap == null)
			{
				_sharedStringMap = new Dictionary<string, StringOffset>();
			}
			if (_sharedStringMap.ContainsKey(s))
			{
				return _sharedStringMap[s];
			}
			StringOffset stringOffset = CreateString(s);
			_sharedStringMap.Add(s, stringOffset);
			return stringOffset;
		}

		public void AddStruct(int voffset, int x, int d)
		{
			if (x != d)
			{
				Nested(x);
				Slot(voffset);
			}
		}

		public int EndTable()
		{
			if (_vtableSize < 0)
			{
				throw new InvalidOperationException("Flatbuffers: calling EndTable without a StartTable");
			}
			AddInt(0);
			int offset = Offset;
			int num = _vtableSize - 1;
			while (num >= 0 && _vtable[num] == 0)
			{
				num--;
			}
			int num2 = num + 1;
			while (num >= 0)
			{
				short x = (short)((_vtable[num] != 0) ? (offset - _vtable[num]) : 0);
				AddShort(x);
				_vtable[num] = 0;
				num--;
			}
			AddShort((short)(offset - _objectStart));
			AddShort((short)((num2 + 2) * 2));
			int num3 = 0;
			for (num = 0; num < _numVtables; num++)
			{
				int num4 = _bb.Length - _vtables[num];
				int space = _space;
				short @short = _bb.GetShort(num4);
				if (@short != _bb.GetShort(space))
				{
					continue;
				}
				int num5 = 2;
				while (num5 < @short)
				{
					if (_bb.GetShort(num4 + num5) == _bb.GetShort(space + num5))
					{
						num5 += 2;
						continue;
					}
					goto IL_0118;
				}
				num3 = _vtables[num];
				break;
				IL_0118:;
			}
			if (num3 != 0)
			{
				_space = _bb.Length - offset;
				_bb.PutInt(_space, num3 - offset);
			}
			else
			{
				if (_numVtables == _vtables.Length)
				{
					int[] array = new int[_numVtables * 2];
					Array.Copy(_vtables, array, _vtables.Length);
					_vtables = array;
				}
				_vtables[_numVtables++] = Offset;
				_bb.PutInt(_bb.Length - offset, Offset - offset);
			}
			_vtableSize = -1;
			return offset;
		}

		public void Required(int table, int field)
		{
			int num = _bb.Length - table;
			int num2 = num - _bb.GetInt(num);
			if (_bb.GetShort(num2 + field) == 0)
			{
				throw new InvalidOperationException("FlatBuffers: field " + field + " must be set");
			}
		}

		protected void Finish(int rootTable, bool sizePrefix)
		{
			Prep(_minAlign, 4 + (sizePrefix ? 4 : 0));
			AddOffset(rootTable);
			if (sizePrefix)
			{
				AddInt(_bb.Length - _space);
			}
			_bb.Position = _space;
		}

		public void Finish(int rootTable)
		{
			Finish(rootTable, sizePrefix: false);
		}

		public void FinishSizePrefixed(int rootTable)
		{
			Finish(rootTable, sizePrefix: true);
		}

		public byte[] SizedByteArray()
		{
			return _bb.ToSizedArray();
		}

		protected void Finish(int rootTable, string fileIdentifier, bool sizePrefix)
		{
			Prep(_minAlign, 4 + (sizePrefix ? 4 : 0) + 4);
			if (fileIdentifier.Length != 4)
			{
				throw new ArgumentException("FlatBuffers: file identifier must be length " + 4, "fileIdentifier");
			}
			for (int num = 3; num >= 0; num--)
			{
				AddByte((byte)fileIdentifier[num]);
			}
			Finish(rootTable, sizePrefix);
		}

		public void Finish(int rootTable, string fileIdentifier)
		{
			Finish(rootTable, fileIdentifier, sizePrefix: false);
		}

		public void FinishSizePrefixed(int rootTable, string fileIdentifier)
		{
			Finish(rootTable, fileIdentifier, sizePrefix: true);
		}
	}
}
