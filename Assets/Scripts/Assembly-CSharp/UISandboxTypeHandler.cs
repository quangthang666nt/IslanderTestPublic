using UnityEngine;

public class UISandboxTypeHandler : MonoBehaviour
{
	public RectTransform cursor;

	public void ActiveCursor()
	{
		if ((bool)cursor)
		{
			cursor.gameObject.SetActive(value: true);
		}
	}

	public void DeactiveCursor()
	{
		if ((bool)cursor)
		{
			cursor.gameObject.SetActive(value: false);
		}
	}
}
