namespace FlatBuffers
{
	public abstract class ByteBufferAllocator
	{
		public byte[] Buffer { get; protected set; }

		public int Length { get; protected set; }

		public abstract void GrowFront(int newSize);
	}
}
