using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UICombinedElement : MonoBehaviour
{
	public enum Axis
	{
		XY = 0,
		X = 1,
		Y = 2
	}

	[SerializeField]
	public List<RectTransform> elements;

	[SerializeField]
	private Axis axis = Axis.X;

	[SerializeField]
	private float extraSpaceX;

	[SerializeField]
	private float extraSpaceY;

	[SerializeField]
	private float minimalX;

	[SerializeField]
	private float minimalY;

	private RectTransform rTransform;

	private void Awake()
	{
		rTransform = GetComponent<RectTransform>();
	}

	public Vector2 GetSizeDelta()
	{
		if (rTransform == null)
		{
			rTransform = GetComponent<RectTransform>();
		}
		Vector2 sizeDelta = rTransform.sizeDelta;
		Vector2 zero = Vector2.zero;
		foreach (RectTransform element in elements)
		{
			UIElementOverrideSize component = element.GetComponent<UIElementOverrideSize>();
			if (component != null)
			{
				zero += component.size;
			}
			else
			{
				zero += element.sizeDelta;
			}
		}
		zero.x += extraSpaceX;
		zero.y += extraSpaceY;
		if (zero.x < minimalX)
		{
			zero.x = minimalX;
		}
		if (zero.y < minimalY)
		{
			zero.y = minimalY;
		}
		if (axis == Axis.X || axis == Axis.XY)
		{
			sizeDelta.x = zero.x;
		}
		if (axis == Axis.Y || axis == Axis.XY)
		{
			sizeDelta.y = zero.y;
		}
		return sizeDelta;
	}

	[ContextMenu("Log Size")]
	public void LogSize()
	{
		Debug.Log(GetSizeDelta());
	}
}
