using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScrollbarWithDragEvent : Scrollbar
{
	public UnityEvent OnDragEvent;

	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		OnDragEvent?.Invoke();
	}
}
