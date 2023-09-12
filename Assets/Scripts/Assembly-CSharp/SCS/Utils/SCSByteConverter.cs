using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace SCS.Utils
{
	public static class SCSByteConverter
	{
		public struct Conversion
		{
			public Func<object, byte[]> ToBytes;

			public Func<byte[], object> FromBytes;

			public Conversion(Func<object, byte[]> toBytes, Func<byte[], object> fromBytes)
			{
				ToBytes = toBytes;
				FromBytes = fromBytes;
			}
		}

		private static Dictionary<Type, Conversion> conversionDictionary;

		public static void AddConversion(Type type, Conversion conversion)
		{
			conversionDictionary[type] = conversion;
		}

		static SCSByteConverter()
		{
			conversionDictionary = new Dictionary<Type, Conversion>();
			conversionDictionary.Add(typeof(bool), new Conversion((object value) => BitConverter.GetBytes((bool)value), (byte[] value) => BitConverter.ToBoolean(value, 0)));
			conversionDictionary.Add(typeof(byte), new Conversion((object value) => new byte[1] { (byte)value }, (byte[] value) => value[0]));
			conversionDictionary.Add(typeof(sbyte), new Conversion((object value) => new byte[1] { (byte)((byte)value + 127) }, (byte[] value) => value[0] - 127));
			conversionDictionary.Add(typeof(ushort), new Conversion((object value) => BitConverter.GetBytes((ushort)value), (byte[] value) => BitConverter.ToUInt16(value, 0)));
			conversionDictionary.Add(typeof(short), new Conversion((object value) => BitConverter.GetBytes((short)value), (byte[] value) => BitConverter.ToInt16(value, 0)));
			conversionDictionary.Add(typeof(uint), new Conversion((object value) => BitConverter.GetBytes((uint)value), (byte[] value) => BitConverter.ToUInt32(value, 0)));
			conversionDictionary.Add(typeof(int), new Conversion((object value) => BitConverter.GetBytes((int)value), (byte[] value) => BitConverter.ToInt32(value, 0)));
			conversionDictionary.Add(typeof(ulong), new Conversion((object value) => BitConverter.GetBytes((ulong)value), (byte[] value) => BitConverter.ToUInt64(value, 0)));
			conversionDictionary.Add(typeof(long), new Conversion((object value) => BitConverter.GetBytes((long)value), (byte[] value) => BitConverter.ToInt64(value, 0)));
			conversionDictionary.Add(typeof(float), new Conversion((object value) => BitConverter.GetBytes((float)value), (byte[] value) => BitConverter.ToSingle(value, 0)));
			conversionDictionary.Add(typeof(double), new Conversion((object value) => BitConverter.GetBytes((double)value), (byte[] value) => BitConverter.ToDouble(value, 0)));
			conversionDictionary.Add(typeof(byte[]), new Conversion((object value) => (byte[])value, (byte[] value) => value));
			conversionDictionary.Add(typeof(string), new Conversion((object value) => Encoding.UTF8.GetBytes((string)value), (byte[] value) => Encoding.UTF8.GetString(value)));
		}

		public static byte[] Convert(object o)
		{
			try
			{
				if (conversionDictionary.ContainsKey(o.GetType()))
				{
					return conversionDictionary[o.GetType()].ToBytes(o);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("[SCSByteConverter] Convert error!! The object you're trying to convert doesn't implement GetType(): " + ex.ToString());
			}
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, o);
			return memoryStream.ToArray();
		}

		public static T Convert<T>(byte[] bytes)
		{
			if (conversionDictionary.ContainsKey(typeof(T)))
			{
				return (T)conversionDictionary[typeof(T)].FromBytes(bytes);
			}
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using MemoryStream serializationStream = new MemoryStream(bytes);
			return (T)binaryFormatter.Deserialize(serializationStream);
		}
	}
}
