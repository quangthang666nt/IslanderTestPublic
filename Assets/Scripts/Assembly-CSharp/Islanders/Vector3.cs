using FlatBuffers;

namespace Islanders
{
	public struct Vector3 : IFlatbufferObject
	{
		private Struct __p;

		public ByteBuffer ByteBuffer => __p.bb;

		public float X => __p.bb.GetFloat(__p.bb_pos);

		public float Y => __p.bb.GetFloat(__p.bb_pos + 4);

		public float Z => __p.bb.GetFloat(__p.bb_pos + 8);

		public void __init(int _i, ByteBuffer _bb)
		{
			__p = new Struct(_i, _bb);
		}

		public Vector3 __assign(int _i, ByteBuffer _bb)
		{
			__init(_i, _bb);
			return this;
		}

		public static Offset<Vector3> CreateVector3(FlatBufferBuilder builder, float X, float Y, float Z)
		{
			builder.Prep(4, 12);
			builder.PutFloat(Z);
			builder.PutFloat(Y);
			builder.PutFloat(X);
			return new Offset<Vector3>(builder.Offset);
		}
	}
}
