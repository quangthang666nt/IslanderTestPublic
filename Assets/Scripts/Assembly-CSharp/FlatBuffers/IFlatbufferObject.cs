namespace FlatBuffers
{
	public interface IFlatbufferObject
	{
		ByteBuffer ByteBuffer { get; }

		void __init(int _i, ByteBuffer _bb);
	}
}
