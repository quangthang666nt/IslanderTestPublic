using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIFitRectTransform : MonoBehaviour
{
	public RectTransform reference;

	public Vector2 paddingXY;

	private RectTransform thisRT;

	private void Start()
	{
		thisRT = GetComponent<RectTransform>();
	}

	private void Update()
	{
		thisRT.sizeDelta = reference.sizeDelta + paddingXY;
	}
}
