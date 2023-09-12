using UnityEngine;
using UnityEngine.EventSystems;

public class UINewIslandButtonSound : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	private void PlayButtonClick()
	{
		AudioManager.singleton.PlayButtonClick();
	}

	private void PlayButtonDisabled()
	{
		AudioManager.singleton.PlayErrorPrompt();
	}

	public void OnPointerClick(PointerEventData e)
	{
		if (LocalGameManager.singleton.BNextIslandAvailable || LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			PlayButtonClick();
		}
		else
		{
			PlayButtonDisabled();
		}
	}
}
