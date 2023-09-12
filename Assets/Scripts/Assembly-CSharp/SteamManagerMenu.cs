using UnityEngine;

public static class SteamManagerMenu
{
	public static void ClearSteamWorksArchivemments()
	{
		if (Application.isPlaying && PlatformPlayerManagerSystem.IsReady)
		{
			Steam_PlatformPlayerManager.DebugClearAllAchievement();
		}
	}
}
