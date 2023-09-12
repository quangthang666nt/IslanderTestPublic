using FlatBuffers;

namespace Islanders
{
	public struct Transform : IFlatbufferObject
	{
		private Struct __p;

		public ByteBuffer ByteBuffer => __p.bb;

		public Vector3 Position => default(Vector3).__assign(__p.bb_pos, __p.bb);

		public Quaternion Rotation => default(Quaternion).__assign(__p.bb_pos + 12, __p.bb);

		public Vector3 LocalScale => default(Vector3).__assign(__p.bb_pos + 28, __p.bb);

		public int Variation => __p.bb.GetInt(__p.bb_pos + 40);

		public void __init(int _i, ByteBuffer _bb)
		{
			__p = new Struct(_i, _bb);
		}

		public Transform __assign(int _i, ByteBuffer _bb)
		{
			__init(_i, _bb);
			return this;
		}

		public static Offset<Transform> CreateTransform(FlatBufferBuilder builder, float Position_X, float Position_Y, float Position_Z, float Rotation_X, float Rotation_Y, float Rotation_Z, float Rotation_W, float LocalScale_X, float LocalScale_Y, float LocalScale_Z, int Variation)
		{
			builder.Prep(4, 44);
			builder.PutInt(Variation);
			builder.Prep(4, 12);
			builder.PutFloat(LocalScale_Z);
			builder.PutFloat(LocalScale_Y);
			builder.PutFloat(LocalScale_X);
			builder.Prep(4, 16);
			builder.PutFloat(Rotation_W);
			builder.PutFloat(Rotation_Z);
			builder.PutFloat(Rotation_Y);
			builder.PutFloat(Rotation_X);
			builder.Prep(4, 12);
			builder.PutFloat(Position_Z);
			builder.PutFloat(Position_Y);
			builder.PutFloat(Position_X);
			return new Offset<Transform>(builder.Offset);
		}
	}
}
