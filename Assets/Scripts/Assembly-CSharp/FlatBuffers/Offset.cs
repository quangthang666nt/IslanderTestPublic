namespace FlatBuffers
{
	public struct Offset<T> where T : struct
	{
		public int Value;

		public Offset(int value)
		{
			Value = value;
		}
	}
}
