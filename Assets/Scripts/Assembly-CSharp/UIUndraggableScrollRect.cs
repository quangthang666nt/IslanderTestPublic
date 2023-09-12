using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIUndraggableScrollRect : ScrollRect
{
	public UnityEvent OnScrollEvent;

	public override void OnBeginDrag(PointerEventData eventData)
	{
	}

	public override void OnDrag(PointerEventData eventData)
	{
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
	}

	public override void OnScroll(PointerEventData data)
	{
		base.OnScroll(data);
		OnScrollEvent?.Invoke();
	}
}
