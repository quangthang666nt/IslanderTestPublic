using UnityEngine;

public class ArchiveIslandPrompt : MonoBehaviour
{
	private void Update()
	{
	}

	public void ExecSaveIsland()
	{
		ArchiveManager.singleton.TrySaveIsland();
	}

	public void ExecLoadIsland()
	{
		UiCanvasManager.Singleton.ToArchiveIslandLoadPrompt();
	}
}
