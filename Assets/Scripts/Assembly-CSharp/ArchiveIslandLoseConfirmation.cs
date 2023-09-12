using UnityEngine;

public class ArchiveIslandLoseConfirmation : MonoBehaviour
{
	private ushort nextIslandId;

	public void SetNextIslandId(ushort nextIslandId)
	{
		this.nextIslandId = nextIslandId;
	}

	public void LoadNextIsland()
	{
		ArchiveManager.LoadEntry(nextIslandId);
	}

	public void Back()
	{
		UiCanvasManager.Singleton.ToPrevious();
	}
}
