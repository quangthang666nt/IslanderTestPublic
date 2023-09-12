using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Windows_PlatformPlayerManager : PlatformPlayerManager
{
	private List<StandalonePlatformHelpers.LoadRequest> m_LoadRequests = new List<StandalonePlatformHelpers.LoadRequest>();

	private List<StandalonePlatformHelpers.SaveRequest> m_SaveRequests = new List<StandalonePlatformHelpers.SaveRequest>();

	private static string m_RandomPlayerName = "RandomPlayerName";

	private readonly string playerId = Guid.NewGuid().ToString();

	private static readonly string[] c_Names = new string[7] { "2WMWMWMWMWMWVWM", "PlaceHolder1", "Aba", "Oka Noo", "Calimero", "BLAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", "A" };

	[CommandLine("change_player_name", "", null, true)]
	public static void ChangePlayerName(string name)
	{
		m_RandomPlayerName = c_Names[UnityEngine.Random.Range(0, c_Names.Length)];
		PlatformPlayerManagerSystem.Instance.ConnectEngagedPlayer();
	}

	public override string GetMainPlayerName()
	{
		return m_RandomPlayerName;
	}

	[CommandLine("system_panic", "", null, true)]
	public static void TriggerSystemPanic()
	{
		(PlatformPlayerManagerSystem.Instance.PlatformPlayerManager as Windows_PlatformPlayerManager).OnSystemPanicCall();
	}

	public override string GetPlatformString()
	{
		return "WINDOWS";
	}

	public override string GetPlayerName(int index)
	{
		return "Placeholder" + Process.GetCurrentProcess().Id + ((index == 0) ? "" : ("(" + (index + 1) + ")"));
	}

	public override string GetOnlinePlatformString()
	{
		return "standalone";
	}

	public override void Update()
	{
		for (int num = m_LoadRequests.Count - 1; num >= 0; num--)
		{
			if (m_LoadRequests[num].CurrentResult != LoadResult.InProgress)
			{
				if (m_LoadRequests[num].LoadCallback != null)
				{
					m_LoadRequests[num].LoadCallback(m_LoadRequests[num].FileName, m_LoadRequests[num].CurrentResult, m_LoadRequests[num].Data);
				}
				m_LoadRequests[num].Reset();
				m_LoadRequests.RemoveAt(num);
			}
		}
		for (int num2 = m_SaveRequests.Count - 1; num2 >= 0; num2--)
		{
			if (m_SaveRequests[num2].Cancelled)
			{
				m_SaveRequests[num2].Reset();
				m_SaveRequests.RemoveAt(num2);
			}
			else if (m_SaveRequests[num2].Done)
			{
				if (m_SaveRequests[num2].SaveCallback != null)
				{
					m_SaveRequests[num2].SaveCallback(m_SaveRequests[num2].FileName, m_SaveRequests[num2].Done);
				}
				m_SaveRequests[num2].Reset();
				m_SaveRequests.RemoveAt(num2);
			}
		}
	}

	public override bool SaveData(string dataName, byte[] data, Action<string, bool> saveCallback, int slot = 0)
	{
		for (int num = m_SaveRequests.Count - 1; num >= 0; num--)
		{
			if (m_SaveRequests[num].FileName == dataName)
			{
				UnityEngine.Debug.Log("[Windows] File already saving...");
				return true;
			}
		}
		StandalonePlatformHelpers.SaveRequest saveRequest = StandalonePlatformHelpers.SaveData(dataName, data, saveCallback);
		if (!saveRequest.Done)
		{
			m_SaveRequests.Add(saveRequest);
			return m_SaveRequests[m_SaveRequests.Count - 1].Done;
		}
		if (saveRequest.SaveCallback != null)
		{
			saveRequest.SaveCallback(saveRequest.FileName, saveRequest.Done);
		}
		return true;
	}

	public override LoadResult LoadData(string dataName, Action<string, LoadResult, byte[]> loadCallback, int slot = 0)
	{
		StandalonePlatformHelpers.LoadRequest loadRequest = StandalonePlatformHelpers.LoadData(dataName, loadCallback);
		if (loadRequest.CurrentResult == LoadResult.InProgress)
		{
			m_LoadRequests.Add(loadRequest);
			return m_LoadRequests[m_LoadRequests.Count - 1].CurrentResult;
		}
		if (loadRequest.LoadCallback != null)
		{
			loadRequest.LoadCallback(loadRequest.FileName, loadRequest.CurrentResult, loadRequest.Data);
		}
		return loadRequest.CurrentResult;
	}

	public override bool DeleteData(string dataName, Action<string, bool> deleteCallback, int slot = 0)
	{
		return StandalonePlatformHelpers.DeleteData(dataName, deleteCallback);
	}
}
