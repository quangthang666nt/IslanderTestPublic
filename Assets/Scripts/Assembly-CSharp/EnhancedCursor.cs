using UnityEngine;

public class EnhancedCursor : MonoBehaviour
{
	private const float BASE_WIDTH = 1920f;

	private const float BASE_HEIGHT = 1080f;

	[SerializeField]
	private Vector3 offset;

	[SerializeField]
	private GameObject displaceHandler;

	[SerializeField]
	private GameObject Defaultcursor;

	[SerializeField]
	private GameObject demolitionCursor;

	private void Start()
	{
		demolitionCursor.SetActive(value: false);
		Defaultcursor.SetActive(value: true);
	}

	public void StartDemolitionMode()
	{
		demolitionCursor.SetActive(value: true);
		Defaultcursor.SetActive(value: false);
	}

	public void SetDefaultCursor()
	{
		demolitionCursor.SetActive(value: false);
		Defaultcursor.SetActive(value: true);
	}

	public void ShowCursor(bool isvisible)
	{
		displaceHandler.SetActive(isvisible);
	}

	private void LateUpdate()
	{
		Cursor.visible = false;
		displaceHandler.transform.position = Input.mousePosition + GetOffset();
	}

	private Vector3 GetOffset()
	{
		Vector3 result = offset;
		result.x *= (float)Screen.width / 1920f;
		result.y *= (float)Screen.height / 1080f;
		return result;
	}
}
