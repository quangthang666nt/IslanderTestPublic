using System.Collections.Generic;
using UnityEngine;

public class UIPageSizeCalculator : MonoBehaviour
{
	[SerializeField]
	private List<UICombinedElement> elements;

	[SerializeField]
	private float minimalX;

	public float GetWidth()
	{
		float num = 0f;
		foreach (UICombinedElement element in elements)
		{
			float x = element.GetSizeDelta().x;
			if (x > num)
			{
				num = x;
			}
		}
		if (num < minimalX)
		{
			num = minimalX;
		}
		return num;
	}

	[ContextMenu("Log Size")]
	public void LogSize()
	{
		Debug.Log(GetWidth());
	}

	public void Add(UICombinedElement element)
	{
		elements.Add(element);
	}
}
