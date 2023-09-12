using UnityEngine;

public class AccountDisconnectionPrompt : MonoBehaviour
{
	private void LateUpdate()
	{
		if (InputManager.Singleton.InputDataCurrent.bUIConfirm && UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.AccountDisconnectionPrompt)
		{
			SaveLoadManager.singleton.OpenTitleScreen();
		}
	}
}
