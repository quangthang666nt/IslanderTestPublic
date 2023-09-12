using FlatBuffers;

namespace Islanders
{
	public struct Quaternion : IFlatbufferObject
	{
		private Struct __p;

		public ByteBuffer ByteBuffer => __p.bb;

		public float X => __p.bb.GetFloat(__p.bb_pos);

		public float Y => __p.bb.GetFloat(__p.bb_pos + 4);

		public float Z => __p.bb.GetFloat(__p.bb_pos + 8);

		public float W => __p.bb.GetFloat(__p.bb_pos + 12);

		public void __init(int _i, ByteBuffer _bb)
		{
			__p = new Struct(_i, _bb);
		}

		public Quaternion __assign(int _i, ByteBuffer _bb)
		{
			__init(_i, _bb);
			return this;
		}

		public static Offset<Quaternion> CreateQuaternion(FlatBufferBuilder builder, float X, float Y, float Z, float W)
		{
			builder.Prep(4, 16);
			builder.PutFloat(W);
			builder.PutFloat(Z);
			builder.PutFloat(Y);
			builder.PutFloat(X);
			return new Offset<Quaternion>(builder.Offset);
		}
	}
}
