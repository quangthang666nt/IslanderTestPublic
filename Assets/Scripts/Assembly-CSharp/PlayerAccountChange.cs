using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAccountChange : MonoBehaviour
{
	public TMP_Text m_PlayerNameText;

	public ContentSizeFitter m_ContentFitter;

	public HorizontalLayoutGroup m_HorizontalLayoutGroup;

	public void SetName(string name)
	{
		m_PlayerNameText.text = name;
		m_PlayerNameText.ForceMeshUpdate();
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform.parent as RectTransform);
		m_HorizontalLayoutGroup.SetLayoutHorizontal();
		Canvas.ForceUpdateCanvases();
		m_PlayerNameText.ForceMeshUpdate();
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform.parent as RectTransform);
		m_HorizontalLayoutGroup.SetLayoutHorizontal();
		Canvas.ForceUpdateCanvases();
	}

	private void Start()
	{
		if (PlatformPlayerManagerSystem.Instance != null)
		{
			SetName(PlatformPlayerManagerSystem.Instance.GetEngagedPlayerName());
			PlatformPlayerManagerSystem.Instance.OnEngagedPlayerConnected += OnEngagedPlayerConnected;
		}
	}

	private void OnEnable()
	{
		SetName(PlatformPlayerManagerSystem.Instance.GetEngagedPlayerName());
	}

	private void OnDestroy()
	{
		if (PlatformPlayerManagerSystem.IsReady)
		{
			PlatformPlayerManagerSystem.Instance.OnEngagedPlayerConnected -= OnEngagedPlayerConnected;
		}
	}

	private void OnEngagedPlayerConnected(PlayerConnectionResult.ResultState result)
	{
	}

	public void ConnectEngagedPlayer()
	{
		PlatformPlayerManagerSystem.Instance.OnEngagedPlayerConnected += OnNewPlayerConnected;
		UiCanvasManager.Singleton.ToSigningPopup();
		PlatformPlayerManagerSystem.Instance.SetEngagedPlayerIndex(0);
		PlatformPlayerManagerSystem.Instance.ConnectEngagedPlayer();
	}

	private void OnNewPlayerConnected(PlayerConnectionResult.ResultState result)
	{
		if (PlatformPlayerManagerSystem.IsReady)
		{
			PlatformPlayerManagerSystem.Instance.OnEngagedPlayerConnected -= OnNewPlayerConnected;
		}
		if (result == PlayerConnectionResult.ResultState.Success || result == PlayerConnectionResult.ResultState.Failed)
		{
			SaveLoadManager.singleton.OpenTitleScreen();
		}
		else
		{
			UiCanvasManager.Singleton.ToPrevious(addToStack: false);
		}
	}

	private void Update()
	{
		if (InputManager.Singleton != null && InputManager.Singleton.InputDataCurrent.bAccountChangeRequested && (UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.MenuNoCurrent || UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.MenuWithCurrent))
		{
			ConnectEngagedPlayer();
		}
	}
}
