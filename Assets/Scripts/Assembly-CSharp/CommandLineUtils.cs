using UnityEngine;

public static class CommandLineUtils
{
	[CommandLine("time_scale", "Change the time scale of the game to the decimal value passed. Note that it can be overriden by the game", null, true)]
	private static void DebugTimeScale(float timescale)
	{
		Time.timeScale = timescale;
		Console.Log("Time Scale is now: " + timescale);
	}
}
