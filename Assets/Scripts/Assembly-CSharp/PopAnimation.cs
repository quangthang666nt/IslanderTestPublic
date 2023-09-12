using System.Collections;
using UnityEngine;

public class PopAnimation : MonoBehaviour
{
	[SerializeField]
	private bool bPlayOnEnable = true;

	[SerializeField]
	private AnimationCurve acScaleCurve;

	[SerializeField]
	private float fTargetScale = 1f;

	[SerializeField]
	private float fAnimationTime = 1f;

	private void OnEnable()
	{
		if (bPlayOnEnable)
		{
			StartCoroutine(Wiggle());
		}
	}

	public IEnumerator Wiggle()
	{
		float fTimer = 0f;
		while (fTimer < fAnimationTime && this != null)
		{
			fTimer += Time.deltaTime;
			float time = Mathf.InverseLerp(0f, fAnimationTime, fTimer);
			base.transform.localScale = fTargetScale * acScaleCurve.Evaluate(time) * Vector3.one;
			yield return null;
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}
