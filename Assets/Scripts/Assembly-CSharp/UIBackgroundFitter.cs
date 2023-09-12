using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIBackgroundFitter : MonoBehaviour
{
	public enum Axis
	{
		XY = 0,
		X = 1,
		Y = 2
	}

	[SerializeField]
	private RectTransform target;

	[SerializeField]
	private Axis matchingAxis;

	[SerializeField]
	private float minX;

	[SerializeField]
	private float minY;

	[SerializeField]
	private bool minSizeIncludePadding;

	[SerializeField]
	private float xPadding;

	[SerializeField]
	private float yPadding;

	private RectTransform rTransform;

	private Vector2 lastTargetSizeDelta;

	private void Start()
	{
		rTransform = GetComponent<RectTransform>();
		AdjustSize();
	}

	private void Update()
	{
		if (target.sizeDelta != lastTargetSizeDelta)
		{
			AdjustSize();
		}
	}

	[ContextMenu("Adjust Size")]
	private void AdjustSize()
	{
		if (rTransform == null)
		{
			rTransform = GetComponent<RectTransform>();
		}
		lastTargetSizeDelta = target.sizeDelta;
		Vector2 sizeDelta = rTransform.sizeDelta;
		if (matchingAxis == Axis.X || matchingAxis == Axis.XY)
		{
			if (minSizeIncludePadding)
			{
				sizeDelta.x = lastTargetSizeDelta.x + xPadding * 2f;
				if (sizeDelta.x < minX)
				{
					sizeDelta.x = minX;
				}
			}
			else if (lastTargetSizeDelta.x < minX)
			{
				sizeDelta.x = minX + xPadding * 2f;
			}
			else
			{
				sizeDelta.x = lastTargetSizeDelta.x + xPadding * 2f;
			}
		}
		if (matchingAxis == Axis.Y || matchingAxis == Axis.XY)
		{
			if (minSizeIncludePadding)
			{
				sizeDelta.y = lastTargetSizeDelta.y + yPadding * 2f;
				if (sizeDelta.y < minY)
				{
					sizeDelta.y = minY;
				}
			}
			else if (lastTargetSizeDelta.y < minY)
			{
				sizeDelta.y = minY + yPadding * 2f;
			}
			else
			{
				sizeDelta.y = lastTargetSizeDelta.y + yPadding * 2f;
			}
		}
		rTransform.sizeDelta = sizeDelta;
	}

	public void SetTarget(RectTransform newTarget)
	{
		target = newTarget;
	}
}
