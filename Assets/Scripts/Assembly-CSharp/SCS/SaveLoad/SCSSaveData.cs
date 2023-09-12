using System;
using System.Collections.Generic;
using System.Linq;
using SCS.Utils;
using UnityEngine;

namespace SCS.SaveLoad
{
	public class SCSSaveData
	{
		private struct SavePiece
		{
			public SCSString32 name;

			public int offset;

			public int size;

			public SavePiece(string name)
			{
				this = default(SavePiece);
				//this.name = name;
			}
		}

		private int pieceCount;

		private Dictionary<string, SavePiece> pieces;

		private Dictionary<string, byte[]> brokenPieces;

		private byte[] buffer;

		private int minSize;

		public Dictionary<string, byte[]> GetPieces
		{
			get
			{
				if (brokenPieces != null)
				{
					return brokenPieces;
				}
				if (pieces != null)
				{
					Dictionary<string, byte[]> dictionary = new Dictionary<string, byte[]>();
					{
						foreach (KeyValuePair<string, SavePiece> piece in pieces)
						{
							dictionary.Add(piece.Key, GetPiece(piece.Value));
						}
						return dictionary;
					}
				}
				return null;
			}
		}

		private bool isBufferBroken => buffer == null;

		//public unsafe int GetStartOfRawData()
		//{
		//	return 4 + sizeof(SavePiece) * pieceCount;
		//}

		public SCSSaveData()
		{
			InitializeEmpty();
		}

		public SCSSaveData(int minSize = 0)
		{
			this.minSize = minSize;
			InitializeEmpty();
		}

		private void InitializeEmpty()
		{
			pieceCount = 0;
			pieces = new Dictionary<string, SavePiece>();
			buffer = null;
			brokenPieces = new Dictionary<string, byte[]>();
		}

		public  void Load(byte[] data)
		{
			if (data == null || data.Length < 4)
			{
				InitializeEmpty();
				return;
			}
			buffer = data;
			pieceCount = BitConverter.ToInt32(data, 0);
			pieces = new Dictionary<string, SavePiece>();
			brokenPieces = null;
			for (int i = 0; i < pieceCount; i++)
			{
	/*			byte* ptr = null;
				fixed (byte* ptr2 = data)
				{
					ptr = ptr2;
					ptr += 4 + sizeof(SavePiece) * i;
				}
				SavePiece value = *(SavePiece*)ptr;
				pieces.Add(value.name, value);*/
			}
		}

		public List<string> FindPieces(string search)
		{
			Break();
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, byte[]> brokenPiece in brokenPieces)
			{
				if (brokenPiece.Key.Contains(search))
				{
					list.Add(brokenPiece.Key);
				}
			}
			return list;
		}
		
		public byte[] GetPiece(string name)
		{
			if (isBufferBroken)
			{
				if (!brokenPieces.ContainsKey(name))
				{
					return null;
				}
				return brokenPieces[name];
			}
			if (!pieces.ContainsKey(name))
			{
				return null;
			}
			return GetPiece(pieces[name]);
		}

		public byte[][] GetAllPieces()
		{
			if (isBufferBroken)
			{
				return brokenPieces.Values.ToArray();
			}
			SavePiece[] array = pieces.Values.ToArray();
			byte[][] array2 = new byte[array.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = GetPiece(array[i]);
			}
			return array2;
		}

		public T[] GetAllPiecesAs<T>(T defaultValue = default(T))
		{
			byte[][] allPieces = GetAllPieces();
			T[] array = new T[allPieces.Length];
			for (int i = 0; i < allPieces.Length; i++)
			{
				if (allPieces[i] == null)
				{
					array[i] = defaultValue;
				}
				array[i] = SCSByteConverter.Convert<T>(allPieces[i]);
			}
			return array;
		}

		private byte[] GetPiece(SavePiece piece)
		{
			byte[] array = new byte[piece.size];
			Array.Copy(buffer, piece.offset, array, 0, piece.size);
			return array;
		}

		public T GetPieceAs<T>(string name, T defaultValue = default(T))
		{
			if (typeof(T) == typeof(byte[]))
			{
				throw new ArgumentException("Cannot use GetPieceAs for bytes! Use GetPiece instead!");
			}
			byte[] piece = GetPiece(name);
			if (piece == null)
			{
				return defaultValue;
			}
			return SCSByteConverter.Convert<T>(piece);
		}

		//public byte[] GetRawBuffer(bool includePieceDefinition = true)
		//{
		//	if (isBufferBroken)
		//	{
		//		RecreateBrokenBuffer();
		//	}
		//	if (includePieceDefinition)
		//	{
		//		byte[] array = new byte[buffer.Length];
		//		Array.Copy(buffer, array, array.Length);
		//		return array;
		//	}
		//	/*int startOfRawData = GetStartOfRawData();*/
		//	/*byte[] array2 = new byte[buffer.Length - startOfRawData];
		//	Array.Copy(buffer, startOfRawData, array2, 0, array2.Length);
		//	return array2;*/
		//}

		private void Break()
		{
			if (isBufferBroken)
			{
				return;
			}
			brokenPieces = new Dictionary<string, byte[]>();
			foreach (SavePiece value in pieces.Values)
			{
				//brokenPieces.Add(value.name, GetPiece(value));
			}
			buffer = null;
		}

		public void RemovePiece(string name)
		{
			Break();
			brokenPieces.Remove(name);
			pieceCount = brokenPieces.Count;
		}

		public void AddOrUpdatePiece(string name, byte[] data)
		{
			Break();
			brokenPieces[name] = data;
			pieceCount = brokenPieces.Count;
		}

		public void AddOrUpdatePiece<T>(string name, T data)
		{
			AddOrUpdatePiece(name, SCSByteConverter.Convert(data));
		}

		//private unsafe void RecreateBrokenBuffer()
		//{
		//	if (!isBufferBroken)
		//	{
		//		return;
		//	}
		//	List<KeyValuePair<string, byte[]>> list = new List<KeyValuePair<string, byte[]>>(brokenPieces);
		//	pieces.Clear();
		//	brokenPieces.Clear();
		//	brokenPieces = null;
		//	int num = GetStartOfRawData();
		//	byte[] array = new byte[num];
		//	Array.Copy(BitConverter.GetBytes(list.Count), array, 4);
		//	int num2 = 4;
		//	SavePiece[] array2 = new SavePiece[list.Count];
		//	for (int i = 0; i < list.Count; i++)
		//	{
		//		SavePiece savePiece = new SavePiece(list[i].Key);
		//		savePiece.offset = num;
		//		savePiece.size = list[i].Value.Length;
		//		num += savePiece.size;
		//		array2[i] = savePiece;
		//		pieces.Add(savePiece.name, savePiece);
		//		byte* ptr = (byte*)(&savePiece);
		//		for (int j = 0; j < sizeof(SavePiece); j++)
		//		{
		//			array[num2++] = *ptr;
		//			ptr++;
		//		}
		//	}
		//	buffer = new byte[Mathf.Max(num, minSize)];
		//	Array.Copy(array, buffer, array.Length);
		//	for (int k = 0; k < array2.Length; k++)
		//	{
		//		Array.Copy(list[k].Value, 0, buffer, array2[k].offset, array2[k].size);
		//	}
		//}

		public bool PieceExists(string name)
		{
			if (isBufferBroken)
			{
				return brokenPieces.ContainsKey(name);
			}
			return pieces.ContainsKey(name);
		}
	}
}
