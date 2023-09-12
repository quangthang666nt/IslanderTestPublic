using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIPageFitter : MonoBehaviour
{
	[SerializeField]
	private UIPageSizeCalculator page;

	private RectTransform rTransform;

	private Vector2 size;

	private void Start()
	{
		rTransform = GetComponent<RectTransform>();
		size = rTransform.sizeDelta;
	}

	private void Update()
	{
		size.x = page.GetWidth();
		rTransform.sizeDelta = size;
	}
}
