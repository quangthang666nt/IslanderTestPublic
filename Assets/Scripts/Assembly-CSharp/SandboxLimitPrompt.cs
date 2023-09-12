using UnityEngine;

public class SandboxLimitPrompt : MonoBehaviour
{
	private void LateUpdate()
	{
		if (InputManager.Singleton.InputDataCurrent.bUIConfirm && UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.SandboxLimitPrompt)
		{
			UiCanvasManager.Singleton.ToPrevious(addToStack: false);
		}
	}
}
