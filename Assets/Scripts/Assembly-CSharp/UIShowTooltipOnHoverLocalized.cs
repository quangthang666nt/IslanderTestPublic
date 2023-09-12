using I2.Loc;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIShowTooltipOnHoverLocalized : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public LocalizedString strHeader;

	public LocalizedString strText;

	public bool bDisableOnClick = true;

	public void OnPointerDown(PointerEventData e)
	{
		if (bDisableOnClick)
		{
			UITooltip.Singleton.Disable();
		}
	}

	public void OnPointerEnter(PointerEventData e)
	{
		UITooltip.Singleton.Enable(strHeader, strText);
	}

	public void OnPointerExit(PointerEventData e)
	{
		UITooltip.Singleton.Disable();
	}
}
