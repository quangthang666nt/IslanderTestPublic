using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSelectEventHandler : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler
{
	[SerializeField]
	private UnityEvent onSelect;

	[SerializeField]
	private UnityEvent onDeselect;

	public void OnSelect(BaseEventData eventData)
	{
		onSelect?.Invoke();
	}

	public void OnDeselect(BaseEventData eventData)
	{
		onDeselect?.Invoke();
	}
}
