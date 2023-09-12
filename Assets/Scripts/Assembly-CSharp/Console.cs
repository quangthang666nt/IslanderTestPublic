using UnityEngine;

public static class Console
{
	public static void Log(string line, string color = "", Object context = null, bool logToUnity = true)
	{
		if (logToUnity)
		{
			Debug.Log(line, context);
		}
		if (InGameConsole.Instance != null)
		{
			InGameConsole.Instance.AddToLog((color != "") ? ("<color=" + color + ">" + line + "</color>") : line);
		}
	}

	public static void LogWarning(string line, Object context = null, bool logToUnity = true)
	{
		if (logToUnity)
		{
			Debug.LogWarning(line, context);
		}
		if (InGameConsole.Instance != null)
		{
			InGameConsole.Instance.AddToLog(line);
		}
	}

	public static void LogError(string line, Object context = null, bool logToUnity = true)
	{
		if (logToUnity)
		{
			Debug.LogError(line, context);
		}
		if (InGameConsole.Instance != null)
		{
			InGameConsole.Instance.AddToLog(line);
		}
	}
}
