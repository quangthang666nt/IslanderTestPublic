using UnityEngine;

public class ArchivePhotoPrompt : MonoBehaviour
{
	private ushort islandId;

	private bool bReady;

	private void OnDisable()
	{
		bReady = false;
	}

	private void Update()
	{
		if (bReady && InputManager.Singleton.InputDataCurrent.bUIConfirm)
		{
			TakeScreenshot();
		}
	}

	public void SetId(ushort islandId)
	{
		this.islandId = islandId;
		bReady = true;
	}

	public void TakeScreenshot()
	{
		UIPlayButtonSoundOnClick component = GetComponent<UIPlayButtonSoundOnClick>();
		if ((bool)component)
		{
			component.PlayButtonClick();
		}
		ArchiveManager.TakeScreenShot(islandId);
		UiCanvasManager.Singleton.ToArchiveIslandSavePrompt();
	}
}
