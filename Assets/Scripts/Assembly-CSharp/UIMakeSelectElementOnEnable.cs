using UnityEngine;
using UnityEngine.EventSystems;

public class UIMakeSelectElementOnEnable : MonoBehaviour
{
	private bool dirty;

	private void OnEnable()
	{
		dirty = true;
	}

	private void Update()
	{
		if (dirty)
		{
			EventSystem.current.SetSelectedGameObject(base.gameObject);
			dirty = false;
		}
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			EventSystem.current.SetSelectedGameObject(base.gameObject);
		}
	}
}
