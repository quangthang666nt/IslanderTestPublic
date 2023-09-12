using System;

namespace FlatBuffers
{
	public sealed class ByteArrayAllocator : ByteBufferAllocator
	{
		private byte[] _buffer;

		public ByteArrayAllocator(byte[] buffer)
		{
			_buffer = buffer;
			InitBuffer();
		}

		public override void GrowFront(int newSize)
		{
			if ((base.Length & 0xC0000000u) != 0L)
			{
				throw new Exception("ByteBuffer: cannot grow buffer beyond 2 gigabytes.");
			}
			if (newSize < base.Length)
			{
				throw new Exception("ByteBuffer: cannot truncate buffer.");
			}
			byte[] array = new byte[newSize];
			System.Buffer.BlockCopy(_buffer, 0, array, newSize - base.Length, base.Length);
			_buffer = array;
			InitBuffer();
		}

		private void InitBuffer()
		{
			base.Length = _buffer.Length;
			base.Buffer = _buffer;
		}
	}
}
