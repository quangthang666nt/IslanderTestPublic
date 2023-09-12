using UnityEngine;

public class CustomCursor : MonoBehaviour
{
	[SerializeField]
	private Texture2D texCustomCursor;

	[SerializeField]
	private int iCursorScaleX = 16;

	[SerializeField]
	private int iCursorScaleY = 16;

	[SerializeField]
	private float fClickScale = 1f;

	[SerializeField]
	private bool bHidecursor = true;

	private void Start()
	{
		iCursorScaleX = Mathf.RoundToInt((float)iCursorScaleX / 1080f * (float)Screen.height);
		iCursorScaleY = Mathf.RoundToInt((float)iCursorScaleY / 1080f * (float)Screen.height);
	}

	private void OnGUI()
	{
		int num = iCursorScaleX;
		int num2 = iCursorScaleY;
		if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
		{
			num = (int)((float)num * fClickScale);
			num2 = (int)((float)num2 * fClickScale);
		}
		GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, num, num2), texCustomCursor);
	}

	private void OnDisable()
	{
		Cursor.visible = true;
	}
}
