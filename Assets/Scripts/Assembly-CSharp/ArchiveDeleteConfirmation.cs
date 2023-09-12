using System.Collections;
using UnityEngine;

public class ArchiveDeleteConfirmation : MonoBehaviour
{
	private ushort islandId;

	private bool fromLoad;

	private void Start()
	{
		GetComponent<UIElement>().eventOnDeactivation.AddListener(Deactivate);
	}

	private void Deactivate()
	{
		StopAllCoroutines();
	}

	public void SetArchiveIslandId(ushort id, bool fromLoad)
	{
		islandId = id;
		this.fromLoad = fromLoad;
	}

	public void Back()
	{
		UiCanvasManager.Singleton.ToPrevious();
	}

	public void DeleteIsland()
	{
		ArchiveManager.DeleteEntry(islandId);
		if (fromLoad)
		{
			UiCanvasManager.Singleton.ToPrevious();
		}
		else if (ArchiveManager.singleton.bScreenshotBeingDeleted)
		{
			StartCoroutine(WaitingScreenshotForBeingDeleted());
		}
		else
		{
			ArchiveManager.singleton.TrySaveIsland();
		}
	}

	private IEnumerator WaitingScreenshotForBeingDeleted()
	{
		while (ArchiveManager.singleton.bScreenshotBeingDeleted)
		{
			yield return null;
		}
		ArchiveManager.singleton.TrySaveIsland();
	}
}
