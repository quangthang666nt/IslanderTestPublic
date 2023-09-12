using UnityEngine;

public class UITextBackground : MonoBehaviour
{
	public RectTransform transTarget;

	public float fPadding = 5f;

	private Vector3 v3Offset = new Vector3(1f, -1f, 0f);

	private RectTransform transThis;

	private void Awake()
	{
		transThis = GetComponent<RectTransform>();
	}

	public void Update()
	{
		transThis.position = UICameraSpaceHelper.ScreenPointToCanvasPoint(UICameraSpaceHelper.CanvasPointToScreenPoint(transTarget.position) - v3Offset * (fPadding / 2f));
		transThis.sizeDelta = transTarget.sizeDelta + Vector2.one * fPadding;
	}
}
