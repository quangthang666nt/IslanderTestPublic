using UnityEngine;
using UnityEngine.EventSystems;

public class UiUndoButtonTrigger : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	private SaveLoadManager saveLoadManager;

	private void Start()
	{
		saveLoadManager = SaveLoadManager.singleton;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		saveLoadManager.Undo();
	}
}
