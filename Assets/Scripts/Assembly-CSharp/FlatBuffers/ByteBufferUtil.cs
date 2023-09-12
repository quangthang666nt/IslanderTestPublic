namespace FlatBuffers
{
	public class ByteBufferUtil
	{
		public static int GetSizePrefix(ByteBuffer bb)
		{
			return bb.GetInt(bb.Position);
		}

		public static ByteBuffer RemoveSizePrefix(ByteBuffer bb)
		{
			ByteBuffer byteBuffer = bb.Duplicate();
			byteBuffer.Position += 4;
			return byteBuffer;
		}
	}
}
