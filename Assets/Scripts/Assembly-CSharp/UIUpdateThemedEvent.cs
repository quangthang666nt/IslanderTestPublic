using UnityEngine;

public class UIUpdateThemedEvent : MonoBehaviour
{
	[SerializeField]
	private UIThemedEvents themedEvents;

	[SerializeField]
	private bool includeSfx = true;

	[HideInInspector]
	public string themeId = string.Empty;

	public void Confirm()
	{
		themedEvents.ChangeTheme(themeId, includeSfx, modifyPlaylist: false);
	}

	public void Cancel()
	{
		UiCanvasManager.Singleton.ToPrevious();
	}
}
