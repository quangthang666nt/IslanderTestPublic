using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlayButtonSoundOnClick : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	private enum SoundType
	{
		UISound = 0,
		ButtonSound = 1
	}

	[SerializeField]
	private SoundType soundType;

	[SerializeField]
	private bool soundOnlyControlledByClick;

	public void PlayButtonClick()
	{
		if (soundType == SoundType.UISound && AudioManager.singleton != null)
		{
			AudioManager.singleton.PlayMenuClick();
		}
		if (soundType == SoundType.ButtonSound && AudioManager.singleton != null)
		{
			AudioManager.singleton.PlayButtonClick();
		}
	}

	public void OnPointerClick(PointerEventData e)
	{
		if (soundOnlyControlledByClick)
		{
			PlayButtonClick();
		}
	}
}
