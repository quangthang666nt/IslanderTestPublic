using System.Collections.Generic;
using UnityEngine;

public static class MenuHelper
{
	public enum AxisDirection
	{
		Horizontal = 0,
		Vertical = 1,
		None = 2
	}

	public static T FindNavigationItemInDirection<T>(T current, List<T> items, Vector3 dir, bool allowLoop = false, bool usePositionOnly = false, AxisDirection forcedAxisDirection = AxisDirection.None) where T : class, INavigationItem
	{
		if (current == null)
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].IsAvailableForNavigation)
				{
					return items[i];
				}
			}
			return null;
		}
		if (forcedAxisDirection != AxisDirection.None)
		{
			return FindNavigationItemInForcedAxis(current, items, dir, allowLoop, forcedAxisDirection);
		}
		dir = current.RectTransform.rotation * dir;
		dir = dir.normalized;
		Vector3 vector = Quaternion.Inverse(current.RectTransform.rotation) * dir;
		Vector3 zero = Vector3.zero;
		zero = ((!usePositionOnly) ? current.RectTransform.TransformPoint(GetPointOnRectEdge(current.RectTransform, vector)) : current.RectTransform.position);
		float num = float.NegativeInfinity;
		float num2 = float.NegativeInfinity;
		T val = null;
		T result = null;
		for (int j = 0; j < items.Count; j++)
		{
			T val2 = items[j];
			if (val2 == current || val2 == null || !val2.IsAvailableForNavigation)
			{
				continue;
			}
			Vector3 zero2 = Vector3.zero;
			zero2 = ((!usePositionOnly) ? (val2.RectTransform.TransformPoint(val2.RectTransform.rect.center) - zero) : (val2.RectTransform.position - zero));
			float num3 = Vector3.Dot(dir, zero2.normalized);
			if (num3 == 0f)
			{
				continue;
			}
			if (num3 > 0f)
			{
				float num4 = num3 / Mathf.Max(zero2.magnitude, 0.0001f);
				num4 -= zero2.magnitude;
				if (num4 > num)
				{
					num = num4;
					val = val2;
				}
			}
			else if (allowLoop)
			{
				float num5 = ((Mathf.Abs(num3) > 0.999f) ? zero2.sqrMagnitude : 0f);
				if (num5 > num2)
				{
					num2 = num5;
					result = val2;
				}
			}
		}
		if (val == null)
		{
			return result;
		}
		return val;
	}

	private static T FindNavigationItemInForcedAxis<T>(T current, List<T> items, Vector3 dir, bool allowLoop, AxisDirection forcedAxisDirection) where T : class, INavigationItem
	{
		int num = items.IndexOf(current);
		int num2 = 0;
		switch (forcedAxisDirection)
		{
		case AxisDirection.Horizontal:
			num2 = ((dir.x > 0f) ? 1 : (-1));
			break;
		case AxisDirection.Vertical:
			num2 = ((!(dir.y > 0f)) ? 1 : (-1));
			break;
		}
		switch (num2)
		{
		case 1:
			if (allowLoop)
			{
				for (int j = 1; j < items.Count; j++)
				{
					int index = (num + j) % items.Count;
					if (items[index].IsAvailableForNavigation)
					{
						return items[index];
					}
				}
			}
			else
			{
				for (int k = num + 1; k < items.Count; k++)
				{
					if (items[k].IsAvailableForNavigation)
					{
						return items[k];
					}
				}
			}
			return items[num];
		case -1:
			if (allowLoop)
			{
				for (int i = 1; i < items.Count; i++)
				{
					int num3 = num - i;
					if (num3 < 0)
					{
						num3 = items.Count + num3;
					}
					if (items[num3].IsAvailableForNavigation)
					{
						return items[num3];
					}
				}
			}
			else
			{
				for (int num4 = num - 1; num4 >= 0; num4--)
				{
					if (items[num4].IsAvailableForNavigation)
					{
						return items[num4];
					}
				}
			}
			return items[num];
		default:
			Debug.LogError("ForcedAxis with AxisDirection.None");
			return null;
		}
	}

	public static T FindClosestItemToPosition<T>(List<T> items, Vector3 position) where T : class, INavigationItem
	{
		float num = -1f;
		T result = null;
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].IsAvailableForNavigation)
			{
				float num2 = Vector3.Distance(position, items[i].RectTransform.position);
				if (num < 0f || num2 < num)
				{
					num = num2;
					result = items[i];
				}
			}
		}
		return result;
	}

	public static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
	{
		if (rect == null)
		{
			return Vector3.zero;
		}
		if (dir != Vector2.zero)
		{
			dir /= Mathf.Max(Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y)), 0.001f);
		}
		dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
		return dir;
	}
}
