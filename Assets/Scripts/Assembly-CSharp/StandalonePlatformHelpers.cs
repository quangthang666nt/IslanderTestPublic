using System;
using System.IO;
using UnityEngine;

public static class StandalonePlatformHelpers
{
	public class LoadRequest
	{
		public string FileName;

		public string BufferName;

		public byte[] Data;

		public Action<string, LoadResult, byte[]> LoadCallback;

		public FileStream FileStream;

		public LoadResult CurrentResult;

		public void Reset()
		{
			lock (this)
			{
				CurrentResult = LoadResult.FailOther;
				LoadCallback = null;
				BufferName = "";
				FileName = "";
				if (FileStream != null)
				{
					FileStream.Close();
					FileStream = null;
				}
			}
		}
	}

	public class SaveRequest
	{
		public string FileName;

		public string BufferName;

		public Action<string, bool> SaveCallback;

		public FileStream FileStream;

		public bool Done;

		public bool Cancelled;

		public void Reset()
		{
			lock (this)
			{
				Done = false;
				SaveCallback = null;
				BufferName = "";
				FileName = "";
				Cancelled = false;
				if (FileStream != null)
				{
					FileStream.Close();
					FileStream = null;
				}
			}
		}

		public void Cancel()
		{
			Reset();
			Cancelled = true;
		}
	}

	public static bool DeleteData(string dataName, Action<string, bool> deleteCallback)
	{
		string path = Path.Combine(Application.persistentDataPath, dataName);
		if (File.Exists(path))
		{
			File.Delete(path);
			deleteCallback?.Invoke(dataName, arg2: true);
			return true;
		}
		deleteCallback?.Invoke(dataName, arg2: false);
		return false;
	}

	public static LoadRequest LoadData(string dataName, Action<string, LoadResult, byte[]> loadCallback)
	{
		string text = Path.Combine(Application.persistentDataPath, dataName);
		LoadRequest loadRequest = new LoadRequest
		{
			FileName = dataName,
			BufferName = text,
			LoadCallback = loadCallback,
			CurrentResult = LoadResult.InProgress
		};
		if (!File.Exists(text))
		{
			loadRequest.CurrentResult = LoadResult.FailFileNotFound;
			return loadRequest;
		}
		FileStream fileStream = new FileStream(loadRequest.BufferName, FileMode.Open, FileAccess.Read, FileShare.Read);
		loadRequest.Data = new byte[fileStream.Length];
		loadRequest.FileStream = fileStream;
		fileStream.BeginRead(loadRequest.Data, 0, loadRequest.Data.Length, OnLoadCompleted, loadRequest);
		return loadRequest;
	}

	private static void OnLoadCompleted(IAsyncResult ar)
	{
		LoadRequest loadRequest = (LoadRequest)ar.AsyncState;
		lock (loadRequest)
		{
			if (ar.IsCompleted)
			{
				loadRequest.CurrentResult = LoadResult.Success;
			}
			else
			{
				Debug.Log("[Steam_PlatformPlayerManager] Couldn't complete load request for file " + loadRequest.FileName);
				loadRequest.CurrentResult = LoadResult.FailOther;
			}
			loadRequest.FileStream.Close();
			loadRequest.FileStream.Dispose();
			loadRequest.FileStream = null;
		}
	}

	public static SaveRequest SaveData(string dataName, byte[] data, Action<string, bool> saveCallback)
	{
		SaveRequest saveRequest = new SaveRequest
		{
			FileName = dataName,
			BufferName = Path.Combine(Application.persistentDataPath, dataName),
			SaveCallback = saveCallback,
			Done = false
		};
		if (!Directory.Exists(Path.GetDirectoryName(saveRequest.BufferName)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(saveRequest.BufferName));
		}
		FileStream fileStream = (saveRequest.FileStream = new FileStream(saveRequest.BufferName, FileMode.Create, FileAccess.Write));
		fileStream.Flush();
		fileStream.BeginWrite(data, 0, data.Length, OnSaveCompleted, saveRequest);
		return saveRequest;
	}

	private static void OnSaveCompleted(IAsyncResult ar)
	{
		SaveRequest saveRequest = (SaveRequest)ar.AsyncState;
		lock (saveRequest)
		{
			if (!saveRequest.Cancelled)
			{
				if (ar.IsCompleted)
				{
					saveRequest.Done = true;
				}
				else
				{
					Debug.Log("[Steam_PlatformPlayerManager] Couldn't complete save request for file " + saveRequest.FileName);
					saveRequest.Done = false;
				}
				saveRequest.FileStream.EndWrite(ar);
				saveRequest.FileStream.Close();
				saveRequest.FileStream = null;
			}
		}
	}
}
