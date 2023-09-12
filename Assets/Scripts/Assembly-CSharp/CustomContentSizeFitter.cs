using UnityEngine;
using UnityEngine.UI;

public class CustomContentSizeFitter : ContentSizeFitter
{
	[SerializeField]
	private Vector2 widthLimit = new Vector2(-1f, -1f);

	[SerializeField]
	private Vector2 heightLimit = new Vector2(-1f, -1f);

	public override void SetLayoutHorizontal()
	{
		base.SetLayoutHorizontal();
		if (!(widthLimit.x >= 0f) || !(widthLimit.y >= 0f) || !(widthLimit.y < widthLimit.x))
		{
			RectTransform component = GetComponent<RectTransform>();
			Vector2 sizeDelta = component.sizeDelta;
			if (widthLimit.x >= 0f && sizeDelta.x < widthLimit.x)
			{
				sizeDelta.x = widthLimit.x;
			}
			if (widthLimit.y >= 0f && sizeDelta.x > widthLimit.y)
			{
				sizeDelta.x = widthLimit.y;
			}
			component.sizeDelta = sizeDelta;
		}
	}

	public override void SetLayoutVertical()
	{
		base.SetLayoutVertical();
		if (!(heightLimit.x >= 0f) || !(heightLimit.y >= 0f) || !(heightLimit.y < heightLimit.x))
		{
			RectTransform component = GetComponent<RectTransform>();
			Vector2 sizeDelta = component.sizeDelta;
			if (heightLimit.x >= 0f && sizeDelta.y < heightLimit.x)
			{
				sizeDelta.y = heightLimit.x;
			}
			if (heightLimit.y >= 0f && sizeDelta.y > heightLimit.y)
			{
				sizeDelta.y = heightLimit.y;
			}
			component.sizeDelta = sizeDelta;
		}
	}
}
