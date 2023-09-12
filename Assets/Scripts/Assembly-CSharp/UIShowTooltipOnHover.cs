using UnityEngine;
using UnityEngine.EventSystems;

public class UIShowTooltipOnHover : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public string strHeader;

	[TextArea]
	public string strText;

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
