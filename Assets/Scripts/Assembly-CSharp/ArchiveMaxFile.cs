using UnityEngine;

public class ArchiveMaxFile : MonoBehaviour
{
	public void Back()
	{
		UiCanvasManager.Singleton.ToPrevious();
	}

	public void DeleteOldIsland()
	{
		UiCanvasManager.Singleton.ToArchiveIslandDeletePrompt();
	}
}
