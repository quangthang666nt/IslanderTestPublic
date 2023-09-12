using System.Text;

namespace SCS.Utils
{
	public struct SCSString32
	{
		private const int SIZE = 32;

		//private unsafe fixed byte data[32];

		private int bsize;

		//public unsafe static implicit operator string(SCSString32 s)
		//{
		//	byte[] array = new byte[32];
		//	for (int i = 0; i < 32; i++)
		//	{
		//		array[i] = s.data[i];
		//	}
		//	return Encoding.Default.GetString(array, 0, s.bsize);
		//}

		//public unsafe static implicit operator SCSString32(string s)
		//{
		//	byte[] bytes = Encoding.Default.GetBytes(s);
		//	SCSString32 result = default(SCSString32);
		//	result.bsize = bytes.Length;
		//	for (int i = 0; i < result.bsize; i++)
		//	{
		//		result.data[i] = bytes[i];
		//	}
		//	return result;
		//}

		//public override string ToString()
		//{
		//	return this;
		//}
	}
}
