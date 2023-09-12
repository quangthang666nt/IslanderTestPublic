using UnityEngine;
using UnityEngine.UI;

public class DebugUIState : MonoBehaviour
{
	public UiCanvasManager canvasmanager;

	public LocalGameManager lclmanager;

	public Text lclState;

	public Text uiState;

	public Text gamemode;

	private void Update()
	{
		lclState.text = "GAME STATE: " + lclmanager.GameState;
		uiState.text = "UI STATE: " + canvasmanager.UIState;
		gamemode.text = "GAME MODE: " + lclmanager.GameMode;
	}
}
