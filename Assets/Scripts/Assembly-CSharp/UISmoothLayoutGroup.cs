using System.Collections.Generic;
using UnityEngine;

public class UISmoothLayoutGroup : MonoBehaviour
{
	public class SmoothingData
	{
		public Transform transReference;

		public Vector3 v3Origin = Vector3.zero;

		public Vector3 v3Target = Vector3.zero;

		public Vector3 v3SmoothRef = Vector3.zero;

		public SmoothingData(Transform _transReference, Vector3 _v3Origin, Vector3 _v3Target)
		{
			transReference = _transReference;
			v3Target = _v3Target;
			v3Origin = _v3Origin;
		}
	}

	public float fRelativeMaxWidth = 0.67f;

	public float fPrefferedSpacing;

	public float fMinimumSpacing;

	public float fSmoothTime = 1f;

	public List<Transform> transScaleChildren = new List<Transform>();

	private float fOverallWidth;

	private List<Transform> goCurrentChildren = new List<Transform>();

	private List<Transform> goPreviousChildren = new List<Transform>();

	private RectTransform thisRT;

	private bool doRebuild = true;

	private List<SmoothingData> smoothingData = new List<SmoothingData>();

	private Vector3 v3TargetScale = Vector3.one;

	private Vector3 v3OriginScale = Vector3.one;

	private float fSmoothingTimer;

	public float DisplayWidth => fOverallWidth * base.transform.localScale.x;

	private float FHalfExtent => fOverallWidth / 2f;

	private void Update()
	{
		goCurrentChildren.Clear();
		goCurrentChildren.AddRange(base.transform.GetComponentsInChildren<Transform>(includeInactive: false));
		if (goCurrentChildren.Count != goPreviousChildren.Count)
		{
			MarkForRebuild();
		}
		else
		{
			for (int i = 0; i < goCurrentChildren.Count; i++)
			{
				if (goCurrentChildren[i] != goPreviousChildren[i])
				{
					MarkForRebuild();
					break;
				}
			}
		}
		goPreviousChildren.Clear();
		goPreviousChildren.AddRange(goCurrentChildren);
		if (doRebuild)
		{
			RebuildLayout();
			if (UICameraSpaceHelper.TransCnvs.sizeDelta.x > 0f)
			{
				doRebuild = false;
			}
		}
		if (!(fSmoothingTimer <= fSmoothTime))
		{
			return;
		}
		fSmoothingTimer += Time.deltaTime;
		float t = Mathf.InverseLerp(0f, fSmoothTime, fSmoothingTimer);
		foreach (SmoothingData smoothingDatum in smoothingData)
		{
			smoothingDatum.transReference.localPosition = Vector3.Slerp(smoothingDatum.v3Origin, smoothingDatum.v3Target, t);
		}
		base.transform.localScale = Vector3.Slerp(v3OriginScale, v3TargetScale, t);
		foreach (Transform transScaleChild in transScaleChildren)
		{
			transScaleChild.localScale = base.transform.localScale;
		}
	}

	public void MarkForRebuild()
	{
		doRebuild = true;
	}

	[ContextMenu("Rebuild Layout")]
	private void RebuildLayout()
	{
		thisRT = GetComponent<RectTransform>();
		float num = 0f;
		int num2 = 0;
		foreach (RectTransform item in thisRT)
		{
			if (item.gameObject.activeInHierarchy)
			{
				num += item.sizeDelta.x;
				num2++;
			}
		}
		float num3 = UICameraSpaceHelper.TransCnvs.sizeDelta.x * fRelativeMaxWidth;
		float num4 = (num3 - num) / (float)(num2 - 1);
		if (num4 < fMinimumSpacing)
		{
			num4 = fMinimumSpacing;
		}
		else if (num4 > fPrefferedSpacing)
		{
			num4 = fPrefferedSpacing;
		}
		fOverallWidth = 0f;
		foreach (RectTransform item2 in thisRT)
		{
			if (item2.gameObject.activeInHierarchy)
			{
				fOverallWidth += item2.sizeDelta.x + num4;
			}
		}
		fOverallWidth -= num4;
		float fHalfExtent = FHalfExtent;
		float num5 = 0f;
		smoothingData.Clear();
		for (int i = 0; i < thisRT.childCount; i++)
		{
			RectTransform component = thisRT.GetChild(i).GetComponent<RectTransform>();
			if (component.gameObject.activeInHierarchy)
			{
				Vector3 zero = Vector3.zero;
				zero.x = 0f - fHalfExtent + num5 + component.sizeDelta.x / 2f;
				smoothingData.Add(new SmoothingData(component, component.localPosition, zero));
				num5 += component.sizeDelta.x;
				if (i != thisRT.childCount - 1)
				{
					num5 += num4;
				}
			}
		}
		float value = num3 / num5;
		value = Mathf.Clamp(value, 0.01f, 1f);
		v3OriginScale = base.transform.localScale;
		v3TargetScale = Vector3.one * value;
		fSmoothingTimer = 0f;
	}
}
