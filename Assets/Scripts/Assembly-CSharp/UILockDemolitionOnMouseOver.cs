using UnityEngine;
using UnityEngine.EventSystems;

public class UILockDemolitionOnMouseOver : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public static UILockDemolitionOnMouseOver singleton;

	public bool bIsBackgroundBar;

	private bool bMouseOver;

	public bool BMouseOver => bMouseOver;

	private void Awake()
	{
		if (bIsBackgroundBar)
		{
			singleton = this;
		}
	}

	public void OnPointerEnter(PointerEventData e)
	{
		DemolitionController.Lock();
		bMouseOver = true;
	}

	public void OnPointerExit(PointerEventData e)
	{
		DemolitionController.Unlock();
		bMouseOver = false;
	}

	private void OnDisable()
	{
		bMouseOver = false;
	}

	public static bool BIsMouseOver()
	{
		if (singleton != null)
		{
			return singleton.bMouseOver;
		}
		return false;
	}
}
