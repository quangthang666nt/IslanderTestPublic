using UnityEngine;
using UnityEngine.EventSystems;

public class UIBuildingTooltip : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public Building building;

	public void OnPointerEnter(PointerEventData e)
	{
		if (UiBuildingButtonManager.singleton.GoBuildingPreview != base.gameObject && UiBuildingButtonManager.singleton.GoBuildingPreview == null && UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.MenuWithCurrent && UiCanvasManager.Singleton.UIState != 0)
		{
			UITooltip.Singleton.Enable(building.strBuildingName, "");
		}
	}

	public void OnPointerExit(PointerEventData e)
	{
		UITooltip.Singleton.Disable();
	}

	public void OnPointerDown(PointerEventData e)
	{
		if (e.button == PointerEventData.InputButton.Left && UiBuildingButtonManager.singleton.GoBuildingPreview == null && UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.MenuWithCurrent && UiCanvasManager.Singleton.UIState != 0)
		{
			UITooltip.Singleton.EnableImmediate(building.strBuildingName, "");
		}
	}
}
