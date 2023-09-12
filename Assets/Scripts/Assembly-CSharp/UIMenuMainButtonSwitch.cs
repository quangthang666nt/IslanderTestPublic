using UnityEngine;

public class UIMenuMainButtonSwitch : GenericMenuList
{
	public GameObject GoNoCurrent;

	public GameObject GoWithCurrent;

	public GameObject m_SaveButtonParent;

	protected override void OnEnable()
	{
		if (UiCanvasManager.Singleton == null)
		{
			return;
		}
		if (UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.MenuNoCurrent)
		{
			GoWithCurrent.SetActive(value: false);
			GoNoCurrent.SetActive(value: true);
		}
		if (UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.MenuWithCurrent)
		{
			GoWithCurrent.SetActive(value: true);
			GoNoCurrent.SetActive(value: false);
			if (LocalGameManager.singleton.GameState == LocalGameManager.EGameState.InGame)
			{
				m_SaveButtonParent.gameObject.SetActive(value: true);
			}
			else
			{
				m_SaveButtonParent.gameObject.SetActive(value: false);
			}
		}
		base.OnEnable();
	}
}
